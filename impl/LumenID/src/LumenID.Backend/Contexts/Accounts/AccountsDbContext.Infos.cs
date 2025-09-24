using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Accounts;

public partial class AccountsDbContext {
  public Models.Infos CreateNewInfo(string email) {
    return new Models.Infos {
      Id = Guid.NewGuid().ToString(),
      Username = email.Split('@')[0],
      Email = email,
      AvatarId = string.Empty,
      HeaderId = string.Empty,
      KeyColor = string.Empty
    };
  }

  public async Task<AccountsDbContext> SaveInfosAsync(Models.Infos new_info) {
    // Add to DbSet
    await Infos.AddAsync(new_info);

    // Save changes to database
    await SaveChangesAsync();

    return this;
  }

  public async Task<bool> IsEmailExistsAsync(string email) {
    var result = await Infos
        .Where(data => data.Email == email)
        .FirstOrDefaultAsync();

    return result != null; // If exists, return true
  }

  public async Task<Models.Infos?> GetInfoAsync(string primaryKey) {
    var data = await Infos.Where(item => item.Id == primaryKey).FirstOrDefaultAsync();
    return data;
  }
}
