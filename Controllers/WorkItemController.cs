using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;
using PutzPilotApi.Models;

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

    [HttpPost]
    public async Task<IActionResult> OnCreateWorkItem([FromBody] WorkItem workItem)
    {
        var relatedObjecct = await context.CleaningObjects.FirstOrDefaultAsync(x => x.Id == workItem.ObjectId);

        if(relatedObjecct == null)
            return BadRequest("Object nicht gefunden");

        context.Workitems.Add(workItem);
        await context.SaveChangesAsync();

        return Ok(new { Message = "Gespeichert", WorkItemId = workItem.Id});
    }
}
