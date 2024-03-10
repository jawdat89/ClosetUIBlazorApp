using ClosetUI.Models.Models;
using ClosetUI.Models.Dtos;

namespace ClosetUI.Services;

public class PartCalculationService : IPartCalculationService
{
    public Task<List<PartMeasu>> FindClosestSeriesAsync(List<PartMeasu> sortedList, int target)
    {
        throw new NotImplementedException();
    }

    public async Task<ParamsModel> ProcessAsync(PartGeneratorDto parameters)
    {
        try
        {
            // Mocking the Parts for the demonstration purpose since the `Ceed` method is not present in PartGeneratorDto.
            // Ideally, the PartGeneratorDto should already have the Parts populated before this method is called.

            // Convert PartGeneratorDto to Params object
            var paramsObj = ConvertToParams(parameters);

            // Perform the calculations
            await paramsObj.CalcHypotenuseAsync();
            var addThicknessTasks = paramsObj.Parts.Select(part => part.AddBladeThicknessAsync(paramsObj.BladeThikness)).ToArray();
            await Task.WhenAll(addThicknessTasks);

            // Call the new function to calculate positions
            paramsObj.CalculatePartPositions(paramsObj);

            paramsObj.FillAllWidths();
            paramsObj.FillAllHeights();
            paramsObj.FillAllHypotenuse();
            paramsObj.AggrigWidth();
            //paramsObj.AggrigHeight();
            paramsObj.AggrigHypotenuse();

            return paramsObj;
        }
        catch (Exception)
        {

            throw new Exception("Failed to Prcess Calculations");
        }
    }

    private ParamsModel ConvertToParams(PartGeneratorDto dto)
    {
        var closetParts = ConvertToClosetParts(dto.Parts);


        return new ParamsModel
        {
            BladeThikness = dto.BladeThickness,
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
            ClosetPart closetPart = new ClosetPart
            {
                ID = part.Id,
                PartName = part.PartName,
                PartWidth = part.PartWidth,
                PartHeight = part.PartHeight,
                PartQty = part.PartQty
            };

            partsList.Add(closetPart);
        }

        return partsList;
    }
}
