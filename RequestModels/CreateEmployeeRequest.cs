using System;

namespace PutzPilotApi.RequestModels;

public class CreateEmployeeRequest
{
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
}
