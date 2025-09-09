using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LumenID.Backend.Contexts;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Services;

public class AuthenticateServiceImpl(
    [FromServices] TokenGenerator tokenGenerator,
    [FromServices] AccountsDbContext database,
    [FromServices] ILogger<RegisterServiceImpl> logger
) : AuthenticateService.AuthenticateServiceBase {
    public override async Task<AuthenticateResponse> Authenticate(AuthAccountModel request, ServerCallContext context)
    {
        // 1. Find user (verify) with email and password
        // 1.1 Get info using email
        var info = await database.Infos
            .Select(item => new { item.Id, item.Email })
            .Where(item => item.Email == request.Email)
            .FirstOrDefaultAsync();
        if (info is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));

        // 1.3 Get metadata using user id and secret id
        var metadata = await database.Metadata
            .Select(item => new { item.Id, item.SecretId, item.InfoId, item.CreatedAt, item.UpdatedAt })
            .Where(item => item.InfoId == info.Id)
            .FirstOrDefaultAsync();

        if (metadata is null) throw new RpcException(new Status(StatusCode.NotFound, "Account metadata not found"));

        // 1.2 Get secret using user id
        var secret = await database.Secrets
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
            new Claim(JwtRegisteredClaimNames.Sub, info.Id),
            new Claim(JwtRegisteredClaimNames.Email, info.Email)
        };
        // 2.2 Create token with claims(Sign token with secret key (secret key from 1.2))
        var token = tokenGenerator.GenerateToken(metadata.Id, secret.SecretKey, claims, issuedAt);

        // 3. Return token and user info in AuthenticateResponse
        // 3.1 Create new session data
        var newSession = database.CreateNewSession(metadata.Id, tokenGenerator.WriteToken(token), issuedAt,
        token.ValidTo);
        // 3.2 Create AuthenticateResponse
        var response = new AuthenticateResponse
        {
            SessionId = newSession.Id,
            UserId = newSession.MetaId,
            Token = newSession.Token,
            ExpiresAt = Timestamp.FromDateTime(newSession.ExpiresAt.ToUniversalTime()),
        };
        // 3.3 Save response data to database (sessions table)
        await database.SaveSessionsAsync(newSession);
        // 3.4 Return response
        return response;
    }
}
