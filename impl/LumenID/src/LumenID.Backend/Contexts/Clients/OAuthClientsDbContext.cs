using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext(DbContextOptions<OAuthClientsDbContext> options) : DbContext(options) {
    public DbSet<Models.Secrets> Secrets { get; set; }
    public DbSet<Models.Profiles> Profiles { get; set; }
    public DbSet<Models.Metadata> Metadata { get; set; }
    public DbSet<Models.Codes> Codes { get; set; }
    
    public async Task CreateNewClientDataAsync(string clientName, string redirectUri)
    {
        var newSecret = CreateNewSecret(clientName, redirectUri);
        var newProfile = CreateNewProfile(clientName, "", "", "", "");
        var newMetadata = CreateNewMetadata(newSecret.Id, newProfile.Id);
        
        await SaveSecretAsync(newSecret);
        await SaveProfileAsync(newProfile);
        await SaveMetadataAsync(newMetadata);
    }
}
