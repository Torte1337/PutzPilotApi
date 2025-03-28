using System;
using PutzPilotApi.Enums;

namespace PutzPilotApi.RequestModels.CleaningObjectRequests;

public class CreateCleaningObjectRequest
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string Zipcode { get; set; }
    public string CleaningDays { get; set; }
    public string CleaningTimeSpan { get; set; }
    public CleaningType CleaningType { get; set; }
}
