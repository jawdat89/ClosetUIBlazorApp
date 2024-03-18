using System.Text.Json.Serialization;

namespace ClosetUI.Models.Models;

public class ParamsModel
{
    public int WoodType { get; set; }  // 1 = Interior or 2 = Outsider
    public int Direction { get; set; } // 1 = Verticat or 2 = Horizontal
    public int BladeThickness { get; set; }   //   in mm
    public int TotalWidth { get; set; }  // X in mm
    public int TotalHeight { get; set; } // Y in mm
    public int Hypotenuse { get; set; }
    public List<ClosetPart> Parts { get; set; }
    public List<PartMeasu> AllWidths { get; set; }
    public List<PartMeasu> AllHeights { get; set; }
    public List<PartMeasu> AllHypotenuse { get; set; }
    public List<List<PartMeasu>> FitWidths { get; set; }
    public List<List<PartMeasu>> FitHeights { get; set; }
    public List<List<PartMeasu>> FitHypotenuse { get; set; }

    public ParamsModel()
    {
        WoodType = 2;   // Outsider
        Direction = 1;  // Verticat
        TotalWidth = 1220;
        TotalHeight = 2440;
        BladeThickness = 2;
        Parts = [];
        AllWidths = [];
        AllHeights = [];
        AllHypotenuse = [];
        FitWidths = [];
        FitHeights = [];
        FitHypotenuse = [];
    }

