using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;

namespace PutzPilotApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly PutzPilotDbContext context;

    public SetupController(PutzPilotDbContext _ctx)
    {
        context = _ctx;
    }

    [HttpGet("migrate")]
    public IActionResult OnMigrateDatabase()
    {
        try
        {
            context.Database.Migrate();

            return Ok("Migration erfolgreich ausgeführt");
        }
        catch(Exception ex)
        {
            return StatusCode(500,$"Migration fehlgeschlagen: {ex.Message}");
        }
    }
    [HttpGet("dbCheck")]
    public async Task<IActionResult> OnCheckDatabaseIsOnline()
    {
        try
        {
            await context.Database.CanConnectAsync();
            return Ok("Datenbank ist online und erreichbar");
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { Message = "Fehler beim prüfen der Datenbankverbindung", Error = ex.Message});
        }
    }

    [HttpGet("check")]
    public async Task<IActionResult> OnCheckDatabase()
    {
        try
        {
            using(SqlConnection sqlCon = new SqlConnection("Persist Security Info=False;server=db1045676903.hosting-data.io;Initial Catalog=db1045676903; User ID=dbo1045676903; Password=Test1234567#"))
            {
                await sqlCon.OpenAsync(); 

                using (SqlCommand cmd = new SqlCommand("SELECT 1", sqlCon))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        // Erfolgreiche Verbindung
                        return Ok("Verbindung war erfolgreich.");
                    }
                    else
                    {
                        return StatusCode(500, "Fehler bei der Verbindung: Abfrage fehlgeschlagen.");
                    }
                }
            }
        }
        catch(Exception ex)
        {
            return StatusCode(500,$"Fehler beim Verbinden mit der Datenbank -> {ex.Message}");
        }
    }
}
