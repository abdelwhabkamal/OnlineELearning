# AWS Free Tier Deployment Guide: OELearning API + SQL Server

This approach uses **1 AWS EC2 instance (Free Tier)** that hosts both your **SQL Server** and your **Web API** together using Docker Compose. It costs **$0** within AWS Free Tier limits and gives you a public IP address for your frontend team.

---

## Step 1: Launch an AWS EC2 Instance (Free Tier)
1. Sign in to the [AWS Management Console](https://aws.amazon.com/console/).
2. In the top search bar, type **EC2** and click **EC2**.
3. Click the orange button **Launch instance**.
4. Configure the instance:
   - **Name**: `oelearning-server`
   - **OS / AMI**: **Ubuntu Server 24.04 LTS (HVM)** *(Eligible for Free Tier)*
   - **Instance type**: `t3.micro` (or `t2.micro` depending on region) *(Marked "Free tier eligible")*
   - **Key pair (login)**: Click **Create new key pair**:
     - Key pair name: `oelearning-key`
     - Private key file format: `.pem`
     - Download and keep this file safe on your computer.
   - **Network settings (Firewall / Security Group)**:
     - Check **Allow SSH traffic from Anywhere (0.0.0.0/0)**
     - Check **Allow HTTP traffic from the internet (Port 80)**
     - Click **Edit** (under Network settings) > **Add security group rule**:
       - Type: **Custom TCP**
       - Port: `1433`
       - Source: `Anywhere (0.0.0.0/0)` *(Allows you to connect to SQL Server from SQL Server Management Studio on your laptop)*
   - **Configure Storage**: Set size to **25 GiB** (AWS Free Tier gives up to 30 GB gp3 storage free).
5. Click **Launch instance**.

---

## Step 2: Connect to your Server
1. Go to the **EC2 Instances** page and click your instance.
2. Note your **Public IPv4 address** (e.g. `3.85.120.45`).
3. Click **Connect** at the top > **EC2 Instance Connect** tab > click the orange **Connect** button (this opens a Linux terminal in your browser with 1 click!).

---

## Step 3: Install Docker & Docker Compose (Takes 1 minute)
Inside the browser terminal, run these commands:

```bash
# Update and install Docker
sudo apt update && sudo apt install -y docker.io docker-compose-v2 git

# Enable Docker without sudo
sudo usermod -aG docker $USER
newgrp docker
```

---

## Step 4: Clone Your Project & Start Everything
Run in the same terminal:

```bash
# 1. Clone your repo
git clone https://github.com/abdelwhabkamal/OnlineELearning.git
cd OnlineELearning

# 2. Run both SQL Server and .NET API with one command!
docker compose up -d --build
```

---

## Step 5: Test & Share with Frontend
- **Swagger / API URL**:
  `http://<YOUR_EC2_PUBLIC_IP>/`
  *(e.g. `http://3.85.120.45/`)*
- **Connecting SSMS from your laptop**:
  - Server name: `<YOUR_EC2_PUBLIC_IP>,1433`
  - Login: `sa`
  - Password: `YourStrong@Password123!`
  - Encryption: Optional / Mandatory with Trust Server Certificate checked.
