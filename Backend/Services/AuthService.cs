using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GenerateToken(int userId, string firstName, string lastName, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, $"{firstName} {lastName}"),
            new Claim(ClaimTypes.GivenName, firstName),
            new Claim(ClaimTypes.Surname, lastName),
            new Claim(ClaimTypes.Role, role)
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SoftwareTrackerAPI",
            audience: _configuration["Jwt:Audience"] ?? "SoftwareTrackerClient",
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public bool IsMentor(string firstName, string lastName)
    {
        var mentors = new List<(string, string)>
        {
            ("Cihangir", "Yaman"),
            ("İbrahim", "Kabadayı"),
            ("Eren", "Vural")
        };
        
        return mentors.Any(m => 
            string.Equals(m.Item1, firstName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(m.Item2, lastName, StringComparison.OrdinalIgnoreCase));
    }
}
