using OELearning.Api.Models.Enums;

namespace OELearning.Api.DTOs.Auth
{
    public record LoginRequestDto(string PhoneNumber, string Password);
    public record AuthResponseDto(string Token, string FullName, string PhoneNumber, string Role, int UserId, int? StudentProfileId);
    public record RegisterStudentDto(
        string FullName,
        string PhoneNumber,
        string ParentPhoneNumber,
        string Password,
        int GroupId
    );
}

namespace OELearning.Api.DTOs.Students
{
    public record StudentListDto(
        int Id,
        int UserId,
        int RowNumber,
        string FullName,
        string OELearningCode,
        string GroupName,
        string CenterName,
        string StudentPhoneNumber,
        string ParentPhoneNumber,
        string WhatsAppStudentUrl,
        string WhatsAppParentUrl,
        string Status,
        bool IsTopStudent,
        string? QrCodeBase64
    );

    public record CreateStudentDto(
        string FullName,
        string PhoneNumber,
        string ParentPhoneNumber,
        int GroupId,
        string Password
    );

    public record UpdateStudentStatusDto(StudentStatus Status);

    public record BulkImportStudentItem(
        string FullName,
        string PhoneNumber,
        string ParentPhoneNumber,
        string GroupName
    );
}

namespace OELearning.Api.DTOs.Centers
{
    public record CenterDto(
        int Id,
        string Name,
        string? Address,
        string? ContactPhone,
        bool IsOnline,
        int GroupsCount,
        int StudentsCount
    );

    public record CreateCenterDto(string Name, string? Address, string? ContactPhone, bool IsOnline);

    public record CenterScheduleDto(
        int Id,
        int CenterId,
        string CenterName,
        string LectureTitle,
        DateTime SessionTime,
        string? Classroom
    );
}

namespace OELearning.Api.DTOs.Curriculum
{
    public record CourseDto(int Id, string Title, string? Description, int AcademicYearId, List<UnitDto> Units);
    public record UnitDto(int Id, string Title, int Order, List<LectureDto> Lectures);
    public record LectureDto(int Id, string Title, string? Description, string? VideoUrl, string? AttachmentPdfUrl, decimal Price, bool IsFree, int Order);
}

namespace OELearning.Api.DTOs.Exams
{
    public record ExamDto(int Id, string Title, int DurationMinutes, int TotalMarks, int PassingMarks, int QuestionsCount);
    public record ExamDetailDto(int Id, string Title, int DurationMinutes, int TotalMarks, List<QuestionDto> Questions);
    public record QuestionDto(int Id, string QuestionText, string? ImageUrl, int Marks, List<OptionDto> Options);
    public record OptionDto(int Id, string OptionText);
    public record SubmitExamDto(int ExamId, Dictionary<int, int> SelectedOptionByQuestionId);
    public record ExamResultDto(int SubmissionId, int ExamId, int Score, int TotalMarks, bool Passed, DateTime SubmittedAt);
}
