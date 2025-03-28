using System;
using PutzPilotApi.Enums;

namespace PutzPilotApi.Models;

public class CleaningObject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string Zipcode { get; set; }
    public string CleaningDays { get; set; } // z. B. "Mo,Mi,Fr"
    public string CleaningTimeSpan { get; set; } // z. B. "01:30"
    public CleaningType CleaningType { get; set; }
    public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
}
