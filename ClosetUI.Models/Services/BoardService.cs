
using ClosetUI.Models.Models;

namespace ClosetUI.Services;

public class BoardService : IBoardService
{
    public async Task<BoardDrawingData> PrepareDrawingData(ParamsModel paramsModel)
    {
        var drawingData = new BoardDrawingData();
        double boardWidth = paramsModel.TotalWidth; // The maximum width a board can have.
        double boardHeight = paramsModel.TotalHeight; // The maximum height a board can have.

        int boardIndex = 1; // Initialize boardIndex to start with the first board.
        double xPosition = 0, yPosition = 0; // Track the current X and Y position on the board for placing parts.
        double currentRowMaxHeight = 0; // Keep track of the maximum part height in the current row to calculate the starting Y position of the next row.

        // Initialize the first board.
        Board currentBoard = new Board { BoardIndex = boardIndex };

        // Create a deep copy of FitWidths to safely modify it during iteration.
        var fitWidthsCopy = new List<List<PartMeasu>>(paramsModel.FitWidths);

        // Process each row in FitWidths until all have been processed.
        while (fitWidthsCopy.Count > 0)
        {
            var row = fitWidthsCopy.First();
            foreach (var partMeasure in row)
            {
                // Find the part in paramsModel based on partMeasure's ID.
                ClosetPart part = paramsModel.Parts.FirstOrDefault(p => p.ID == partMeasure.ID);
                if (part == null) continue; // Skip this iteration if the part wasn't found.

                // Determine if a new row or new board is needed based on the part's dimensions.
                if (xPosition + part.Wt > boardWidth || yPosition + part.Ht > boardHeight)
                {
                    if (xPosition + part.Wt > boardWidth)
                    {
                        // Start a new row on the current board.
                        yPosition += currentRowMaxHeight;
                        xPosition = 0; // Reset xPosition for the new row
                        currentRowMaxHeight = 0; // Reset the row height tracker
                    }

                    if (yPosition + part.Ht > boardHeight)
                    {
                        // Start a new board if the part exceeds the board height.
                        drawingData.Boards.Add(currentBoard);
                        boardIndex++; // Increment board index for a new board
                        currentBoard = new Board { BoardIndex = boardIndex };
                        xPosition = 0; // Reset positions for the new board
                        yPosition = 0;
                    }
                }

                // Place the part on the current board at the calculated position.
                currentBoard.Parts.Add(new PartDrawingInfo
                {
                    ID = part.ID,
                    X = xPosition,
                    Y = yPosition,
                    Width = part.Wt,
                    Height = part.Ht,
                    Color = GenerateColorBySize(part.Wt)
                });

                // Update position trackers for placing the next part.
                xPosition += part.Wt;
                currentRowMaxHeight = Math.Max(currentRowMaxHeight, part.Ht);
            }

            // Remove the processed row and reset trackers for the next row.
            fitWidthsCopy.RemoveAt(0);
            xPosition = 0;
            yPosition += currentRowMaxHeight;
            currentRowMaxHeight = 0;
        }

        // Add the last processed board to drawing data if it contains any parts.
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
