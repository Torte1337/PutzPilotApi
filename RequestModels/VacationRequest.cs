using System;
using PutzPilotApi.Enums;
using PutzPilotApi.Models;

namespace PutzPilotApi.RequestModels;

public class VacationRequest
{
    public Guid Id {get;set;}
    public Guid EmployeeId {get;set;}
    public Employee Employee {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public VacationStatus Status {get;set;} = VacationStatus.Requested;
    public string? Note {get;set;}
}
