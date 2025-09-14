using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext {
    public Models.Metadata CreateNewMetadata(string secretId, string profileId)
    {
        var now = DateTime.UtcNow;

        var newData = new Models.Metadata()
        {
            Id = Guid.NewGuid().ToString(),
            SecretId = secretId,
            ProfileId = profileId,

            CreatedAt = now,
            UpdatedAt = now,
        };
        return newData;
    }

    public async Task<OAuthClientsDbContext> SaveMetadataAsync(Models.Metadata metadata)
    {
        await Metadata.AddAsync(metadata);
        await SaveChangesAsync();
        
        return this;
    }

    public async Task<Models.Metadata?> GetMetadataByPrimaryKeyAsync(string primaryKey)
    {
        var data = await Metadata.Where(item => item.Id == primaryKey).FirstOrDefaultAsync();
        return data;
    }
    public async Task<Models.Metadata?> GetMetadataBySecretIdAsync(string secretId)
    {
        var data = await Metadata.Where(item => item.SecretId == secretId).FirstOrDefaultAsync();
        return data;
    }
    public async Task<Models.Metadata?> GetMetadataByProfileIdAsync(string profileId)
    {
        var data = await Metadata.Where(item => item.ProfileId == profileId).FirstOrDefaultAsync();
        return data;
    }
}
