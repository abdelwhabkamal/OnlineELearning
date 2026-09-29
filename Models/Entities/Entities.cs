using AsqueraLms.Api.Models.Enums;

namespace AsqueraLms.Api.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public StudentProfile? StudentProfile { get; set; }
}

public class Center
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. Louran Academy, Alpha Station, Smouha Academy, Sedi beshr, Online
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsOnline { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<CenterSchedule> Schedules { get; set; } = new List<CenterSchedule>();
}

public class CenterSchedule
{
    public int Id { get; set; }
    public int CenterId { get; set; }
    public Center Center { get; set; } = null!;
    public string LectureTitle { get; set; } = string.Empty;
    public DateTime SessionTime { get; set; }
    public string? Classroom { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. "Online - Group O", "Louran - Group L", "Alpha Station - Group F"
    public int CenterId { get; set; }
    public Center Center { get; set; } = null!;
    public int AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    public ICollection<StudentProfile> Students { get; set; } = new List<StudentProfile>();
}

public class AcademicYear
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "الصف الثالث الثانوي", "Senior 3"
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}

public class StudentProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string AsqueraCode { get; set; } = string.Empty; // e.g. "641FF", "076QT", "977UI"
    public string? QrCodePayload { get; set; }
    public string ParentPhoneNumber { get; set; } = string.Empty;
    public string? SchoolName { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Pending;
    public bool IsTopStudent { get; set; } = false;

    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    public ICollection<AttendanceRecord> Attendances { get; set; } = new List<AttendanceRecord>();
    public ICollection<ExamSubmission> ExamSubmissions { get; set; } = new List<ExamSubmission>();
    public ICollection<PaymentTransaction> Payments { get; set; } = new List<PaymentTransaction>();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    public ICollection<CurriculumUnit> Units { get; set; } = new List<CurriculumUnit>();
}

public class CurriculumUnit
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}

public class Lecture
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public CurriculumUnit Unit { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? AttachmentPdfUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsFree { get; set; } = false;
    public int Order { get; set; }

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? LectureId { get; set; }
    public Lecture? Lecture { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public int TotalMarks { get; set; } = 100;
    public int PassingMarks { get; set; } = 50;
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<ExamSubmission> Submissions { get; set; } = new List<ExamSubmission>();
}

public class Question
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public string QuestionText { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Marks { get; set; } = 1;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
}

public class ExamSubmission
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile Student { get; set; } = null!;
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public int Score { get; set; }
    public bool Passed { get; set; }
}

public class AttendanceRecord
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile Student { get; set; } = null!;
    public int? CenterScheduleId { get; set; }
    public CenterSchedule? CenterSchedule { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }
}

public class PaymentTransaction
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile Student { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.VodafoneCash;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
