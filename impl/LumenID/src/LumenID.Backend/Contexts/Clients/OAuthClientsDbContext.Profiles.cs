using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext {
    public Models.Profiles CreateNewProfile(string name, string description, string iconId, string terms, string privacy)
    {
        var newData = new Models.Profiles()
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            IconId = iconId,
            Terms = terms,
            Privacy = privacy
        };
        return newData;
    }

    public async Task<OAuthClientsDbContext> SaveProfileAsync(Models.Profiles profile)
    {
        await Profiles.AddAsync(profile);
        await SaveChangesAsync();

        return this;
    }

    public async Task<Models.Profiles?> GetProfileByPrimaryKeyAsync(string primaryKey)
    {
        var data = await Profiles.Where(item => item.Id == primaryKey).FirstOrDefaultAsync();
        return data;
    }
}
