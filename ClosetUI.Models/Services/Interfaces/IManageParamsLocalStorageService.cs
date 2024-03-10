using ClosetUI.Models.Models;

namespace ClosetUI.Services;

public interface IManageParamsLocalStorageService
{
    Task<ParamsModel> GetCollection();
    Task RemoveCollection();
    Task AddCollection(ParamsModel paramsModel);
}
