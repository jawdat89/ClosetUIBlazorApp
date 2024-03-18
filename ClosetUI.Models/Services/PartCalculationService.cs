using ClosetUI.Models.Models;
using ClosetUI.Models.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClosetUI.Services;

public class PartCalculationService : IPartCalculationService
{
    private ParamsModel? _params { get; set; }

    public async Task<ParamsModel> GetParams()
    {
        return _params;
    }
    public async Task<ParamsModel> ProcessAsync(PartGeneratorDto parameters)
    {
        try
        {
            // Convert DTO to domain model (ParamsModel)
            var paramsObj = ConvertToParams(parameters);

            // Ensure parts are processed (e.g., blade thickness applied, hypotenuse calculated)
            foreach (var part in paramsObj.Parts)
            {
                part.AddBladeThickness(paramsObj.BladeThickness);
                part.CalcHypotenuse();
            }

            // Run fitting logic to organize parts
            await paramsObj.Fitting(); // Assuming Fitting is async; if not, just call without await

            // At this point, paramsObj contains organized parts; 
            // you can optionally prepare drawing data or return paramsObj for further processing
            _params = paramsObj; // Storing processed params for other uses if needed

            return paramsObj;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to process calculations: " + ex.Message);
        }
    }

    private ParamsModel ConvertToParams(PartGeneratorDto dto)
    {
        var closetParts = ConvertToClosetParts(dto.Parts);


        return new ParamsModel
        {
            BladeThickness = dto.BladeThickness,
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
            ClosetPart closetPart = new()
            {
                ID = part.ID,
                PartName = part.PartName,
                PartWidth = part.PartWidth,
                PartHeight = part.PartHeight,
                PartQty = part.PartQty,
            };

            partsList.Add(closetPart);
        }

        return partsList;
    }

    public async Task<byte[]> GenerateAndDownloadPdf(ParamsModel paramsModel)
    {
        var pdfData = await PreparePDFData(paramsModel);

        var document = Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(TextStyle.Default.FontSize(12));
                page.Header()
                    .PaddingBottom(20)
                    .Background(Colors.Grey.Lighten1)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text($"{pdfData.Title}")
                    .FontSize(20)
                    .SemiBold();

                page.Content().Padding(10).Column(column =>
                {
                    int partIndex = 0;

                    foreach (var board in pdfData.Boards)
                    {
                        if (board.Parts.Count > 0)
                        {
                            column.Item().Element(element =>
                            {
                                element.Row(row =>
                                {
                                    row.RelativeItem().Text($"Board {board.BoardIndex}").Bold().FontSize(16);
                                });
                            });

                            foreach (var part in board.Parts)
                            {
                                // Include the row number in the part's text
                                column.Item().Padding(2).Text($"Row {part.RowNumber}, Part {part.ID}: {part.Dimensions}, Position: {part.Position}");
                            }

                            partIndex++;

                            if (partIndex == board.Parts.Count + 1)
                            {
                                column.Item().PageBreak();
                                partIndex = 0;
                            } 
                            else
                            {
                                column.Item().Padding(10);
                            }
                        } 
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(18));
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static async Task<PDFData> PreparePDFData(ParamsModel paramsModel)
    {
        var pdfData = new PDFData
        {
            Title = "Board Layout",
            Boards = []
        };

        var boardWidth = paramsModel.TotalWidth;
        var boardHeight = paramsModel.TotalHeight;
        int boardIndex = 1;
        double xPosition = 0, yPosition = 0;
        double currentRowMaxHeight = 0;

        var fitWidthsCopy = new List<List<PartMeasu>>(paramsModel.FitWidths);
        int rowNumber = 1; // Initialize row number for the first row.

        while (fitWidthsCopy.Count > 0)
        {
            var row = fitWidthsCopy.First();
            BoardPDFInfo boardPDFInfo = new()
            {
                BoardIndex = boardIndex,
                Parts = []
            };

            foreach (var partMeasure in row)
            {
                var part = paramsModel.Parts.FirstOrDefault(p => p.ID == partMeasure.ID);
                if (part == null) continue;

                // Check if a new row or board is needed due to size constraints.
                if (xPosition + part.Wt > boardWidth)
                {
                    // Start a new row within the current board.
                    yPosition += currentRowMaxHeight;
                    xPosition = 0;
                    currentRowMaxHeight = 0;
                    rowNumber++; // Increment the row number within the current board.
                }

                if (yPosition + part.Ht > boardHeight)
                {
                    // Save the current board and start a new one.
                    pdfData.Boards.Add(boardPDFInfo);
                    boardIndex++;
                    boardPDFInfo = new BoardPDFInfo
                    {
                        BoardIndex = boardIndex,
                        Parts = []
                    };
                    xPosition = 0;
                    yPosition = 0;
                    currentRowMaxHeight = 0;
                    rowNumber = 1; // Reset row number for the new board.
                }

                var partPDFInfo = new PartPDFInfo
                {
                    ID = part.ID,
                    Dimensions = $"{part.Wt}mm x {part.Ht}mm",
                    Position = $"X: {xPosition}mm, Y: {yPosition}mm",
                    RowNumber = rowNumber
                };

                boardPDFInfo.Parts.Add(partPDFInfo);
                xPosition += part.Wt;
                currentRowMaxHeight = Math.Max(currentRowMaxHeight, part.Ht);
            }

            // After processing a row, prepare for the next one.
            fitWidthsCopy.RemoveAt(0);
            if (xPosition > 0) // Only adjust if the last row was partially filled.
            {
                xPosition = 0;
                yPosition += currentRowMaxHeight;
                currentRowMaxHeight = 0;
                rowNumber++;
            }

            if (boardPDFInfo.Parts.Count > 0 && !pdfData.Boards.Contains(boardPDFInfo))
            {
                pdfData.Boards.Add(boardPDFInfo);
            }
        }

        await Task.CompletedTask;
        return pdfData;
    }
}
