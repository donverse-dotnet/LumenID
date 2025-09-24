using Microsoft.EntityFrameworkCore;

namespace LumenID.Backend.Contexts.Clients;

public partial class OAuthClientsDbContext {
  public Models.Codes CreateNewCode(string baseUuid, string code, string userId, string appId) {
    var newData = new Models.Codes() {
      Id = baseUuid,
      Code = code,
      UserId = userId,
      AppId = appId,
      Used = false
    };
    return newData;
  }

  public async Task<OAuthClientsDbContext> SaveCodeAsync(Models.Codes code) {
    await Codes.AddAsync(code);
    await SaveChangesAsync();
    return this;
  }

  public async Task<Models.Codes?> GetCodeByCodeAsync(string code) {
    var data = await Codes.Where(item => item.Code == code).FirstOrDefaultAsync();
    return data;
  }
}
