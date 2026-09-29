using Microsoft.EntityFrameworkCore;
using OELearning.Api.Models.Entities;
using OELearning.Api.Models.Enums;

namespace OELearning.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Center> Centers => Set<Center>();
    public DbSet<CenterSchedule> CenterSchedules => Set<CenterSchedule>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CurriculumUnit> CurriculumUnits => Set<CurriculumUnit>();
    public DbSet<Lecture> Lectures => Set<Lecture>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<ExamSubmission> ExamSubmissions => Set<ExamSubmission>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<StudentProfile>()
            .HasIndex(s => s.OELearningCode)
            .IsUnique();

        modelBuilder.Entity<StudentProfile>()
            .HasOne(s => s.User)
            .WithOne(u => u.StudentProfile)
            .HasForeignKey<StudentProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Center)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CenterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Lecture>()
            .Property(l => l.Price)
            .HasColumnType("decimal(18,2)");
    }
}
