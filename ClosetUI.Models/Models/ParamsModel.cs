namespace ClosetUI.Models.Models;

public class ParamsModel
{
    public int WoodType { get; set; }  // 1 = Interior or 2 = Outsider
    public int Direction { get; set; } // 1 = Verticat or 2 = Horizontal
    public int BladeThikness { get; set; }   //   in mm
    public int TotalWidth { get; set; }  // X in mm
    public int TotalHeight { get; set; } // Y in mm
    public int Hypotenuse { get; set; }
    public List<ClosetPart> Parts { get; set; }
    private List<PartMeasu> AllWidths { get; set; }
    private List<PartMeasu> AllHeights { get; set; }
    private List<PartMeasu> AllHypotenuse { get; set; }
    public List<List<PartMeasu>> FitWidths { get; set; }
    public List<List<PartMeasu>> FitHeights { get; set; }
    public List<List<PartMeasu>> FitHypotenuse { get; set; }

    public ParamsModel()
    {
        WoodType = 2;   // Outsider
        Direction = 1;  // Verticat
        TotalWidth = 1220;
        TotalHeight = 2440;
        BladeThikness = 2;
        Parts = new List<ClosetPart>();
        AllWidths = new List<PartMeasu>();
        AllHeights = new List<PartMeasu>();
        AllHypotenuse = new List<PartMeasu>();
        FitWidths = new List<List<PartMeasu>>();
        FitHeights = new List<List<PartMeasu>>();
        FitHypotenuse = new List<List<PartMeasu>>();
    }

    public void CalcHypotenuse()
    {
        Hypotenuse = (int)Math.Floor((Math.Sqrt(Math.Pow(TotalWidth, 2) + Math.Pow(TotalHeight, 2))));
    }

    public async Task CalcHypotenuseAsync()
    {
        await Task.Run(() =>
        {
            // Perform the calculation on a background thread
            CalcHypotenuse();
        });
    }

    public ParamsModel Ceed()
    {
        Parts.Add(new ClosetPart() { ID = 1, PartName = "part1", PartWidth = 250, PartHeight = 350, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 2, PartName = "part2", PartWidth = 350, PartHeight = 550, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 3, PartName = "part3", PartWidth = 400, PartHeight = 500, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 4, PartName = "part4", PartWidth = 510, PartHeight = 650, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 5, PartName = "part5", PartWidth = 430, PartHeight = 750, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 6, PartName = "part6", PartWidth = 150, PartHeight = 450, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 7, PartName = "part7", PartWidth = 270, PartHeight = 600, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 8, PartName = "part8", PartWidth = 310, PartHeight = 620, PartQty = 3 });
        Parts.Add(new ClosetPart() { ID = 9, PartName = "part9", PartWidth = 700, PartHeight = 840, PartQty = 3 });
        return this;
    }

    public void FillAllWidths()
    {
        try
        {
            Parts = Parts.OrderBy(x => x.Wt).ToList();//.ThenBy(x => x.PartHeigth).ToList();
            AllWidths = Parts.GroupBy(t => t.PartName)
                                     .Select(g => new PartMeasu
                                     {
                                         ID = g.FirstOrDefault().ID,
                                         Measure = g.FirstOrDefault().Wt
                                     }).OrderBy(x => x.Measure).ToList();
        }
        catch (Exception)
        {
            throw new Exception("Failed to Fill All Widths");
        }
    }

    public void FillAllHeights()
    {
        try
        {
            Parts = Parts.OrderBy(x => x.Ht).ToList();//.ThenBy(x => x.PartHeigth).ToList();
            AllHeights = Parts.GroupBy(t => t.PartName)
                                     .Select(g => new PartMeasu
                                     {
                                         ID = g.FirstOrDefault().ID,
                                         Measure = g.FirstOrDefault().Ht
                                     }).OrderBy(x => x.Measure).ToList();
        }
        catch (Exception)
        {
            throw new Exception("Failed to Fill All Heights");
        }
    }

    public void FillAllHypotenuse()
    {
        try
        {
            Parts = Parts.OrderBy(x => x.Hypotenuse).ToList();//.ThenBy(x => x.PartHeigth).ToList();
            AllHeights = Parts.GroupBy(t => t.PartName)
                                     .Select(g => new PartMeasu
                                     {
                                         ID = g.FirstOrDefault().ID,
                                         Measure = g.FirstOrDefault().Hypotenuse
                                     }).OrderBy(x => x.Measure).ToList();
        }
        catch (Exception)
        {
            throw new Exception("Failed to Fill All Hypotenuse");
        }
    }

    public List<List<PartMeasu>> AggrigWidth()
    {
        try
        {
            List<List<PartMeasu>> ll = new List<List<PartMeasu>>();
            List<PartMeasu> all_widths = AllWidths.OrderBy(x => x.Measure).ToList();
            while (all_widths.Count > 0)
            {
                List<PartMeasu> l = Processing.FindClosestSeries(all_widths, TotalWidth);
                // Remove items from list1 where Id exists in list2
                all_widths = all_widths.Where(item1 => !l.Any(item2 => item2.ID == item1.ID)).ToList();
                ll.Add(l);
            }
            FitWidths = ll;
            return ll;
        }
        catch (Exception)
        {
            throw new Exception("Failed to Aggrigate Width");
        }
    }

    public List<List<PartMeasu>> AggrigHeight()
    {
        try
        {
            List<List<PartMeasu>> ll = new List<List<PartMeasu>>();
            List<PartMeasu> all_heights = AllHeights.OrderBy(x => x.Measure).ToList();
            while (all_heights.Count > 0)
            {
                List<PartMeasu> l = Processing.FindClosestSeries(all_heights, TotalWidth);
                // Remove items from list1 where Id exists in list2
                all_heights = all_heights.Where(item1 => !l.Any(item2 => item2.ID == item1.ID)).ToList();
                ll.Add(l);
            }
            FitHeights = ll;
            return ll;
        }
        catch (Exception)
        {
            throw new Exception("Failed to Aggrigate Height");
        }
    }

    public List<List<PartMeasu>> AggrigHypotenuse()
    {
        try
        {
            List<List<PartMeasu>> ll = new List<List<PartMeasu>>();
            List<PartMeasu> all_hypotenuse = AllHypotenuse.OrderBy(x => x.Measure).ToList();
            while (all_hypotenuse.Count > 0)
            {
                List<PartMeasu> l = Processing.FindClosestSeries(all_hypotenuse, Hypotenuse);
                // Remove items from list1 where Id exists in list2
                all_hypotenuse = all_hypotenuse.Where(item1 => !l.Any(item2 => item2.ID == item1.ID)).ToList();
                ll.Add(l);
            }
            FitHypotenuse = ll;
            return ll;
        }
        catch (Exception)
        {
            throw new Exception("Failed to Aggrigate Hypotenuse");
        }
    }

    public void CalculatePartPositions(ParamsModel p)
    {
        int currentX = 0, currentY = 0;
        int maxHeightInRow = 0;

        foreach (var part in p.Parts)
        {
            if (currentX + part.Wt > p.TotalWidth)
            {
                // Move to the next row
                currentY += maxHeightInRow;
                currentX = 0;
                maxHeightInRow = 0;
            }

            part.X = currentX;
            part.Y = currentY;

            currentX += part.Wt;
            maxHeightInRow = Math.Max(maxHeightInRow, part.Ht);

            // Check if the current Y position exceeds the total height, handle accordingly
            if (currentY + maxHeightInRow > p.TotalHeight)
            {
                // Exceeded material height, handle according to your requirements
                break;
            }
        }
    }
}
