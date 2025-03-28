using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels;
using PutzPilotApi.RequestModels.WorkItemRequests;

namespace PutzPilotApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkItemController : ControllerBase
{
    private readonly PutzPilotDbContext context;

    public WorkItemController(PutzPilotDbContext _ctx)
    {
        context = _ctx;
    }

    [HttpPost("user/{userId}/workitems")]
    public async Task<IActionResult> OnGetWorkitemsForDate(Guid userId, [FromBody] WorkItemDateRequest request)
    {
        try
        {
            var userExist = await context.Employees.AnyAsync(x => x.Id == userId);

            if(!userExist)
                return BadRequest("Benutzer konnte nicht gefunden werden");

            var startDate = request.Date.Date;
            var endDate = startDate.AddDays(1);

            var workItems = await context.Workitems.Where(x => x.EmployeeId == userId && x.Date >= startDate && x.Date < endDate).ToListAsync();

            if(workItems.Count <= 0)
                return NotFound("Es wurden keine Arbeitszeiten für diesen Tag gefunden");

            return Ok(workItems);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { Message = "Es gab einen Fehler beim Abrufen der Arbeitszeiten!", Error = ex.Message});
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> OnCreateWorkItem([FromBody] CreateWorkitemRequest request)
    {
        try
        {
            var relatedObject = await context.CleaningObjects.FirstOrDefaultAsync(x => x.Id == request.ObjectId);

            if(relatedObject == null)
                return BadRequest("Das Objekt wurde nicht gefunden");

            var workitem = new WorkItem
            {
                Id = Guid.NewGuid(),
                ObjectId = request.ObjectId,
                EmployeeId = request.EmployeeId,
                Date = request.Date,
                CleaningType = request.CleaningType,
                Type = request.Type,
                Note = request.Note,
                TimeFrom = request.TimeFrom,
                TimeTo = request.TimeTo,
                Title = request.Title
            };

            context.Workitems.Add(workitem);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Arbeitszeit erfolgreich hinzugefügt"});
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { Message = "Es gab einen Fehler beim Erstellen des Workitems", Error = ex.Message });
        }
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> OnUpdateWorkItem(Guid id, [FromBody] UpdateWorkItemRequest request)
    {
        try
        {
            var existingWorkItem = await context.Workitems.FirstOrDefaultAsync(x => x.Id == id);
            if(existingWorkItem == null)
                return NotFound("Arbeitszeit wurde nicht gefunden");

            existingWorkItem.Title = request.Title;
            existingWorkItem.TimeFrom = request.TimeFrom;
            existingWorkItem.TimeTo = request.TimeTo;
            existingWorkItem.Date = request.Date;
            existingWorkItem.Note = request.Note;
            existingWorkItem.CleaningType = request.CleaningType;
            existingWorkItem.Type = request.Type;
            existingWorkItem.ObjectId = request.ObjectId;


            await context.SaveChangesAsync();

            return Ok(new { Message = "Arbeitszeit erfolgreich aktualisiert"});
        }
        catch(Exception ex)
        {
            return StatusCode(500,new { Message = "Es gab einen Fehler beim bearbeiten der Arbeitszeit", Error = ex.Message});
        }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> OnDeleteWorkItem(Guid workItemid, Guid employeeId)
    {
        try
        {
            var toDeleteWorkItem = await context.Workitems.FirstOrDefaultAsync(x => x.Id == workItemid);
            if(toDeleteWorkItem == null)
                return NotFound("Arbeitszeit wurde nicht gefunden");
            
            var user = await context.Employees.FirstOrDefaultAsync(x => x.Id == employeeId);
            if(user == null)
                return NotFound("Arbeitszeit kann aus Berechtigungsgründen nicht gelöscht werden");
            
            if(user.Id == toDeleteWorkItem.EmployeeId || user.Role == "Admin" || user.Role == "Superadmin" || user.Role == "Personalabteilung")
            {
                context.Workitems.Remove(toDeleteWorkItem);
                await context.SaveChangesAsync();

                return Ok(new { Message = "Arbeitszeit wurde erfolgreich gelöscht"});
            }
            else
            {
                return NotFound("Arbeitszeit kann aus Berechtigungsgründen nicht gelöscht werden");
            }
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { Message = "Es gab einen Fehler beim Löschen der Arbeitszeit", Error = ex.Message});
        }
    }

}