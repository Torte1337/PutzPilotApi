using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;
using PutzPilotApi.Manager;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels;
using PutzPilotApi.Enums;

namespace PutzPilotApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly PutzPilotDbContext context;
   
        public EmployeeController(PutzPilotDbContext _ctx)
        {
            context = _ctx;
        }

        [HttpPost("{employeeId}/createvacation")]
        public async Task<IActionResult> OnCreateVacationRequest(Guid employeeId, [FromBody] VacationRequestDto request)
        {
            var employee = await context.Employees.FindAsync(employeeId);

            if (employee == null)
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

            return Ok(new { Message = "Urlaubsanfrage gesendet." });
        }

        [HttpGet("{employeeId}/getvacation")]
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
    
        [HttpPost("create")]
        public async Task<IActionResult> OnCreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                // Prüfe, ob der Username bereits existiert
                if (await context.Employees.AnyAsync(e => e.Username == request.Username))
                {
                    return BadRequest("Username bereits vergeben.");
                }

                // Generiere Salt und Hash für das Passwort
                var salt = PasswordManager.OnGenerateSalt();
                var passwordHash = PasswordManager.OnHashPassword(request.Password, salt);

                // Erstelle das Mitarbeiterobjekt
                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = salt,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    Surname = request.Surname,
                    IsActive = request.IsActive,
                    Role = request.Role
                };

                // Füge das Mitarbeiterobjekt zum DbContext hinzu
                context.Employees.Add(employee);

                // Speichere die Änderungen in der Datenbank
                await context.SaveChangesAsync();

                // Erfolgreiche Antwort zurückgeben
                return Ok(new { Message = "Mitarbeiter erfolgreich erstellt", EmployeeId = employee.Id });
            }
            catch (Exception ex)
            {
                // Fehler loggen
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Es gab einen Fehler beim Erstellen des Mitarbeiters", Error = ex.Message });
            }
        }

        [HttpDelete("delete/{employeeId}")]
        public async Task<IActionResult> OnDeleteEmployee(Guid employeeId)
        {
            try
            {
                // Finde den Mitarbeiter in der Datenbank
                var employee = await context.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    return NotFound("Mitarbeiter nicht gefunden.");
                }

                // Entferne den Mitarbeiter aus der Datenbank
                context.Employees.Remove(employee);
                await context.SaveChangesAsync();

                // Erfolgreiche Antwort zurückgeben
                return Ok(new { Message = "Mitarbeiter erfolgreich gelöscht." });
            }
            catch (Exception ex)
            {
                // Fehler loggen
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Es gab einen Fehler beim Löschen des Mitarbeiters", Error = ex.Message });
            }
        }    
    
        [HttpPut("update/{employeeId}")]
        public async Task<IActionResult> OnUpdateEmployee(Guid employeeId, [FromBody] CreateEmployeeRequest request)
        {
            try
            {
                // Finde den Mitarbeiter in der Datenbank
                var employee = await context.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    return NotFound("Mitarbeiter nicht gefunden.");
                }

                // Aktualisiere die Mitarbeiterdaten
                employee.Username = request.Username;
                employee.Firstname = request.Firstname;
                employee.Lastname = request.Lastname;
                employee.Surname = request.Surname;

                // Speichere die Änderungen in der Datenbank
                await context.SaveChangesAsync();

                // Erfolgreiche Antwort zurückgeben
                return Ok(new { Message = "Mitarbeiter erfolgreich aktualisiert." });
            }
            catch (Exception ex)
            {
                // Fehler loggen
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Es gab einen Fehler beim Aktualisieren des Mitarbeiters", Error = ex.Message });
            }
        }
    
        [HttpPut("deactivate/{employeeId}")]
        public async Task<IActionResult> OnDeactivateEmployee(Guid employeeId)
        {
            try
            {
                // Finde den Mitarbeiter in der Datenbank
                var employee = await context.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    return NotFound("Mitarbeiter nicht gefunden.");
                }

                // Setze das IsActive-Flag auf false (Sperren)
                employee.IsActive = false;

                // Speichere die Änderungen in der Datenbank
                await context.SaveChangesAsync();

                // Erfolgreiche Antwort zurückgeben
                return Ok(new { Message = "Mitarbeiter erfolgreich deaktiviert." });
            }
            catch (Exception ex)
            {
                // Fehler loggen
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Es gab einen Fehler beim Deaktivieren des Mitarbeiters", Error = ex.Message });
            }
        }
    
    
    
    
    }
}
