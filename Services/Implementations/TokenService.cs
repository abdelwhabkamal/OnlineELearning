using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OELearning.Api.Models.Entities;

namespace OELearning.Api.Services.Implementations;

public interface ITokenService
{
    string GenerateJwtToken(User user, int? studentProfileId = null);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateJwtToken(User user, int? studentProfileId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "OELearningSuperSecretKeyLongEnoughForHmacSha256Security12345!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (studentProfileId.HasValue)
        {
            claims.Add(new Claim("StudentProfileId", studentProfileId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "OELearningApi",
            audience: _config["Jwt:Audience"] ?? "OELearningWebClient",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
