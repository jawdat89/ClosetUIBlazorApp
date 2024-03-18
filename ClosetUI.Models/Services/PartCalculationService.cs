using ClosetUI.Models.Models;
using ClosetUI.Models.Dtos;
using System.Text.Json;

namespace ClosetUI.Services;

public class PartCalculationService : IPartCalculationService
{
    private ParamsModel? _params { get; set; }

    public async Task<ParamsModel> GetParams()
    {
        return _params;
    }
    public async Task<ParamsModel> ProcessAsync(PartGeneratorDto parameters)
    {
        try
        {
            // Convert DTO to domain model (ParamsModel)
            var paramsObj = ConvertToParams(parameters);

            // Ensure parts are processed (e.g., blade thickness applied, hypotenuse calculated)
            foreach (var part in paramsObj.Parts)
            {
                part.AddBladeThickness(paramsObj.BladeThickness);
                part.CalcHypotenuse();
            }

            // Run fitting logic to organize parts
            await paramsObj.Fitting(); // Assuming Fitting is async; if not, just call without await

            // At this point, paramsObj contains organized parts; 
            // you can optionally prepare drawing data or return paramsObj for further processing
            _params = paramsObj; // Storing processed params for other uses if needed

            return paramsObj;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to process calculations: " + ex.Message);
        }
    }

    private ParamsModel ConvertToParams(PartGeneratorDto dto)
    {
        var closetParts = ConvertToClosetParts(dto.Parts);


        return new ParamsModel
        {
            BladeThickness = dto.BladeThickness,
            Direction = dto.Direction,
            TotalWidth = dto.TotalWidth,
            TotalHeight = dto.TotalHeight,
            Hypotenuse = dto.HypotenuseType,
            Parts = closetParts
        };
    }

    private List<ClosetPart> ConvertToClosetParts(List<PartInput> parts)
    {
        var partsList = new List<ClosetPart>();

        foreach (var part in parts)
        {
            ClosetPart closetPart = new()
            {
                ID = part.ID,
                PartName = part.PartName,
                PartWidth = part.PartWidth,
                PartHeight = part.PartHeight,
                PartQty = part.PartQty,
            };

            partsList.Add(closetPart);
        }

        return partsList;
    }
}
