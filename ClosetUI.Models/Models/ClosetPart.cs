namespace ClosetUI.Models.Models;

public class ClosetPart
{
    public int ID { get; set; }
    public int PlateID { get; set; }
    public int X { get; set; }  // in mm   X on big wood plate
    public int Y { get; set; }  // in mm   Y on big wood plate
    public int Wt { get; set; } // in mm   part width  + blade thikness
    public int Ht { get; set; } // in mm   part height  + blade thikness
    public int Hypotenuse { get; set; }
    public string PartName { get; set; }
    public int PartWidth { get; set; }
    public int PartHeight { get; set; }
    public int PartQty { get; set; }

    public ClosetPart()
    {
        PartName = "";
        PartWidth = 0;
        PartHeight = 0;
        PartQty = 0;
    }

    public void CalcHypotenuse()
    {
        Hypotenuse = (int)Math.Ceiling(Math.Sqrt(Math.Pow(Wt, 2) + Math.Pow(Ht, 2)));
    }

    public void AddBladeThickness(int bladeThikness)
    {
        Wt = PartWidth + bladeThikness;
        Ht = PartWidth + bladeThikness;
    }

    public async Task AddBladeThicknessAsync(int bladeThikness)
    {
        await Task.Run(() => AddBladeThickness(bladeThikness));
    }
}
