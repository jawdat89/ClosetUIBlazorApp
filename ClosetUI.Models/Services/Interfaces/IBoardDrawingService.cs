using ClosetUI.Models.Models;

namespace ClosetUI.Models.Services.Interfaces;

public interface IBoardDrawingService
{
    Task<BoardDrawingData> PrepareDrawingData(ParamsModel paramsModel);
}
