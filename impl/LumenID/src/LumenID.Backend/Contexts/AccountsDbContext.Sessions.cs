namespace LumenID.Backend.Contexts;

public partial class AccountsDbContext {
    public Models.Sessions CreateNewSession(string meta_id, string token, DateTime tokenCreatedAt, DateTime tokenExpiresAt)
    {
        return new Models.Sessions
        {
            Id = Guid.NewGuid().ToString(),
            MetaId = meta_id,
            Token = token,
            CreatedAt = tokenCreatedAt,
            UpdatedAt = tokenCreatedAt,
            ExpiresAt = tokenExpiresAt
        };
    }

    public async Task<AccountsDbContext> SaveSessionsAsync(Models.Sessions new_session)
    {
        // Add to DbSet
        await Sessions.AddAsync(new_session);

        // Save changes to database
        await SaveChangesAsync();

        return this;
    }
}
