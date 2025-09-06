namespace LumenID.Backend.Contexts;

public partial class AccountsDbContext
{
    public Models.Configs CreateNewConfig()
    {
        return new Models.Configs
        {
            Id = Guid.NewGuid().ToString(),
            Notify = new Models.NotifyConfig(),
            Theme = new Models.ThemeConfig()
        };
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
