using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Accounts;

public partial class AccountsDbContext {
    public Models.Secrets CreateNewSecret(string password_hash, string secret_key)
    {
        return new Models.Secrets
        {
            Id = Guid.NewGuid().ToString(),
            Password = password_hash,
            SecretKey = secret_key
        };
    }

    public async Task<AccountsDbContext> SaveSecretsAsync(Models.Secrets new_secret)
    {
        // Add to DbSet
        await Secrets.AddAsync(new_secret);

        // Save changes to database
        await SaveChangesAsync();

        return this;
    }

    public async Task<Models.Secrets?> GetSecretAsync(string primaryKey)
    {
        var data = await Secrets.Where(item => item.Id == primaryKey).FirstOrDefaultAsync();
        return data;
    }
}