    public async Task Fitting()
    {
        await Task.Run(() =>
        {
            //Ceed();
            CalcHypotenuse();
            Parts.ForEach(x => x.AddBladeThickness(BladeThickness));
            Parts.ForEach(x => x.CalcHypotenuse());
            FillAllWidths();
            FillAllHeights();
            FillAllHypotenuse();
            AggrigWidth();
            AggrigHeight();
            AggrigHypotenuse();
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
        Generate();
        return this;
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


    public void FillAllWidths()
    {
        try
        {
            // Order parts by width and then by height to prepare for width-based aggregation.
            List<ClosetPart> lcps
                = Parts.OrderBy(x => x.Wt).ThenBy(x => x.PartHeight).ToList();


            // Initially, a grouping by part name was considered to ensure uniqueness, but it is not utilized in the final approach.
            // This commented-out code can be removed or adapted if the grouping logic is needed in future revisions.
            /*
            AllWidths = lcps.GroupBy(t => t.PartName)
                            .Select(g => new PartMeasu
                            {
                                ID = g.FirstOrDefault().ID,
                                Measure = g.FirstOrDefault().Wt
                            }).OrderBy(x => x.Measure).ToList();
            */

            // Populate the AllWidths list with each part's width, ensuring parts are sorted by their width.
            // This makes the AllWidths list a straightforward representation of all parts' widths, ready for further processing.
            AllWidths = lcps.Select(x => new PartMeasu
            {
                ID = x.ID,
                Measure = x.Wt
            }).OrderBy(x => x.Measure).ToList();
        }
        catch (Exception)
        {
            // In the event of an error, throw a descriptive exception to aid in debugging and handling.
            throw new Exception("Failed to Fill All Widths");
        }
    }

    public void FillAllHeights()
    {
        try
        {
            List<ClosetPart> lcps
                = this.Parts.OrderBy(x => x.Ht).ThenBy(x => x.PartHeight).ToList();
            AllHeights = lcps.GroupBy(t => t.PartName)
                                     .Select(g => new PartMeasu
                                     {
                                         ID = g.FirstOrDefault().ID,
                                         Measure = g.FirstOrDefault().Ht
                                     }).OrderBy(x => x.Measure).ToList();

            AllHeights = lcps.Select(x => new PartMeasu
            {
                ID = x.ID,
                Measure = x.Ht
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
            List<ClosetPart> lcps
                = this.Parts.OrderBy(x => x.Hypotenuse).ThenBy(x => x.Ht).ThenBy(x => x.PartHeight).ToList();
            AllHypotenuse = lcps.GroupBy(t => t.PartName)
                                     .Select(g => new PartMeasu
                                     {
                                         ID = g.FirstOrDefault().ID,
                                         Measure = g.FirstOrDefault().Hypotenuse
                                     }).OrderBy(x => x.Measure).ToList();

            AllHypotenuse = lcps
                                   .Select(x => new PartMeasu
                                   {
                                       ID = x.ID,
                                       Measure = x.Hypotenuse
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
            // A list to hold the final groups of parts.
            List<List<PartMeasu>> ll = [];

            // Create a new list to hold multiple instances of each part based on its quantity.
            // This ensures that the quantity of each part is considered in the aggregation process.
            List<PartMeasu> allWidthsExpanded = [];

            // Expand the AllWidths list to include duplicates based on each part's quantity.
            foreach (var width in AllWidths)
            {
                // Find the corresponding part in the parts list.
                var part = Parts.FirstOrDefault(p => p.ID == width.ID);
                if (part != null)
                {
                    // Add the part to the expanded list as many times as its quantity.
                    for (int i = 0; i < part.PartQty; i++)
                    {
                        allWidthsExpanded.Add(new PartMeasu { ID = width.ID, Measure = width.Measure });
                    }
                }
            }

            // Keep grouping parts until all parts have been considered.
            while (allWidthsExpanded.Count > 0)
            {
                // Find a group of parts that together fit within the total width.
                List<PartMeasu> l = FindClosestSeries(allWidthsExpanded, TotalWidth);

                // Remove the parts that were just grouped from the list of parts to be considered.
                foreach (var selected in l)
                {
                    allWidthsExpanded.Remove(allWidthsExpanded.First(w => w.ID == selected.ID));
                }

                // Add the group to the list of groups.
                ll.Add(l);
            }

            // Store the final groups in FitWidths for further use.
            FitWidths = ll;
            return ll;
        }
        catch (Exception ex)
        {       
            // In case of any errors, throw an exception with a message and the original exception.
            throw new Exception("Failed to Aggregate Width", ex);
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
                List<PartMeasu> l = FindClosestSeries(all_heights, TotalWidth);
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
                List<PartMeasu> l = FindClosestSeries(all_hypotenuse, Hypotenuse);
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
        // This example assumes FitWidths or FitHeights contain lists of parts grouped by best fit
        // Let's say we decide to use FitWidths for horizontal arrangement and FitHeights for vertical if needed

        int currentX = 0, currentY = 0;
        int maxHeightInRow = 0;

        // Example of iterating through FitWidths for horizontal arrangement
        foreach (var group in p.FitWidths)
        {
            foreach (var partMeasu in group)
            {
                ClosetPart part = p.Parts.FirstOrDefault(cp => cp.ID == partMeasu.ID);
                if (part != null)
                {
                    if (currentX + part.Wt > p.TotalWidth)
                    {
                        // Move to the next row
                        currentY += maxHeightInRow;
                        currentX = 0;
                        maxHeightInRow = 0;
                    }

                    // Assuming part X and Y are to be set
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
    }

    static List<PartMeasu> FindClosestSeries(List<PartMeasu> sortedList, int target)
    {
        List<PartMeasu> currentSeries = [];
        List<PartMeasu> bestSeries = [];
        int currentSum = 0;
        int bestSum = 0;

        FindClosestSeriesHelper(sortedList, target, 0, currentSeries, ref currentSum, ref bestSeries, ref bestSum);

        return bestSeries;
    }

    static void FindClosestSeriesHelper(List<PartMeasu> sortedList, int target, int index, List<PartMeasu> currentSeries, ref int currentSum, ref List<PartMeasu> bestSeries, ref int bestSum)
    {
        if (currentSum <= target && currentSum > bestSum)
        {
            bestSum = currentSum;
            bestSeries = new List<PartMeasu>(currentSeries);
        }

        for (int i = index; i < sortedList.Count; i++)
        {
            if (currentSum + sortedList[i].Measure <= target)
            {
                currentSeries.Add(sortedList[i]);
                currentSum += sortedList[i].Measure;

                FindClosestSeriesHelper(sortedList, target, i + 1, currentSeries, ref currentSum, ref bestSeries, ref bestSum);

                currentSum -= sortedList[i].Measure;
                currentSeries.RemoveAt(currentSeries.Count - 1);
            }
            else
            {
                break;  // No need to check further, as the list is sorted
            }
        }
    }

    private void Generate()
    {
        List<ClosetPart> l = [];
        if (Parts.Count > 0)
        {
            int ID = 1;
            foreach (var part in Parts)
            {
                part.ID = ID;
                l.Add(part);
                ID++;
                for (int i = 0; i < part.PartQty - 1; i++)
                {
                    ClosetPart cp = new ();
                    part.CopyAllPropertiesTo(cp);
                    cp.ID = ID++;
                    l.Add(cp);
                }
            }
        }
        l = l.OrderBy(x => x.ID).ToList();
        Parts = l;
    }
}
