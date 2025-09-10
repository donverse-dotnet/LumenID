using System.Security.Cryptography;
using System.Text;
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
    
    [Authorize(Policy = "users")]
    public override async Task<GrantResponse> GrantApp(GrantRequest request, ServerCallContext context)
    {
        // 1. Get OAuth client data from database
        var clientMeta = await oauth.GetMetadataByPrimaryKeyAsync(request.ClientId);
        if (clientMeta is null) throw new RpcException(new Status(StatusCode.NotFound, "Client does not exist"));
        
        var clientSecret = await oauth.GetSecretByPrimaryKeyAsync(clientMeta.SecretId);
        if (clientSecret is null) throw new RpcException(new Status(StatusCode.NotFound, $"{request.ClientId} is maybe broken. Please contact your administrator"));
        
        // 2. Check public key is right
        var publicKeyBytes = Encoding.UTF8.GetBytes(request.ClientPublicKey);
        var secretKeyBytes = Encoding.UTF8.GetBytes(clientSecret.SecretKey);
        var hmac = HMACSHA256.HashData(secretKeyBytes, publicKeyBytes);
        var hmacString = hmac.ToString();
        if (hmacString is null || clientSecret.PublicValue != hmacString) throw new RpcException(new Status(StatusCode.InvalidArgument, "Public key does not match"));
        
        // 3. Add granted apps to user account
        var userId = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (userId is null) throw new RpcException(new Status(StatusCode.NotFound, "User not found in claims"));
        
        var userMeta = await accounts.GetMetadataAsync(userId);
        if (userMeta is null) throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
        
        var userConfig = await accounts.GetConfigAsync(userMeta.ConfigId);
        if (userConfig is null) throw new RpcException(new Status(StatusCode.NotFound, "Account config not found"));

        try
        {
            var grantedApps = userConfig.GetGrantedApps();
            grantedApps.AppIds.Add(request.ClientId);
            userConfig.SetGrantedApps(grantedApps);
            await accounts.SaveConfigsAsync(userConfig);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Account update failed"));
        }
        
        // 4. Generate code
        var uuid = Guid.NewGuid().ToString("N");
        var uuidBytes = Encoding.UTF8.GetBytes(uuid);
        var uuidHmac = HMACSHA256.HashData(secretKeyBytes, uuidBytes);
        var uuidString = uuidHmac.ToString();
        
        if (uuidString is null or "") throw new RpcException(new Status(StatusCode.InvalidArgument, "Server can not generate code."));
        
        var newCode = oauth.CreateNewCode(uuidString, userMeta.Id, request.ClientId);
        await oauth.SaveCodeAsync(newCode);
        
        // 5. Return response
        return new GrantResponse()
        {
            Code = uuidString,
        };
    }
}
