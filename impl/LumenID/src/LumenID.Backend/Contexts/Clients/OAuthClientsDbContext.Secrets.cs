using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext {
  public Models.Secrets CreateNewSecret(string appName, string redirectUri) {
    var secretKeyUuid = Guid.NewGuid().ToString();
    var secretKeyBytes = Encoding.UTF8.GetBytes(secretKeyUuid);

    using var sha256 = SHA256.Create();
    var secretKey = sha256.ComputeHash(secretKeyBytes);

    var newData = new Models.Secrets() {
      Id = Guid.NewGuid().ToString(),
      SecretKey = secretKey.ToString() ?? throw new InvalidOperationException("Can not generate secret key"),
      RedirectUrl = redirectUri
    };

    return newData;
  }

  public async Task<OAuthClientsDbContext> SaveSecretAsync(Models.Secrets secret) {
    await Secrets.AddAsync(secret);
    await SaveChangesAsync();

    return this;
  }

  public async Task<Models.Secrets?> GetSecretByPrimaryKeyAsync(string secretId) {
    var data = await Secrets.Where(item => item.Id == secretId).FirstOrDefaultAsync();
    return data;
  }
}
