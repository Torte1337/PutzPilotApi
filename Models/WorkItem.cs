using System;
using PutzPilotApi.Enums;

namespace PutzPilotApi.Models;

public class WorkItem
{
    public required Guid Id {get;set;}
    public string? Title {get;set;}
    public required WorkItemType Type {get;set;}
    public CleaningType? CleaningType {get;set;}
    public required DateTime Date {get;set;}
    public DateTime? TimeFrom {get;set;}
    public DateTime? TimeTo {get;set;}
    public string? Note {get;set;}

    public Guid? ObjectId {get;set;}
    public required Guid EmployeeId {get;set;}
}
