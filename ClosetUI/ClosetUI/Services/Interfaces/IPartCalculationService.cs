using ClosetUI.Models.Dtos;
using ClosetUI.Models.Models;

namespace ClosetUI.Services;

public interface IPartCalculationService
{
    Task<ParamsModel> ProcessAsync(PartGeneratorDto parameters);
}
