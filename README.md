# OELearning E-Learning Platform - .NET 9 Web API

A clean, production-ready ASP.NET Core Web API tailored for the **OELearning** educational management and e-learning system shown in the dashboard screenshots.

---

## ?? Key Modules & Features

1. **?????? (Students Management)**:
   - Paginated listing with filtering by group, status (`Approved`, `Pending`, `Suspended`), and search by student name/mobile/parent mobile/OELearning code.
   - Dynamic **WhatsApp direct click-to-chat links** for both student & parent numbers (`https://wa.me/20...`).
   - Built-in **QR Code generation** for student cards & attendance scanning.
   - Student status changes (Approve, Suspend, Password Reset).
   - Bulk student import endpoint (`api/Students/bulk-import`).

2. **??????? ?????????? (Centers & Batches)**:
   - Complete CRUD for physical centers (*Louran Academy*, *Alpha Station*, *Smouha Academy*, *Sedi Beshr*) and *Online*.
   - Groups/Batches linked to centers and academic stages.
   - Center schedules and session timings matching dashboard UI.

3. **????? ?????? (Curriculum & Lessons)**:
   - Academic Year -> Courses -> Units -> Lectures hierarchy.
   - Video streaming URLs, attachment PDFs, pricing, and free preview flags.

4. **?????????? ???? ??????? (Exams & Question Bank)**:
   - Timed exams, passing score thresholds, questions with multi-choice options.
   - Automated exam evaluation and instant score calculation.

5. **Security & Database**:
   - JWT Bearer Authentication with role-based claims (`Admin`, `Teacher`, `Assistant`, `Student`).
   - BCrypt password hashing.
   - EF Core 9 with SQL Server integration.
   - Database auto-creation and seeding with mock data from the screenshots on first startup.

---

## ?? Getting Started

### 1. Requirements
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, or SQL Server Instance)

### 2. Database Connection
Open `appsettings.json` and adjust the connection string if needed:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OELearningDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 3. Run the Project
Navigate to the project folder and run:
```bash
dotnet run
```

### 4. Interactive Swagger UI
Open your browser at:
```
http://localhost:5000/
```
(Swagger UI is configured at the root URL `/` with full JWT Authorization header support).

---

## ?? Default Seeded Accounts
- **Admin**:
  - Phone: `01000000000`
  - Password: `Admin@123456`
- **Demo Students (from UI)**:
  - Phone: `01099277423` (Jana Ahmed)
  - Phone: `01091743748` (???? ???? ??? ????)
  - Phone: `01008290343` (Zamzam Waleed)
  - Password for all demo students: `Student@123`
