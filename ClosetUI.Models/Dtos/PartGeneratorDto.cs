namespace ClosetUI.Models.Dtos;

public class PartGeneratorDto
{
    public int? Id { get; set; }
    public int BladeThickness { get; set; }
    public int Direction { get; set; }
    public int TotalWidth { get; set; }
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
