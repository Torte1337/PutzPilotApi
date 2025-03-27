using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;
using PutzPilotApi.Manager;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels;
using PutzPilotApi.Enums;

namespace PutzPilotApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class EmployeeController : ControllerBase
{
    private readonly PutzPilotDbContext context;

    public EmployeeController(PutzPilotDbContext _ctx)
    {
        context = _ctx;
    }

    [HttpGet("login")]
    public async Task<IActionResult> OnLogin([FromBody] LoginRequest request)
    {
        var user = await context.Employees.FirstOrDefaultAsync(x => x.Username == request.Username);

        if(user == null)
            return Unauthorized("Benutzer nicht gefunden");
        
        bool valid = PasswordManager.OnVerifyPassword(request.Password,user.PasswordSalt,user.PasswordHash);

        if(!valid)
            return Unauthorized("Benutzername oder Passwort ist falsch");
        
        return Ok(new
        {
            Message = "Login erfolgreich",
            EmployeeId = user.Id,
            Name = $"{user.Firstname} {user.Surname}"
        });
    }
    
    [HttpPost("logout")]
    public IActionResult OnLogout()
    {
        return Ok(new { Message = "Logout erfolgreich"});
    }

    [HttpPost("{employeeId}/vacation")]
    public async Task<IActionResult> OnCreateVacationRequest(Guid employeeId, [FromBody] VacationRequestDto request)
    {
        var employee = await context.Employees.FindAsync(employeeId);

        if(employee == null)
            return NotFound("Mitarbeiter nicht gefunden");

        var vacation = new VacationRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Note = request.Note,
            Status = VacationStatus.Requested
        };

        context.VacationRequests.Add(vacation);
        await context.SaveChangesAsync();

        return Ok(new { Message = "Urlaubsanfrage gesendet."});
    }

    [HttpGet("{employeeId}/vacation")]
    public async Task<IActionResult> OnGetVacations(Guid employeeId)
    {
        var vacations = await context.VacationRequests
                        .Where(v => v.EmployeeId == employeeId)
                        .OrderBy(v => v.StartDate)
                        .ToListAsync();
                    
        return Ok(vacations);
    }

    [HttpGet("{employeeId}/vacation/summary")]
    public async Task<IActionResult> OnGetVacationSummary(Guid employeeId)
    {
        const int annualAllowance = 30;

        var approved = await context.VacationRequests
            .Where(v => v.EmployeeId == employeeId && v.Status == VacationStatus.Approved)
            .ToListAsync();

        int usedDays = approved.Sum(v => (v.EndDate - v.StartDate).Days + 1);
        int remaining = annualAllowance - usedDays;

        var planned = await context.VacationRequests
            .Where(v => v.EmployeeId == employeeId && v.Status == VacationStatus.Requested)
            .ToListAsync();

        int plannedDays = planned.Sum(v => (v.EndDate - v.StartDate).Days + 1);

        return Ok(new
        {
            Annual = annualAllowance,
            Used = usedDays,
            Planned = plannedDays,
            Available = remaining
        });
    }
}
