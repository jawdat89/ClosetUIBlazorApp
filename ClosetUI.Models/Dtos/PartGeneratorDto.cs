using ClosetUI.Models.Locales;
using System.ComponentModel.DataAnnotations;

namespace ClosetUI.Models.Dtos;

public class PartGeneratorDto
{
    public int? Id { get; set; }
    [Required(ErrorMessageResourceName = "BladeThickness_Required", ErrorMessageResourceType = typeof(PartGeneratorDtoResource))]
    public int BladeThickness { get; set; }
    public int Direction { get; set; }
    [Required(ErrorMessageResourceName = "TotalWidth_Required", ErrorMessageResourceType = typeof(PartGeneratorDtoResource))]
    public int TotalWidth { get; set; }
    [Required(ErrorMessageResourceName = "TotalHeight_Required", ErrorMessageResourceType = typeof(PartGeneratorDtoResource))]
    public int TotalHeight { get; set; }
    public int HypotenuseType { get; set; }
    public List<PartInput> Parts { get; set; } = [];

    public void Process()
    {
        // Example logic
        // This needs to be adapted to your actual logic and PartInput structure
        Parts.ForEach(part => part.ApplyBladeThickness(BladeThickness));
    }
}
