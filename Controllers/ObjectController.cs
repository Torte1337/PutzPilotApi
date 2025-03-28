using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels.CleaningObjectRequests;

namespace PutzPilotApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObjectController : ControllerBase
{
    //TODO -> Objekte sollen ein Leistungsverzeichnis haben -> Aufgaben wie Staubwischen uvm.
    //TODO Wie viel Zeit hat man maximal bei einem objekt -> Thema erstellung eines Objektes
    private readonly PutzPilotDbContext context;

    public ObjectController(PutzPilotDbContext _ctx)
    {
        context = _ctx;
    }

    [HttpPost("create")]
    public async Task<IActionResult> OnCreateObject([FromBody] CreateCleaningObjectRequest request)
    {
        try
        {
            var exists = await context.CleaningObjects.AnyAsync(o =>
                        o.Name == request.Name &&
                        o.Location == request.Location &&
                        o.Street == request.Street &&
                        o.StreetNumber == request.StreetNumber &&
                        o.Zipcode == request.Zipcode &&
                        o.CleaningDays == request.CleaningDays &&
                        o.CleaningTimeSpan == request.CleaningTimeSpan &&
                        o.CleaningType == request.CleaningType);

            if(exists)
                return Conflict(new { Message = "Dieses Objekt mit diesen Werten gibt es bereits!"});
            
            CleaningObject newObject = new CleaningObject
            {
                Id = Guid.NewGuid(),
                CleaningDays = request.CleaningDays,
                CleaningTimeSpan = request.CleaningTimeSpan,
                CleaningType = request.CleaningType,
                Location = request.Location,
                Name = request.Name,
                Street = request.Street,
                StreetNumber = request.StreetNumber
            };

            context.CleaningObjects.Add(newObject);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Object erfolgreich angelegt"});

        }
        catch(Exception ex)
        {
            return StatusCode(500, new { Message = "Es gab beim erstellen des Objektes einen Fehler", Error = ex.Message});
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> OnDeleteObject(Guid id)
    {
        try
        {
            var obj = await context.CleaningObjects
                .Include(o => o.WorkItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj == null)
                return NotFound(new { Message = "Objekt nicht gefunden" });

            if (obj.WorkItems.Any())
                return BadRequest("Objekt kann nicht gelöscht werden, da noch WorkItems zugeordnet sind.");

            context.CleaningObjects.Remove(obj);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Objekt erfolgreich gelöscht" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Fehler beim Löschen des Objekts", Error = ex.Message });
        }
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> OnEditObject(Guid id, [FromBody] CreateCleaningObjectRequest request)
    {
        try
        {
            var obj = await context.CleaningObjects.FindAsync(id);

            if (obj == null)
                return NotFound(new { Message = "Objekt nicht gefunden" });

            var duplicate = await context.CleaningObjects.AnyAsync(o =>
                o.Id != id &&
                o.Location == request.Location &&
                o.Street == request.Street &&
                o.StreetNumber == request.StreetNumber &&
                o.Zipcode == request.Zipcode &&
                o.CleaningDays == request.CleaningDays &&
                o.CleaningTimeSpan == request.CleaningTimeSpan &&
                o.CleaningType == request.CleaningType);

            if (duplicate)
                return Conflict(new { Message = "Ein Objekt mit diesen Daten existiert bereits." });

            obj.Name = request.Name;
            obj.Location = request.Location;
            obj.Street = request.Street;
            obj.StreetNumber = request.StreetNumber;
            obj.Zipcode = request.Zipcode;
            obj.CleaningDays = request.CleaningDays;
            obj.CleaningTimeSpan = request.CleaningTimeSpan;
            obj.CleaningType = request.CleaningType;

            await context.SaveChangesAsync();

            return Ok(new { Message = "Objekt wurde erfolgreich aktualisiert" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Fehler beim Aktualisieren des Objekts", Error = ex.Message });
        }
    }

    [HttpGet("loadall")]
    public async Task<IActionResult> OnGetAllCleaningObjects()
    {
        try
        {
            var objects = await context.CleaningObjects
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Location,
                    o.Street,
                    o.StreetNumber,
                    o.Zipcode,
                    o.CleaningDays,
                    o.CleaningTimeSpan,
                    CleaningType = o.CleaningType.ToString(),
                    WorkItemCount = o.WorkItems.Count
                })
                .ToListAsync();

            return Ok(objects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Fehler beim Abrufen der Objekte", Error = ex.Message });
        }
    }

    [HttpGet("load{id}")]
    public async Task<IActionResult> OnGetCleaningObjectById(Guid id)
    {
        try
        {
            var obj = await context.CleaningObjects
                .Include(o => o.WorkItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj == null)
                return NotFound(new { Message = "Objekt nicht gefunden" });

            return Ok(obj);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Fehler beim Abrufen des Objekts", Error = ex.Message });
        }
    }

}
