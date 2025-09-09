namespace LumenID.Backend.Contexts;

public partial class AccountsDbContext
{
    public Models.Configs CreateNewConfig()
    {
        var new_config = new Models.Configs
        {
            Id = Guid.NewGuid().ToString(),
        };

        new_config.SetNotifyConfig(new Models.NotifyConfig());
        new_config.SetThemeConfig(new Models.ThemeConfig());
        new_config.SetGrantedApps(new Models.GrantedApps());

        return new_config;
    }

    public async Task<AccountsDbContext> SaveConfigsAsync(Models.Configs new_config)
    {
        // Add to DbSet
        await Configs.AddAsync(new_config);

        // Save changes to database
        await SaveChangesAsync();

        return this;
    }
}
