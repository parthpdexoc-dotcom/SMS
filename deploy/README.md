# SMS Backend CI/CD Setup for GitHub Actions & Windows Server (IIS)

This repository is configured with automated CI/CD using **GitHub Actions** and a **GitHub Self-Hosted Runner** installed on your Windows Server.

---

## 🔒 Configuration Protection (Zero Overwrite Guarantee)

When releasing the Backend, your live IIS server configuration is strictly protected. The deployment automation explicitly excludes:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json` (and all `appsettings.*.json`)
- `web.config`

Your production database connection strings, API secrets, JWT keys, Firebase configuration, and IIS rewrite rules will **NEVER** be overwritten or altered during automated releases.

---

## 🛡️ Automatic Pre-Deployment Backup

Before any new files are copied or synchronized, the deployment script automatically creates a full, timestamped copy of your existing live directory:
- 📁 `C:\inetpub\wwwroot\Backups\SMSAPI_YYYYMMDD_HHMMSS`

Your previous live release and server-side configurations are safely archived before every single deployment.

---

## ⚙️ Architecture & Sequence

```
[Developer / Git Push]
         │ (Pushes to develop / Develop / main)
         ▼
[GitHub Repository: SMS]
         │
         ▼ (Secure HTTPS Outbound Poll - No open firewall ports required)
[Windows Server: GitHub Self-Hosted Runner]
         │
         ├── 1. Code Checkout via actions/checkout@v4
         ├── 2. Setup .NET 8 SDK
         ├── 3. Pre-Deployment Backup to C:\inetpub\wwwroot\Backups\SMSAPI_YYYYMMDD_HHMMSS
         ├── 4. dotnet publish SMS\SMS.csproj -c Release
         ├── 5. Place app_offline.htm in IIS (releases DLL locks)
         ├── 6. Robocopy sync (Strictly excludes appsettings.*.json & web.config)
         ├── 7. Remove app_offline.htm (API back online)
         └── 8. Recycle IIS AppPool / Restart Service 'Plexoc'
```

---

## 🚀 Setting Up the GitHub Self-Hosted Runner (One-Time Setup)

To allow GitHub Actions to deploy directly to your IIS server:

### 1. Run Configuration Command
Open PowerShell as **Administrator** on the Windows Server:
```powershell
New-Item -ItemType Directory -Path "C:\actions-runner-sms" -Force | Out-Null
Set-Location "C:\actions-runner-sms"

# Copy runner zip or download
Copy-Item "C:\actions-runner\actions-runner.zip" "C:\actions-runner-sms\" -Force
Expand-Archive -Path "actions-runner.zip" -DestinationPath "." -Force

# Configure runner as an automatic Windows Service
.\config.cmd --url https://github.com/parthpdexoc-dotcom/SMS --token <SMS_BACKEND_TOKEN> --runasservice
```

---

## 🧪 Triggering Deployments

### 1. Automatic Deployment on Push
Any push to `develop`, `Develop`, or `main` automatically triggers deployment.

### 2. Manual Trigger via GitHub UI
1. Go to **Actions** tab in GitHub.
2. Select **Deploy Backend to IIS**.
3. Click **Run workflow**.

### 3. Manual Local Deployment (PowerShell)
You can test the deployment locally at any time:
```powershell
powershell -ExecutionPolicy Bypass -File "deploy\deploy.ps1" -IisSitePath "C:\inetpub\wwwroot\SMSAPI" -AppPoolName "Plexoc"
```
