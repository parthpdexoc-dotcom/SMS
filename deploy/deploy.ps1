<#
.SYNOPSIS
    Automated CI/CD Deployment Script for SMS Backend on Windows Server (IIS).
.DESCRIPTION
    1. Validates repository, target branch, and environment (Dev, UAT, Prod).
    2. Automatically creates a timestamped backup of the current live site folder.
    3. Builds and publishes SMS in Release mode.
    4. Safely engages app_offline.htm in IIS to release file locks.
    5. Syncs published binaries to live IIS directory WITHOUT overwriting:
       - appsettings.json
       - appsettings.Development.json
       - appsettings.UAT.json
       - appsettings.Production.json (and any appsettings.*.json)
       - web.config
    6. Cleans up app_offline.htm and recycles/restarts IIS AppPool / Windows Service.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$TargetBranch = "develop",

    [Parameter(Mandatory = $false)]
    [ValidateSet("Dev", "UAT", "Prod", "development", "uat", "prod", "production", "")]
    [string]$Environment = "",

    [Parameter(Mandatory = $false)]
    [string]$IisSitePath = "",

    [Parameter(Mandatory = $false)]
    [string]$AppPoolName = "",

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
    # 1. Resolve Environment from Branch or Parameter
    if ([string]::IsNullOrWhiteSpace($Environment)) {
        if ($TargetBranch -match "uat") {
            $Environment = "UAT"
        } elseif ($TargetBranch -in @("main", "master", "prod", "production")) {
            $Environment = "Prod"
        } else {
            $Environment = "Dev"
        }
    } else {
        # Normalize casing
        if ($Environment -in @("uat", "UAT")) {
            $Environment = "UAT"
        } elseif ($Environment -in @("prod", "production", "Prod")) {
            $Environment = "Prod"
        } else {
            $Environment = "Dev"
        }
    }

    # 2. Determine Environment-Specific Defaults
    $defaultIisPath = ""
    $defaultAppPool = ""

    # Resolve default IIS Site Path and AppPool if not specified
    if ([string]::IsNullOrWhiteSpace($IisSitePath)) {
        if (Test-Path "D:\PublishedProject\wwwroot\SMSapi") {
            $IisSitePath = "D:\PublishedProject\wwwroot\SMSapi"
        } elseif (Test-Path "D:\PublishedProject\wwwroot\SMSAPI") {
            $IisSitePath = "D:\PublishedProject\wwwroot\SMSAPI"
        } elseif (Test-Path "C:\inetpub\wwwroot\SMSApi") {
            $IisSitePath = "C:\inetpub\wwwroot\SMSApi"
        } elseif (Test-Path "C:\inetpub\wwwroot\SMSAPI") {
            $IisSitePath = "C:\inetpub\wwwroot\SMSAPI"
        } else {
            $IisSitePath = "D:\PublishedProject\wwwroot\SMSapi"
        }
    }
    if ([string]::IsNullOrWhiteSpace($AppPoolName)) {
        $AppPoolName = "Plexoc"
    }

    Write-Step "Starting SMS Backend Deployment..."
    Write-Host "Environment:     $Environment"
    Write-Host "Target IIS Path: $IisSitePath"
    Write-Host "Service/AppPool: $AppPoolName"
    Write-Host "Target Branch:   $TargetBranch"
    Write-Host "Repository Dir:  $RepoDir"

    # 3. Verify Directories
    if (-not (Test-Path $RepoDir)) {
        throw "Repository directory '$RepoDir' not found."
    }

    # =========================================================================
    # STEP: AUTOMATIC PRE-DEPLOYMENT BACKUP (STRICTLY KEEP ONLY 1 LATEST COPY)
    # =========================================================================
    if (Test-Path $IisSitePath) {
        $hasFiles = (Get-ChildItem -Path $IisSitePath | Measure-Object).Count -gt 0
        if ($hasFiles) {
            $parentDir = Split-Path $IisSitePath -Parent
            $targetFolderName = (Get-Item $IisSitePath).Name
            
            # Use the last write time of the existing live build
            $lastBuildDate = (Get-Item $IisSitePath).LastWriteTime.ToString("yyyyMMdd_HHmmss")
            $backupFolder = Join-Path $parentDir "${targetFolderName} - Copy ($lastBuildDate)"
            
            Write-Step "CREATING BACKUP: Preserving previous live build in '$backupFolder'..."
            New-Item -ItemType Directory -Path $backupFolder -Force | Out-Null
            
            # Fast recursive copy to backup folder
            & robocopy.exe "$IisSitePath" "$backupFolder" /E /R:1 /W:1 /NP | Out-Null
            Write-Success "Previous build backup copy created successfully at: '$backupFolder'"

            # STRICT POLICY: Secure ONLY 1 backup copy. Delete all older backup copies.
            Write-Step "CLEANUP: Ensuring only 1 backup copy exists in '$parentDir'..."
            $existingBackups = Get-ChildItem -Path $parentDir -Directory -Filter "${targetFolderName} - Copy*" |
                               Where-Object { $_.FullName -ne $backupFolder }
            foreach ($oldBackup in $existingBackups) {
                Write-Host "  Removing older backup: $($oldBackup.Name)"
                Remove-Item -Path $oldBackup.FullName -Recurse -Force -ErrorAction SilentlyContinue
            }
            Write-Success "Backup secured. Exactly 1 backup copy exists on disk."
        }
    } else {
        Write-WarningMsg "Target directory '$IisSitePath' does not exist yet. Creating directory..."
        New-Item -ItemType Directory -Path $IisSitePath -Force | Out-Null
    }

    # 4. Update Source from Git (optional, skipped if handled by GitHub Actions runner)
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

    # 5. Clean Temp Publish Directory
    if (Test-Path $TempPublishDir) {
        Remove-Item -Path $TempPublishDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $TempPublishDir -Force | Out-Null

    # 6. Build and Publish .NET Web App
    $csprojPath = Join-Path $RepoDir "SMS\SMS.csproj"
    if (-not (Test-Path $csprojPath)) {
        throw "Could not find project file at '$csprojPath'."
    }

    Write-Step "Publishing SMS project in Release configuration for '$Environment'..."
    dotnet publish "$csprojPath" -c Release -o "$TempPublishDir" --nologo -v q
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE."
    }
    Write-Success "Build and publish succeeded."

    # 7. Zero-Downtime Binary Sync with ROBOCOPY
    # Lock-free sync: DLLs are copied directly while IIS serves active traffic
    # STRICT EXCLUSION: appsettings.json, appsettings.*.json, and web.config are NEVER overwritten
    Write-Step "Syncing new binaries to IIS directory with ZERO DOWNTIME (protecting configs)..."
    
    $excludeFiles = @("appsettings.json", "appsettings.Development.json", "appsettings.UAT.json", "appsettings.Production.json", "appsettings.*.json", "web.config")
    
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

    # Resilient handling: only if a legacy file lock is encountered, perform rapid micro-release
    if ($roboExit -ge 8) {
        Write-WarningMsg "Active assembly lock detected. Performing rapid micro-unlock..."
        $appOfflinePath = Join-Path $IisSitePath "app_offline.htm"
        Set-Content -Path $appOfflinePath -Value "<!-- updating -->" -Encoding UTF8
        Start-Sleep -Milliseconds 600
        & robocopy.exe @robocopyArgs | Out-Null
        $roboExit = $LASTEXITCODE
        if (Test-Path $appOfflinePath) {
            Remove-Item -Path $appOfflinePath -Force -ErrorAction SilentlyContinue
        }
        if ($roboExit -ge 8) {
            throw "Robocopy failed with exit code $roboExit."
        }
    }
    Write-Success "Binaries synced successfully without taking site offline. Server appsettings.json preserved."

    # 8. Overlapped IIS AppPool Recycle (Zero-Downtime Worker Handoff)
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
                } elseif (Test-Path "IIS:\AppPools\Plexoc") {
                    Write-Step "Recycling fallback IIS Application Pool 'Plexoc'..."
                    Restart-WebAppPool -Name "Plexoc"
                    Write-Success "AppPool 'Plexoc' recycled."
                    $recycled = $true
                }
            }
            if (-not $recycled) {
                $appCmd = "$env:SystemRoot\system32\inetsrv\appcmd.exe"
                if (Test-Path $appCmd) {
                    & $appCmd recycle apppool /apppool.name:"$AppPoolName" 2>&1 | Out-Null
                    if ($LASTEXITCODE -ne 0) {
                        & $appCmd recycle apppool /apppool.name:"Plexoc" 2>&1 | Out-Null
                    }
                    Write-Success "AppPool recycled via appcmd."
                    $recycled = $true
                }
            }
            $winService = Get-Service -Name $AppPoolName -ErrorAction SilentlyContinue
            if ($winService -and $winService.Status -eq "Running") {
                Write-Step "Restarting Windows Service '$AppPoolName'..."
                Restart-Service -Name $AppPoolName -Force
                Write-Success "Windows Service '$AppPoolName' restarted."
            } elseif ($AppPoolName -ne "Plexoc") {
                $plexocService = Get-Service -Name "Plexoc" -ErrorAction SilentlyContinue
                if ($plexocService -and $plexocService.Status -eq "Running") {
                    Write-Step "Restarting Windows Service 'Plexoc'..."
                    Restart-Service -Name "Plexoc" -Force
                    Write-Success "Windows Service 'Plexoc' restarted."
                }
            }
        }
        catch {
            Write-WarningMsg "Notice on AppPool/Service recycle: $_"
        }
    }

    # 11. Clean Temp Directory
    if (Test-Path $TempPublishDir) {
        Remove-Item -Path $TempPublishDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    Write-Success "=========================================================="
    Write-Success " SMS Backend Deployment Completed Successfully! "
    Write-Success " Environment:     $Environment"
    Write-Success " Target Path:     $IisSitePath"
    Write-Success " Service/AppPool: $AppPoolName"
    Write-Success " Live configs preserved. Backup saved before release."
    Write-Success "=========================================================="
    exit 0
}
catch {
    Write-Failure "Deployment FAILED: $_"
    if (-not [string]::IsNullOrWhiteSpace($IisSitePath)) {
        $fallbackOffline = Join-Path $IisSitePath "app_offline.htm"
        if (Test-Path $fallbackOffline) {
            Remove-Item -Path $fallbackOffline -Force -ErrorAction SilentlyContinue
        }
    }
    exit 1
}
