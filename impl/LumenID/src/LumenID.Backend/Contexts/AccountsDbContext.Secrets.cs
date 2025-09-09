namespace LumenID.Backend.Contexts;

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
}
