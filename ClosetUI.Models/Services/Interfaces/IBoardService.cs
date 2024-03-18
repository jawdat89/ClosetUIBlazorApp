using ClosetUI.Models.Models;

namespace ClosetUI.Services;

public interface IBoardService
{
    Task<BoardDrawingData> PrepareDrawingData(ParamsModel paramsModel);
}