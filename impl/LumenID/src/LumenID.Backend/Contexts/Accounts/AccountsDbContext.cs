using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Accounts;

public partial class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options) {
  public DbSet<Models.Metadata> Metadata { get; set; }
  public DbSet<Models.Infos> Infos { get; set; }
  public DbSet<Models.Configs> Configs { get; set; }
  public DbSet<Models.Secrets> Secrets { get; set; }
  public DbSet<Models.Sessions> Sessions { get; set; }

  public async Task CreateNewAccountAsync(string email, string password_hash) {
    var new_info = CreateNewInfo(email);
    var new_secret = CreateNewSecret(password_hash, Guid.NewGuid().ToString());
    var new_config = CreateNewConfig();
    var new_metadata = CreateNewMetadata(new_info.Id, new_config.Id, new_secret.Id);

    await SaveInfosAsync(new_info);
    await SaveSecretsAsync(new_secret);
    await SaveConfigsAsync(new_config);
    await SaveMetadataAsync(new_metadata);
  }
}
