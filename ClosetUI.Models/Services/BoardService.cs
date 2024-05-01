
using ClosetUI.Models.Models;

namespace ClosetUI.Services;

public class BoardService : IBoardService
{
    public async Task<BoardDrawingData> PrepareDrawingData(ParamsModel paramsModel)
    {
        var drawingData = new BoardDrawingData();
        double boardWidth = paramsModel.TotalWidth; // The maximum width a board can have.
        double boardHeight = paramsModel.TotalHeight; // The maximum height a board can have.

        int boardIndex = 1;
        double xPosition = 0, yPosition = 0;
        double currentRowMaxHeight = 0;

        // Initialize the first board.
        Board currentBoard = new Board { BoardIndex = boardIndex };

        // Process each group from FitHeights for optimal vertical fitting.
        foreach (var group in paramsModel.FitHeights)
        {
            foreach (var partMeasure in group)
            {
                ClosetPart part = paramsModel.Parts.FirstOrDefault(p => p.ID == partMeasure.ID);
                if (part == null) continue;

                // Iterate over each part based on its quantity.
                for (int qty = 0; qty < part.PartQty; qty++)
                {
                    // Check if a new row or new board is needed.
                    if (xPosition + part.Wt > boardWidth)
                    {
                        yPosition += currentRowMaxHeight;
                        xPosition = 0;
                        currentRowMaxHeight = 0;
                    }

                    if (yPosition + part.Ht > boardHeight)
                    {
                        drawingData.Boards.Add(currentBoard);
                        boardIndex++;
                        currentBoard = new Board { BoardIndex = boardIndex };
                        xPosition = 0;
                        yPosition = 0;
                        currentRowMaxHeight = 0;
                    }

                    currentBoard.Parts.Add(new PartDrawingInfo
                    {
                        ID = part.ID,
                        X = xPosition,
                        Y = yPosition,
                        Width = part.PartWidth,
                        Height = part.PartHeight,
                        Wt = part.Wt,
                        Ht = part.Ht,
                        Color = GenerateColorBySize(part.Wt)
                    });

                    xPosition += part.Wt;
                    currentRowMaxHeight = Math.Max(currentRowMaxHeight, part.Ht);
                }
            }

            // Reset position for next group.
            xPosition = 0;
            yPosition += currentRowMaxHeight;
            currentRowMaxHeight = 0;
        }

        // Add the last processed board if it contains any parts.
        if (currentBoard.Parts.Count != 0)
        {
            drawingData.Boards.Add(currentBoard);
        }

        return drawingData;
    }

    private static string GenerateColorBySize(double size)
    {
        // Define your size range and corresponding hue range
        double minSize = 10; // Example minimum size
        double maxSize = 1000; // Example maximum size
        int minHue = 0; // Start of hue range (red)
        int maxHue = 260; // End of hue range (blue), avoiding the very bright/pale end

        // Normalize size within its range to a [0, 1] scale
        double normalizedSize = (size - minSize) / (maxSize - minSize);

        // Clamp the normalized size to [0, 1] to ensure it's within the range
        normalizedSize = Math.Max(0, Math.Min(1, normalizedSize));

        // Map the normalized size to the hue range
        int hue = (int)(minHue + (maxHue - minHue) * normalizedSize);

        // Fixed saturation and lightness values to avoid too bright or white colors
        int saturation = 75; // Saturation at 75% to ensure colorfulness
        int lightness = 50; // Lightness at 50% to avoid too dark or too bright colors

        // Return the HSL color string
        return $"hsl({hue}, {saturation}%, {lightness}%)";
    }
}
