using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Domain.Entities;
using System.Security.Claims;
using System.Text;

namespace QuestLog.Infrastructure.Authentication;

public class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public AccessToken CreateToken(AppUser user, IList<string> roles)
    {
        // 1) Token'ın içine yazılacak bilgiler (claim'ler)
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.DisplayName ?? user.Email!)
        };
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        // 2) İmza: gizli anahtar + algoritma
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3) Son kullanma zamanı
        var expiresAt = TimeProvider.System.GetUtcNow().AddMinutes(_options.ExpiryMinutes);

        // 4) Token'ı oluştur
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = credentials
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessToken(token, expiresAt);
    }

}