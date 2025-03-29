using System;

namespace PutzPilotApi.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; }
    public int ExpireHours { get; set; }
}
