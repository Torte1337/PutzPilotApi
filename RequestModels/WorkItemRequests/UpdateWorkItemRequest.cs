using PutzPilotApi.Enums;
using PutzPilotApi.Models;

namespace PutzPilotApi.RequestModels.WorkItemRequests;

public class UpdateWorkItemRequest
{
    public string Title {get;set;}
    public WorkItemType Type {get;set;}
    public CleaningType CleaningType {get;set;}
    public DateTime Date {get;set;}
    public DateTime TimeFrom {get;set;}
    public DateTime TimeTo {get;set;}
    public string Note {get;set;}

    public Guid ObjectId {get;set;}
    public CleaningObject? Object {get;set;}
}
