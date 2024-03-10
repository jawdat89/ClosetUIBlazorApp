namespace ClosetUI.Models.Models;

public class Processing
{
    public static void Process(ParamsModel p)
    {
        p.Ceed();
        p.CalcHypotenuse();
        p.Parts.ForEach(x => x.AddBladeThickness(p.BladeThikness));
        p.Parts.ForEach(x => x.CalcHypotenuse());
        p.FillAllWidths();
        p.FillAllHeights();
        p.FillAllHypotenuse();
        p.AggrigWidth();
        p.AggrigHeight();
        p.AggrigHypotenuse();
    }

    public static List<PartMeasu> FindClosestSeries(List<PartMeasu> sortedList, int target)
    {
        List<PartMeasu> currentSeries = new List<PartMeasu>();
        List<PartMeasu> bestSeries = new List<PartMeasu>();
        int currentSum = 0;
        int bestSum = 0;

        FindClosestSeriesUTIL(sortedList, target, 0, currentSeries, ref currentSum, ref bestSeries, ref bestSum);
        return bestSeries;
    }

    public static void FindClosestSeriesUTIL(List<PartMeasu> sortedList, int target, int index, List<PartMeasu> currentSeries, ref int currentSum, ref List<PartMeasu> bestSeries, ref int bestSum)
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

                FindClosestSeriesUTIL(sortedList, target, i + 1, currentSeries, ref currentSum, ref bestSeries, ref bestSum);

                currentSum -= sortedList[i].Measure;
                currentSeries.RemoveAt(currentSeries.Count - 1);
            }
            else
            {
                break;  // No need to check further, as the list is sorted
            }
        }
    }
}
