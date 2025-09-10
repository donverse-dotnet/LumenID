using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext {
    public Models.Secrets CreateNewSecret(string appName, string redirectUri)
    {
        using var sha256 = SHA256.Create();
        var publicKeyUuid = Guid.NewGuid().ToString();
        var publicKeyUuidBytes = Encoding.UTF8.GetBytes(publicKeyUuid);
        var publicKeyHash = sha256.ComputeHash(publicKeyUuidBytes);
        var publicKey = Base64Url.EncodeToString(publicKeyHash);

        var secretKeyUuid = Guid.NewGuid().ToString();
        var secretKeyBytes = Encoding.UTF8.GetBytes(secretKeyUuid);
        var secretKey = sha256.ComputeHash(secretKeyBytes);

        var publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
        var hmac = HMACSHA256.HashData(secretKey, publicKeyBytes);

        var newData = new Models.Secrets()
        {
            Id = publicKeyUuid,
            PublicKey = publicKey,
            PublicValue = hmac.ToString() ?? throw new InvalidOperationException("Can not generate OAuth client password"),
            SecretKey = secretKey.ToString() ?? throw new InvalidOperationException("Can not generate secret key"),
            RedirectUrl = redirectUri
        };
        
        return newData;
    }

    public async Task<OAuthClientsDbContext> SaveSecretAsync(Models.Secrets secret)
    {
        await Secrets.AddAsync(secret);
        await SaveChangesAsync();
        
        return this;
    }

    public async Task<Models.Secrets?> GetSecretByPrimaryKeyAsync(string secretId)
    {
        var data = await Secrets.Where(item => item.Id == secretId).FirstOrDefaultAsync();
        return data;
    }
}
