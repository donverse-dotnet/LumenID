using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

using LumenID.Backend.Contexts.Accounts;
using LumenID.Backend.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LumenID.Backend.Handlers;

public class UsersAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    [FromServices] TokenGenerator tokens,
    [FromServices] AccountsDbContext accounts
) : AuthenticationHandler<AuthenticationSchemeOptions>(
    options,
    loggerFactory,
    encoder
) {
  private ILogger<UsersAuthenticationHandler> logger = loggerFactory.CreateLogger<UsersAuthenticationHandler>();

  private const string HeaderTokenKey = "token";
  private const string HeaderUserIdKey = "user-id";
  private const string HeaderSessionIdKey = "session-id";

  private const string TokenNotFoundMessage = "Token not found in metadata.";
  private const string UserIdNotFoundMessage = "User ID not found in metadata.";
  private const string SessionIdNotFoundMessage = "Session ID not found in metadata.";
  private const string UserNotFoundMessage = "User not found in database.";
  private const string SessionExpiredMessage = "Session expired.";
  private const string InvalidTokenMessage = "Invalid token.";

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
    // Started notice on server.
    logger.LogInformation("Handling authentication for {id} : {path} (from {ip})", Request.HttpContext.Connection.Id, Request.Path, Request.HttpContext.Connection.RemoteIpAddress);

    // Get data from header
    if (Request.Headers.TryGetValue(HeaderTokenKey, out var token) is false) {
      logger.LogWarning("{Request} is not include token.", Request.HttpContext.Connection.Id);
      return AuthenticateResult.Fail(TokenNotFoundMessage);
    }
    if (Request.Headers.TryGetValue(HeaderUserIdKey, out var userId) is false) {
      logger.LogWarning("{Request} is not include user id.", Request.HttpContext.Connection.Id);
      return AuthenticateResult.Fail(UserIdNotFoundMessage);
    }
    if (Request.Headers.TryGetValue(HeaderSessionIdKey, out var sessionId) is false) {
      logger.LogWarning("{Request} is not include session id.", Request.HttpContext.Connection.Id);
      return AuthenticateResult.Fail(SessionIdNotFoundMessage);
    }

    // Check header data has data
    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId)) {
      logger.LogWarning("{Request} is not include token or user id.", Request.HttpContext.Connection.Id);
      return AuthenticateResult.Fail(TokenNotFoundMessage);
    }

    // Get user secret
    var userMetadata = await accounts.GetMetadataAsync(userId);
    if (userMetadata is null) return AuthenticateResult.Fail(UserNotFoundMessage);

    var userSecret = await accounts.GetSecretAsync(userMetadata.SecretId);
    if (userSecret is null) return AuthenticateResult.Fail(UserNotFoundMessage);

    // Validate token
    var validatedToken = tokens.ValidateToken(token, userSecret.SecretKey, out var jwt);
    if (validatedToken is false) return AuthenticateResult.Fail(SessionExpiredMessage);
    if (jwt is null) return AuthenticateResult.Fail(InvalidTokenMessage);

    // Create ticket
    var identity = new ClaimsIdentity(jwt.Claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    var uid = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
    logger.LogInformation("Handled authentication for {id} : {path} (from {ip} with {uid}", Request.HttpContext.Connection.Id, Request.Path, Request.HttpContext.Connection.RemoteIpAddress, uid);

    // Return authenticate result
    return AuthenticateResult.Success(ticket);
  }
}
