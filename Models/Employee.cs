using System;
using PutzPilotApi.RequestModels;

namespace PutzPilotApi.Models;

public class Employee
{
    public Guid Id {get;set;}
    public string Username {get;set;}
    public string PasswordHash {get;set;}
    public string PasswordSalt {get;set;}
    public string Firstname {get;set;}
    public string? Lastname {get;set;}
    public string Surname {get;set;}
    public ICollection<VacationRequest> VacationRequests {get;set;} 
}
