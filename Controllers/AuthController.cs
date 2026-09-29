using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsqueraLms.Api.Data;
using AsqueraLms.Api.DTOs.Auth;
using AsqueraLms.Api.Models.Entities;
using AsqueraLms.Api.Models.Enums;
using AsqueraLms.Api.Services.Implementations;

namespace AsqueraLms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid phone number or password." });
        }

        if (!user.IsActive)
        {
            return Forbid("User account is inactive or suspended.");
        }

        var token = _tokenService.GenerateJwtToken(user, user.StudentProfile?.Id);

        return Ok(new AuthResponseDto(
            token,
            user.FullName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.Id,
            user.StudentProfile?.Id
        ));
    }

    [HttpPost("register-student")]
    public async Task<ActionResult<AuthResponseDto>> RegisterStudent([FromBody] RegisterStudentDto request)
    {
        if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
        {
            return BadRequest(new { message = "Phone number is already registered." });
        }

        var group = await _context.Groups.FindAsync(request.GroupId);
        if (group == null)
        {
            return BadRequest(new { message = "Invalid group specified." });
        }

        var user = new User
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = $"{request.PhoneNumber}@student.asquera.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var randomCode = Guid.NewGuid().ToString("N")[..5].ToUpper();
        var profile = new StudentProfile
        {
            UserId = user.Id,
            AsqueraCode = randomCode,
            ParentPhoneNumber = request.ParentPhoneNumber,
            GroupId = request.GroupId,
            Status = StudentStatus.Pending,
            QrCodePayload = $"ASQUERA-STU-{randomCode}-{user.PhoneNumber}"
        };

        _context.StudentProfiles.Add(profile);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateJwtToken(user, profile.Id);

        return Ok(new AuthResponseDto(
            token,
            user.FullName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.Id,
            profile.Id
        ));
    }
}
