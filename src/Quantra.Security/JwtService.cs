using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Quantra.Security;

public class JwtService
{
    private readonly string _secret;
    private readonly TimeSpan _lifetime;

    public JwtService(string secret, TimeSpan lifetime)
    {
        _secret   = secret;
        _lifetime = lifetime;
    }

    public string GenerateToken(Guid userId, string username, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:  "quantra",
            audience:"quantra",
            claims:  claims,
            expires: DateTime.UtcNow.Add(_lifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void ConfigureAuthentication(IServiceCollection services)
    {
        var key = Encoding.UTF8.GetBytes(_secret);

        services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidIssuer              = "quantra",
                        ValidateAudience         = true,
                        ValidAudience            = "quantra",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(key)
                    };
                });
    }
}