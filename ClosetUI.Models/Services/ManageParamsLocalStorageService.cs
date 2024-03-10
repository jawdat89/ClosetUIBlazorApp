//using Blazored.LocalStorage;
//using ClosetUI.Models.Models;

//namespace ClosetUI.Services;

//public class ManageParamsLocalStorageService : IManageParamsLocalStorageService
//{
//    private readonly ILocalStorageService _localStorageService;

//    private const string key = "ParamsCollection";

//    public ManageParamsLocalStorageService(ILocalStorageService localStorageService, IPartCalculationService partCalculationService)
//    {
//        _localStorageService = localStorageService;
//    }
//    public async Task<ParamsModel> GetCollection()
//    {
//        return await _localStorageService.GetItemAsync<ParamsModel>(key);
//    }

//    public async Task RemoveCollection()
//    {
//        await _localStorageService.RemoveItemAsync(key);
//    }

//    public async Task AddCollection(ParamsModel paramsModel)
//    {
//        await _localStorageService.SetItemAsync(key, paramsModel);
//    }
//}
