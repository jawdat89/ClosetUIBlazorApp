namespace ClosetUI.Models.Dtos;

public class PartInput
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int PartWidth { get; set; }
    public int PartHeight { get; set; }
    public int PartQty { get; set; }
    public int Measure { get; set; }

    private static int Incremental = 0;

    public PartInput()
    {
        Id = Incremental++;
        PartName = "";
        PartWidth = 0;
        PartHeight = 0;
        PartQty = 0;
    }

    public void ApplyBladeThickness(int bladeThickness)
    {
        // Adjust the part dimensions based on the blade thickness
        PartWidth += bladeThickness;
        PartHeight += bladeThickness;
    }
}
