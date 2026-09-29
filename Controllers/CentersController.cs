using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OELearning.Api.Data;
using OELearning.Api.DTOs.Centers;
using OELearning.Api.Models.Entities;

namespace OELearning.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CentersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CentersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CenterDto>>> GetCenters()
    {
        var centers = await _context.Centers
            .Include(c => c.Groups)
                .ThenInclude(g => g.Students)
            .Select(c => new CenterDto(
                c.Id,
                c.Name,
                c.Address,
                c.ContactPhone,
                c.IsOnline,
                c.Groups.Count,
                c.Groups.SelectMany(g => g.Students).Count()
            ))
            .ToListAsync();

        return Ok(centers);
    }

    [HttpGet("schedules")]
    public async Task<ActionResult<IEnumerable<CenterScheduleDto>>> GetCenterSchedules()
    {
        var schedules = await _context.CenterSchedules
            .Include(cs => cs.Center)
            .OrderBy(cs => cs.SessionTime)
            .Select(cs => new CenterScheduleDto(
                cs.Id,
                cs.CenterId,
                cs.Center.Name,
                cs.LectureTitle,
                cs.SessionTime,
                cs.Classroom
            ))
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpPost]
    public async Task<ActionResult<Center>> CreateCenter([FromBody] CreateCenterDto dto)
    {
        var center = new Center
        {
            Name = dto.Name,
            Address = dto.Address,
            ContactPhone = dto.ContactPhone,
            IsOnline = dto.IsOnline
        };

        _context.Centers.Add(center);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCenters), new { id = center.Id }, center);
    }

    [HttpGet("groups")]
    public async Task<ActionResult> GetGroups()
    {
        var groups = await _context.Groups
            .Include(g => g.Center)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.CenterId,
                CenterName = g.Center.Name,
                StudentsCount = g.Students.Count
            })
            .ToListAsync();

        return Ok(groups);
    }
}
