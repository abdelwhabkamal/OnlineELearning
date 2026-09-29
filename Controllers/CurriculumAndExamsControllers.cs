using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OELearning.Api.Data;
using OELearning.Api.DTOs.Curriculum;
using OELearning.Api.DTOs.Exams;
using OELearning.Api.Models.Entities;

namespace OELearning.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Units)
                .ThenInclude(u => u.Lectures)
            .Select(c => new CourseDto(
                c.Id,
                c.Title,
                c.Description,
                c.AcademicYearId,
                c.Units.OrderBy(u => u.Order).Select(u => new UnitDto(
                    u.Id,
                    u.Title,
                    u.Order,
                    u.Lectures.OrderBy(l => l.Order).Select(l => new LectureDto(
                        l.Id,
                        l.Title,
                        l.Description,
                        l.VideoUrl,
                        l.AttachmentPdfUrl,
                        l.Price,
                        l.IsFree,
                        l.Order
                    )).ToList()
                )).ToList()
            ))
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("lectures/{id:int}")]
    public async Task<ActionResult<LectureDto>> GetLecture(int id)
    {
        var l = await _context.Lectures.FindAsync(id);
        if (l == null) return NotFound();

        return Ok(new LectureDto(
            l.Id,
            l.Title,
            l.Description,
            l.VideoUrl,
            l.AttachmentPdfUrl,
            l.Price,
            l.IsFree,
            l.Order
        ));
    }
}

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams()
    {
        var exams = await _context.Exams
            .Include(e => e.Questions)
            .Select(e => new ExamDto(
                e.Id,
                e.Title,
                e.DurationMinutes,
                e.TotalMarks,
                e.PassingMarks,
                e.Questions.Count
            ))
            .ToListAsync();

        return Ok(exams);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExamDetailDto>> GetExamDetails(int id)
    {
        var exam = await _context.Exams
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exam == null) return NotFound();

        var dto = new ExamDetailDto(
            exam.Id,
            exam.Title,
            exam.DurationMinutes,
            exam.TotalMarks,
            exam.Questions.Select(q => new QuestionDto(
                q.Id,
                q.QuestionText,
                q.ImageUrl,
                q.Marks,
                q.Options.Select(o => new OptionDto(o.Id, o.OptionText)).ToList()
            )).ToList()
        );

        return Ok(dto);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<ExamResultDto>> SubmitExam(
        [FromQuery] int studentProfileId,
        [FromBody] SubmitExamDto dto)
    {
        var exam = await _context.Exams
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == dto.ExamId);

        if (exam == null) return NotFound();

        int score = 0;
        foreach (var q in exam.Questions)
        {
            if (dto.SelectedOptionByQuestionId.TryGetValue(q.Id, out int selectedOptionId))
            {
                var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null && correctOption.Id == selectedOptionId)
                {
                    score += q.Marks;
                }
            }
        }

        var passed = score >= exam.PassingMarks;
        var submission = new ExamSubmission
        {
            StudentProfileId = studentProfileId,
            ExamId = exam.Id,
            Score = score,
            Passed = passed,
            SubmittedAt = DateTime.UtcNow
        };

        _context.ExamSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(new ExamResultDto(
            submission.Id,
            exam.Id,
            score,
            exam.TotalMarks,
            passed,
            submission.SubmittedAt
        ));
    }
}
