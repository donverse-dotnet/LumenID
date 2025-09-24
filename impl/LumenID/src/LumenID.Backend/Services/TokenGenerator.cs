using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace LumenID.Backend.Services;

public class TokenGenerator {
  private const string Issuer = "Donverse.net / LumenID";
  private const string Audience = "LumenID Clients";
  private const int ExpiresInHours = 24;
  private const string Algorithm = SecurityAlgorithms.HmacSha256;

  public JwtSecurityToken GenerateToken(string secretKey, IEnumerable<Claim> claims, DateTime issuedAt) {
    var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
    var securityKey = new SymmetricSecurityKey(secretKeyBytes);
    var credentials = new SigningCredentials(securityKey, Algorithm);
    var token = new JwtSecurityToken(
    issuer: Issuer,
    audience: Audience,
    claims: claims,
    notBefore: issuedAt,
    expires: issuedAt.AddHours(ExpiresInHours),
    signingCredentials: credentials
    );
    return token;
  }

  public string WriteToken(JwtSecurityToken token) {
    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
  }

  public bool ValidateToken(string token, string secretKey, out JwtSecurityToken? jwtToken) {
    jwtToken = null;
    var tokenHandler = new JwtSecurityTokenHandler();
    var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
    var securityKey = new SymmetricSecurityKey(secretKeyBytes);
    try {
      tokenHandler.ValidateToken(token, new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = Issuer,
        ValidateAudience = true,
        ValidAudience = Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      }, out var validatedToken);

      jwtToken = (JwtSecurityToken)validatedToken;
      return true;
    } catch {
      return false;
    }
  }
}
