using System;

namespace PutzPilotApi.Models;

public class VacationRequestDto
{
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public string? Note {get;set;}
}
