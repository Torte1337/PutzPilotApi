using System;
using PutzPilotApi.Enums;

namespace PutzPilotApi.Models;

public class WorkItem
{
    public Guid Id {get;set;}
    public string Title {get;set;}
    public WorkItemType Type {get;set;}
    public CleaningType CleaningType {get;set;}
    public DateTime Date {get;set;}
    public DateTime TimeFrom {get;set;}
    public DateTime TimoTo {get;set;}
    public string Note {get;set;}

    public Guid ObjectId {get;set;}
    public CleaningObject Object {get;set;}
    public Guid EmployeeId {get;set;}
}
