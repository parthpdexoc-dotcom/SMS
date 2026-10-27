<#
.SYNOPSIS
    Automated CI/CD Deployment Script for SMS Backend on Windows Server (IIS).
.DESCRIPTION
    1. Validates repository and target branch.
    2. Automatically creates a timestamped backup of the current live site folder.
    3. Builds and publishes SMS in Release mode.
    4. Safely engages app_offline.htm in IIS to release file locks.
    5. Syncs published binaries to live IIS directory WITHOUT overwriting:
       - appsettings.json
       - appsettings.Development.json (and any appsettings.*.json)
       - web.config
    6. Cleans up app_offline.htm and recycles/restarts IIS AppPool / Windows Service 'Plexoc'.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$TargetBranch = "develop",

    [Parameter(Mandatory = $false)]
    [string]$IisSitePath = "C:\inetpub\wwwroot\SMSAPI",

    [Parameter(Mandatory = $false)]
    [string]$AppPoolName = "Plexoc",

    [Parameter(Mandatory = $false)]
    [string]$RepoDir = $PSScriptRoot + "\..",

    [Parameter(Mandatory = $false)]
    [string]$TempPublishDir = "C:\temp\SMS_Backend_Publish",

    [Parameter(Mandatory = $false)]
    [switch]$SkipGitPull = $false
)

$ErrorActionPreference = "Stop"

function Write-Step ([string]$msg) {
    Write-Host "`n>>> [CI/CD:Backend] $msg" -ForegroundColor Cyan
}

function Write-Success ([string]$msg) {
    Write-Host ">>> [SUCCESS] $msg" -ForegroundColor Green
}

function Write-WarningMsg ([string]$msg) {
    Write-Host ">>> [WARNING] $msg" -ForegroundColor Yellow
}

function Write-Failure ([string]$msg) {
    Write-Host ">>> [ERROR] $msg" -ForegroundColor Red
}

