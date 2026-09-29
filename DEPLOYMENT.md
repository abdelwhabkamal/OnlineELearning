## Asquera LMS API - Deployment Guide

### Architecture Considerations for Cloud Deployment
1. **Database:** Your local SQL Server (`ABDELWAHAB\SQLEXPRESS`) is on your laptop and not publicly accessible from the internet. When deploying to the cloud:
   - Option A: Create a free/low-cost cloud SQL Server (e.g., Azure SQL free tier, SmarterASP.NET MSSQL, or Aiven/Supabase PostgreSQL if converted).
   - Option B: SmarterASP.NET provides both .NET hosting and hosted MSSQL database in one place.
2. **Environment Variables:** Connection string and JWT secrets can be overridden directly in cloud provider settings via environment variable:
   `ConnectionStrings__DefaultConnection`

---

### Method 1: SmarterASP.NET (Easiest for .NET + SQL Server)
1. Sign up for a free 60-day trial at [SmarterASP.NET](https://www.smarterasp.net/).
2. Create an **MSSQL Database** in control panel, note the server address, db name, username, and password.
3. Publish from command line:
   ```powershell
   dotnet publish -c Release -o ./publish
   ```
4. Upload files from `./publish` via Web Deploy or FTP.
5. In `appsettings.json`, set `ConnectionStrings:DefaultConnection` to your SmarterASP MSSQL database connection string.

---

### Method 2: Render.com (Using the generated Dockerfile)
1. Initialize git and push to GitHub:
   ```powershell
   git init
   git add .
   git commit -m "Initial commit with Dockerfile"
   # Create a repo on GitHub, then link:
   git remote add origin https://github.com/<your-username>/<repo-name>.git
   git push -u origin main
   ```
2. Go to [Render.com](https://render.com) and click **New > Web Service**.
3. Select your GitHub repository.
4. Render will detect the [Dockerfile](file:///c:/Users/Lenovo%20-%20LOQ/Desktop/AsqueraLms.Api/Dockerfile).
5. In **Environment Variables**, add:
   - `ConnectionStrings__DefaultConnection`: `<Your-Public-SQLServer-Connection-String>`
   - `ASPNETCORE_ENVIRONMENT`: `Production`
6. Click **Deploy Web Service**. You will receive a permanent HTTPS URL like `https://asquera-lms.onrender.com`.

---

### Quick Testing Alternative: Cloudflare Tunnel / ngrok (Zero Config)
If you want to share your running local instance immediately without moving your database to the cloud:
1. Keep `dotnet run` running.
2. Run ngrok:
   ```powershell
   ngrok http 5128
   ```
3. Share the generated `https://xxxx.ngrok-free.app` URL with your frontend team.
