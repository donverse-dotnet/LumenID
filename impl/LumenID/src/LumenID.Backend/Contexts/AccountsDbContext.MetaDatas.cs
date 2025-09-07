namespace LumenID.Backend.Contexts;

public partial class AccountsDbContext
{
    public Models.Metadata CreateNewMetadata(string info_id, string config_id, string secret_id)
    {
        return new Models.Metadata
        {
            Id = Guid.NewGuid().ToString(),
            InfoId = info_id,
            ConfigId = config_id,
            SecretId = secret_id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeactivatedAt = null
        };
    }

    public async Task<AccountsDbContext> SaveMetadataAsync(Models.Metadata new_metadata)
    {
        // Add to DbSet
        await Metadata.AddAsync(new_metadata);

        // Save changes to database
        await SaveChangesAsync();

        return this;
    }
}