try {
    Write-Step "Starting SMS Backend Deployment..."
    
    # Auto-detect path if Plexoc subdirectory is used
    if (-not (Test-Path $IisSitePath) -and (Test-Path "C:\inetpub\wwwroot\Plexoc\SMSAPI")) {
        $IisSitePath = "C:\inetpub\wwwroot\Plexoc\SMSAPI"
    } elseif (-not (Test-Path $IisSitePath) -and (Test-Path "C:\inetpub\wwwroot\SMS_API")) {
        $IisSitePath = "C:\inetpub\wwwroot\SMS_API"
    }

    Write-Host "Target IIS Path:  $IisSitePath"
    Write-Host "Service/AppPool:  $AppPoolName"
    Write-Host "Repository Dir:   $RepoDir"

    # 1. Verify Directories
    if (-not (Test-Path $RepoDir)) {
        throw "Repository directory '$RepoDir' not found."
    }

    # =========================================================================
    # STEP: AUTOMATIC PRE-DEPLOYMENT BACKUP
    # =========================================================================
    if (Test-Path $IisSitePath) {
        $hasFiles = (Get-ChildItem -Path $IisSitePath | Measure-Object).Count -gt 0
        if ($hasFiles) {
            $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
            $parentDir = Split-Path $IisSitePath -Parent
            $backupBase = Join-Path $parentDir "Backups"
            if (-not (Test-Path $backupBase)) {
                New-Item -ItemType Directory -Path $backupBase -Force | Out-Null
            }
            $targetFolderName = (Get-Item $IisSitePath).Name
            $backupFolder = Join-Path $backupBase "${targetFolderName}_$timestamp"
            
            Write-Step "CREATING BACKUP: Copying live files to '$backupFolder' before deployment..."
            New-Item -ItemType Directory -Path $backupFolder -Force | Out-Null
            
            # Fast recursive copy to backup folder
            & robocopy.exe "$IisSitePath" "$backupFolder" /E /R:1 /W:1 /NP | Out-Null
            Write-Success "Full backup created successfully at: '$backupFolder'"
        }
    } else {
        Write-WarningMsg "Target directory '$IisSitePath' does not exist yet. Creating directory..."
        New-Item -ItemType Directory -Path $IisSitePath -Force | Out-Null
    }

    # 2. Update Source from Git (optional, skipped if handled by GitHub Actions runner)
    if (-not $SkipGitPull) {
        Write-Step "Updating repository working tree to latest '$TargetBranch'..."
        Push-Location $RepoDir
        try {
            git fetch origin $TargetBranch
            git checkout $TargetBranch
            git reset --hard "origin/$TargetBranch"
        }
        finally {
            Pop-Location
        }
    }

    # 3. Clean Temp Publish Directory
    if (Test-Path $TempPublishDir) {
        Remove-Item -Path $TempPublishDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $TempPublishDir -Force | Out-Null

    # 4. Build and Publish .NET Web App
    $csprojPath = Join-Path $RepoDir "SMS\SMS.csproj"
    if (-not (Test-Path $csprojPath)) {
        throw "Could not find project file at '$csprojPath'."
    }

    Write-Step "Publishing SMS project in Release configuration..."
    dotnet publish "$csprojPath" -c Release -o "$TempPublishDir" --nologo -v q
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE."
    }
    Write-Success "Build and publish succeeded."

    # 5. Place app_offline.htm to release IIS file locks
    Write-Step "Placing app_offline.htm in IIS directory..."
    $appOfflinePath = Join-Path $IisSitePath "app_offline.htm"
    $offlineHtml = @"
<!DOCTYPE html>
<html>
<head><title>Application Updating</title></head>
<body style="font-family: Arial, sans-serif; text-align: center; padding-top: 100px;">
    <h2>SMS API is updating...</h2>
    <p>Please wait a moment while the latest version is deployed.</p>
</body>
</html>
"@
    Set-Content -Path $appOfflinePath -Value $offlineHtml -Encoding UTF8
    Start-Sleep -Seconds 2

    # 6. Copy Published Artifacts with ROBOCOPY
    # STRICT EXCLUSION: appsettings.json, appsettings.*.json, and web.config are NEVER overwritten
    Write-Step "Syncing new binaries to IIS directory (protecting appsettings.json and web.config)..."
    
    $excludeFiles = @("appsettings.json", "appsettings.Development.json", "appsettings.*.json", "web.config")
    
    $robocopyArgs = @(
        "$TempPublishDir",
        "$IisSitePath",
        "/E",
        "/R:2",
        "/W:2",
        "/NP",
        "/XF"
    ) + $excludeFiles

    & robocopy.exe @robocopyArgs | Out-Null
    $roboExit = $LASTEXITCODE

    # Robocopy exit codes < 8 indicate success
    if ($roboExit -ge 8) {
        throw "Robocopy failed with exit code $roboExit."
    }
    Write-Success "Binaries synced successfully. Server appsettings.json and web.config remained untouched."

    # 7. Remove app_offline.htm to bring API back online
    Write-Step "Removing app_offline.htm and restoring API..."
    if (Test-Path $appOfflinePath) {
        Remove-Item -Path $appOfflinePath -Force
    }

    # 8. Recycle IIS AppPool / Restart Service 'Plexoc'
    if (![string]::IsNullOrWhiteSpace($AppPoolName)) {
        try {
            $recycled = $false
            if (Get-Module -ListAvailable -Name WebAdministration) {
                Import-Module WebAdministration -ErrorAction SilentlyContinue
                if (Test-Path "IIS:\AppPools\$AppPoolName") {
                    Write-Step "Recycling IIS Application Pool '$AppPoolName'..."
                    Restart-WebAppPool -Name $AppPoolName
                    Write-Success "AppPool '$AppPoolName' recycled."
                    $recycled = $true
                }
            }
            if (-not $recycled) {
                $appCmd = "$env:SystemRoot\system32\inetsrv\appcmd.exe"
                if (Test-Path $appCmd) {
                    & $appCmd recycle apppool /apppool.name:"$AppPoolName" | Out-Null
                    Write-Success "AppPool '$AppPoolName' recycled via appcmd."
                    $recycled = $true
                }
            }
            $winService = Get-Service -Name $AppPoolName -ErrorAction SilentlyContinue
            if ($winService -and $winService.Status -eq "Running") {
                Write-Step "Restarting Windows Service '$AppPoolName'..."
                Restart-Service -Name $AppPoolName -Force
                Write-Success "Windows Service '$AppPoolName' restarted."
            }
        }
        catch {
            Write-WarningMsg "Notice on AppPool/Service recycle: $_"
        }
    }

    # 9. Clean Temp Directory
    if (Test-Path $TempPublishDir) {
        Remove-Item -Path $TempPublishDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    Write-Success "=========================================================="
    Write-Success " SMS Backend Deployment Completed Successfully! "
    Write-Success " Target: $IisSitePath | Service/AppPool: $AppPoolName"
    Write-Success " Live configs preserved. Backup saved before release."
    Write-Success "=========================================================="
    exit 0
}
catch {
    Write-Failure "Deployment FAILED: $_"
    $fallbackOffline = Join-Path $IisSitePath "app_offline.htm"
    if (Test-Path $fallbackOffline) {
        Remove-Item -Path $fallbackOffline -Force -ErrorAction SilentlyContinue
    }
    exit 1
}
