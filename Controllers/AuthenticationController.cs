using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PutzPilotApi.Context;
using PutzPilotApi.Manager;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels;
using PutzPilotApi.Settings;

namespace PutzPilotApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly PutzPilotDbContext context;
    private readonly JwtSettings jwtsettings;
    public AuthenticationController(PutzPilotDbContext _ctx, IOptions<JwtSettings> jwts)
    {
        context = _ctx;
        jwtsettings = jwts.Value;
    }


    [HttpPost("login")]
    public async Task<IActionResult> OnLogin([FromBody] LoginRequest request)
    {
        var user = await context.Employees.FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user == null)
            return Unauthorized("Benutzer nicht gefunden");

        // Verifiziere das Passwort
        bool valid = PasswordManager.OnVerifyPassword(request.Password, user.PasswordSalt, user.PasswordHash);

        if (!valid)
            return Unauthorized("Benutzername oder Passwort ist falsch");

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            Message = "Login erfolgreich",
            EmployeeId = user.Id,
            Name = $"{user.Firstname} {user.Surname}",
            Token = token
        });
    }
    [HttpPost("logout")]
    public IActionResult OnLogout()
    {
        // Hier kann man bei Bedarf Session-Management hinzufügen, aber für die API reicht es so.
        return Ok(new { Message = "Logout erfolgreich" });
    }

    private string GenerateJwtToken(Employee user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Benutzer-ID als Subject
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Unique Identifier für das Token
            new Claim("username", user.Username),
            new Claim("firstname", user.Firstname),
            new Claim("surname", user.Surname)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "PutzPilotApi",
            audience: "PutzPilotUsers",
            claims: claims,
            expires: DateTime.Now.AddHours(jwtsettings.ExpireHours),  // Token läuft nach 1 Stunde ab
            signingCredentials: credentials
        );

        //Token wird zurückgegeben
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
