using System.Security.Cryptography;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LumenID.Backend.Contexts.Accounts;
using LumenID.Backend.Contexts.Clients;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LumenID.Backend.Services;

public class GrantServiceImpl(
    [FromServices] OAuthClientsDbContext oauth,
    [FromServices] AccountsDbContext accounts,
    [FromServices] ILogger<GrantServiceImpl> logger
) : GrantService.GrantServiceBase {
    public override async Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        logger.LogInformation("{user}", context.AuthContext.IsPeerAuthenticated);
        return await Task.FromResult(new Empty());
    }

    [Authorize(Policy = "users")]
    public override async Task<GrantResponse> GrantApp(GrantRequest request, ServerCallContext context)
    {
        logger.LogInformation("{user} grant permission with client {client}", context.AuthContext.IsPeerAuthenticated, request.ClientId);
        // 1. Get OAuth client data from database
        var clientMeta = await oauth.GetMetadataByPrimaryKeyAsync(request.ClientId);
        if (clientMeta is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Client does not exist"));
        }
        
        var clientSecret = await oauth.GetSecretByPrimaryKeyAsync(clientMeta.SecretId);
        if (clientSecret is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"{request.ClientId} is maybe broken. Please contact your administrator"));
        }
        
        // 2. Check redirect url is same
        if (request.RedirectUrl != clientSecret.RedirectUrl)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Redirect URL is invalid"));
        }
        
        // 3. Add granted apps to user account
        var userId = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (userId is null) throw new RpcException(new Status(StatusCode.NotFound, "User not found in claims"));
        
        logger.LogInformation("{user}", userId);
        
        var userMeta = await accounts.GetMetadataAsync(userId);
        if (userMeta is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
        
        var userConfig = await accounts.GetConfigAsync(userMeta.ConfigId);
        if (userConfig is null) throw new RpcException(new Status(StatusCode.NotFound, "Account config not found"));

        try
        {
            var grantedApps = userConfig.GetGrantedApps();
            if (grantedApps.AppIds.Contains(request.ClientId) is false)
            {
                grantedApps.AppIds.Add(request.ClientId);
            }
            userConfig.SetGrantedApps(grantedApps);
            await accounts.UpdateConfigsAsync(userConfig);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Account update failed"));
        }
        
        // 4. Generate code
        var uuid = Guid.NewGuid().ToString("N");
        var uuidBytes = Encoding.UTF8.GetBytes(uuid);
        var secretKeyBytes = Encoding.UTF8.GetBytes(clientSecret.SecretKey);
        var uuidHmac = HMACSHA256.HashData(secretKeyBytes, uuidBytes);
        var code = Convert.ToHexString(uuidHmac).ToLower();
        
        if (code is null or "") throw new RpcException(new Status(StatusCode.InvalidArgument, "Server can not generate code."));
        
        var newCode = oauth.CreateNewCode(uuid, code, userMeta.Id, request.ClientId);
        await oauth.SaveCodeAsync(newCode);
        
        // 5. Return response
        return new GrantResponse()
        {
            Code = code,
        };
    }
}
