using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using LumenID.Backend.Contexts.Accounts;
using LumenID.Backend.Contexts.Clients;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Services;

public class AuthenticateServiceImpl(
    [FromServices] TokenGenerator tokenGenerator,
    [FromServices] AccountsDbContext accounts,
    [FromServices] OAuthClientsDbContext clients,
    [FromServices] ILogger<RegisterServiceImpl> logger
) : AuthenticateService.AuthenticateServiceBase {
  public override async Task<AuthenticateResponse> Authenticate(AuthAccountModel request, ServerCallContext context) {
    // 1. Find user (verify) with email and password
    // 1.1 Get info using email
    var info = await accounts.Infos
        .Select(item => new { item.Id, item.Email })
        .Where(item => item.Email == request.Email)
        .FirstOrDefaultAsync();
    if (info is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));

    // 1.3 Get metadata using user id and secret id
    var metadata = await accounts.Metadata
        .Select(item => new { item.Id, item.SecretId, item.InfoId, item.CreatedAt, item.UpdatedAt })
        .Where(item => item.InfoId == info.Id)
        .FirstOrDefaultAsync();

    if (metadata is null) throw new RpcException(new Status(StatusCode.NotFound, "Account metadata not found"));

    // 1.2 Get secret using user id
    var secret = await accounts.Secrets
        .Select(item => new { item.Id, item.Password, item.SecretKey })
        .Where(item => item.Id == metadata.SecretId)
        .FirstOrDefaultAsync();

    if (secret is null) throw new RpcException(new Status(StatusCode.NotFound, "Account secret not found"));

    // 1.4 Verify password with secret data
    if (secret.Password != request.Password)
      throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid password"));

    // 2. If found (verified), generate token (JWT)
    // 2.1 Create claims
    var issuedAt = DateTime.UtcNow;
    var claims = new List<Claim>
    {
            new Claim(JwtRegisteredClaimNames.Sub, metadata.Id),
            new Claim(JwtRegisteredClaimNames.Email, info.Email)
        };
    // 2.2 Create token with claims(Sign token with secret key (secret key from 1.2))
    var token = tokenGenerator.GenerateToken(secret.SecretKey, claims, issuedAt);

    // 3. Return token and user info in AuthenticateResponse
    // 3.1 Create new session data
    var newSession = accounts.CreateNewSession(metadata.Id, tokenGenerator.WriteToken(token), issuedAt,
    token.ValidTo);
    // 3.2 Create AuthenticateResponse
    var response = new AuthenticateResponse {
      SessionId = newSession.Id,
      UserId = newSession.MetaId,
      Token = newSession.Token,
      ExpiresAt = Timestamp.FromDateTime(newSession.ExpiresAt.ToUniversalTime()),
    };
    // 3.3 Save response data to database (sessions table)
    await accounts.SaveSessionsAsync(newSession);
    // 3.4 Return response
    return response;
  }

  public override async Task<AuthenticateResponse> GetToken(GetTokenRequest request, ServerCallContext context) {
    // 1. Get granted code from database.
    var grantCode = await clients.GetCodeByCodeAsync(request.Code);
    if (grantCode is null) throw new RpcException(new Status(StatusCode.NotFound, "Code not found"));

    // 2. Verify user is granted.
    var userMeta = await accounts.GetMetadataAsync(grantCode.UserId);
    if (userMeta is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));

    var userConf = await accounts.GetConfigAsync(userMeta.ConfigId);
    if (userConf is null) throw new RpcException(new Status(StatusCode.NotFound, "Account config not found"));

    var grantedApps = userConf.GetGrantedApps();
    if ((request.AppId == grantCode.AppId && grantedApps.AppIds.Contains(grantCode.AppId)) is false) throw new RpcException(new Status(StatusCode.NotFound, "User does not have grant app or grant code is invalid."));

    // 2.1 Remove code data.
    clients.Codes.Remove(grantCode);
    await clients.SaveChangesAsync();

    // 3. Generate session. (bring from authenticate)
    // 1. Find user (verify) with email and password
    // 1.1 Get info using email
    var info = await accounts.Infos
        .Select(item => new { item.Id, item.Email })
        .Where(item => item.Id == userMeta.Id)
        .FirstOrDefaultAsync();
    if (info is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));

    // 1.3 Get metadata using user id and secret id
    var metadata = await accounts.Metadata
        .Select(item => new { item.Id, item.SecretId, item.InfoId, item.CreatedAt, item.UpdatedAt })
        .Where(item => item.InfoId == info.Id)
        .FirstOrDefaultAsync();

    if (metadata is null) throw new RpcException(new Status(StatusCode.NotFound, "Account metadata not found"));

    // 1.2 Get secret using user id
    var secret = await accounts.Secrets
        .Select(item => new { item.Id, item.Password, item.SecretKey })
        .Where(item => item.Id == metadata.SecretId)
        .FirstOrDefaultAsync();

    if (secret is null) throw new RpcException(new Status(StatusCode.NotFound, "Account secret not found"));

    // 2. If found (verified), generate token (JWT)
    // 2.1 Create claims
    var issuedAt = DateTime.UtcNow;
    var claims = new List<Claim>
    {
            new Claim(JwtRegisteredClaimNames.Sub, metadata.Id),
            new Claim(JwtRegisteredClaimNames.Email, info.Email)
        };
    // 2.2 Create token with claims(Sign token with secret key (secret key from 1.2))
    var token = tokenGenerator.GenerateToken(secret.SecretKey, claims, issuedAt);

    // 3. Return token and user info in AuthenticateResponse
    // 3.1 Create new session data
    var newSession = accounts.CreateNewSession(metadata.Id, tokenGenerator.WriteToken(token), issuedAt,
    token.ValidTo);
    // 3.2 Create AuthenticateResponse
    var response = new AuthenticateResponse {
      SessionId = newSession.Id,
      UserId = newSession.MetaId,
      Token = newSession.Token,
      ExpiresAt = Timestamp.FromDateTime(newSession.ExpiresAt.ToUniversalTime()),
    };
    // 3.3 Save response data to database (sessions table)
    await accounts.SaveSessionsAsync(newSession);
    // 3.4 Return response
    return response;
  }
}
