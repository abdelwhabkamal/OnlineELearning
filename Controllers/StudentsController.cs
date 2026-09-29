using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OELearning.Api.Data;
using OELearning.Api.DTOs.Students;
using OELearning.Api.Models.Entities;
using OELearning.Api.Models.Enums;
using OELearning.Api.Services.Implementations;

namespace OELearning.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IQrCodeService _qrCodeService;

    public StudentsController(AppDbContext context, IQrCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentListDto>>> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] int? groupId = null,
        [FromQuery] StudentStatus? status = null,
        [FromQuery] bool? isTopStudent = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.StudentProfiles
            .Include(s => s.User)
            .Include(s => s.Group)
                .ThenInclude(g => g!.Center)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        if (groupId.HasValue)
        {
            query = query.Where(s => s.GroupId == groupId.Value);
        }

        if (isTopStudent.HasValue)
        {
            query = query.Where(s => s.IsTopStudent == isTopStudent.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(sp =>
                sp.User.FullName.Contains(s) ||
                sp.User.PhoneNumber.Contains(s) ||
                sp.ParentPhoneNumber.Contains(s) ||
                sp.OELearningCode.Contains(s)
            );
        }

        var total = await query.CountAsync();
        var students = await query
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = students.Select((s, index) =>
        {
            var studentCleanPhone = s.User.PhoneNumber.Replace("+", "").Trim();
            var parentCleanPhone = s.ParentPhoneNumber.Replace("+", "").Trim();
            
            // Generate standard WhatsApp international click-to-chat links
            var waStudent = studentCleanPhone.StartsWith("0") ? $"2{studentCleanPhone}" : studentCleanPhone;
            var waParent = parentCleanPhone.StartsWith("0") ? $"2{parentCleanPhone}" : parentCleanPhone;

            return new StudentListDto(
                s.Id,
                s.UserId,
                (page - 1) * pageSize + index + 1,
                s.User.FullName,
                s.OELearningCode,
                s.Group?.Name ?? "??? ????",
                s.Group?.Center.Name ?? "??? ????",
                s.User.PhoneNumber,
                s.ParentPhoneNumber,
                $"https://wa.me/{waStudent}",
                $"https://wa.me/{waParent}",
                s.Status.ToString(),
                s.IsTopStudent,
                s.QrCodePayload != null ? _qrCodeService.GenerateBase64QrCode(s.QrCodePayload) : null
            );
        });

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentListDto>> GetStudentById(int id)
    {
        var s = await _context.StudentProfiles
            .Include(sp => sp.User)
            .Include(sp => sp.Group)
                .ThenInclude(g => g!.Center)
            .FirstOrDefaultAsync(sp => sp.Id == id);

        if (s == null) return NotFound();

        var studentCleanPhone = s.User.PhoneNumber.Replace("+", "").Trim();
        var parentCleanPhone = s.ParentPhoneNumber.Replace("+", "").Trim();
        var waStudent = studentCleanPhone.StartsWith("0") ? $"2{studentCleanPhone}" : studentCleanPhone;
        var waParent = parentCleanPhone.StartsWith("0") ? $"2{parentCleanPhone}" : parentCleanPhone;

        var dto = new StudentListDto(
            s.Id,
            s.UserId,
            1,
            s.User.FullName,
            s.OELearningCode,
            s.Group?.Name ?? "??? ????",
            s.Group?.Center.Name ?? "??? ????",
            s.User.PhoneNumber,
            s.ParentPhoneNumber,
            $"https://wa.me/{waStudent}",
            $"https://wa.me/{waParent}",
            s.Status.ToString(),
            s.IsTopStudent,
            s.QrCodePayload != null ? _qrCodeService.GenerateBase64QrCode(s.QrCodePayload) : null
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
        {
            return BadRequest(new { message = "??? ???? ?????? ???? ??????" });
        }

        var user = new User
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Email = $"{dto.PhoneNumber}@student.OELearning.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Student
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var code = Guid.NewGuid().ToString("N")[..5].ToUpper();
        var profile = new StudentProfile
        {
            UserId = user.Id,
            OELearningCode = code,
            ParentPhoneNumber = dto.ParentPhoneNumber,
            GroupId = dto.GroupId,
            Status = StudentStatus.Approved,
            QrCodePayload = $"OELearning-STU-{code}-{user.PhoneNumber}"
        };

        _context.StudentProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudentById), new { id = profile.Id }, profile);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult> UpdateStudentStatus(int id, [FromBody] UpdateStudentStatusDto dto)
    {
        var profile = await _context.StudentProfiles.FindAsync(id);
        if (profile == null) return NotFound();

        profile.Status = dto.Status;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/reset-password")]
    public async Task<ActionResult> ResetPassword(int id, [FromBody] string newPassword)
    {
        var profile = await _context.StudentProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile == null) return NotFound();

        profile.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully." });
    }

    [HttpPost("bulk-import")]
    public async Task<ActionResult> BulkImport([FromBody] List<BulkImportStudentItem> items)
    {
        var createdCount = 0;
        foreach (var item in items)
        {
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == item.PhoneNumber))
            {
                continue;
            }

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == item.GroupName) 
                ?? await _context.Groups.FirstOrDefaultAsync();

            var user = new User
            {
                FullName = item.FullName,
                PhoneNumber = item.PhoneNumber,
                Email = $"{item.PhoneNumber}@student.OELearning.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var code = Guid.NewGuid().ToString("N")[..5].ToUpper();
            var profile = new StudentProfile
            {
                UserId = user.Id,
                OELearningCode = code,
                ParentPhoneNumber = item.ParentPhoneNumber,
                GroupId = group?.Id,
                Status = StudentStatus.Approved,
                QrCodePayload = $"OELearning-STU-{code}-{user.PhoneNumber}"
            };

            _context.StudentProfiles.Add(profile);
            createdCount++;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"Successfully imported {createdCount} students." });
    }
}
