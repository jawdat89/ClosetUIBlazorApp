using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using ClosetUI.Models.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Drawing;
using ClosetUI.Models.Services;
using ClosetUI.Services;
using ClosetUI.Models.Services.Interfaces;

namespace ClosetUI.Client.Pages;

public partial class PartsRenderer : ComponentBase
{
    private Size _size = new();
    private double _fPS;
    private double _scaleFactor;

    private double _globalX = 350;
    private double _globalY = 50;
    private bool _isDragging = false;
    private CanvasMouseArgs _lastDragPosition;

    private const double InitialXOffset = 0; // Initial offset from the left edge
    private const double InitialYOffset = 0; // Initial offset from the top edge
    private const double BoardGap = 55; // Gap between each board

    protected Canvas2DContext Ctx;
    protected BECanvasComponent CanvasReference;
    protected CanvasHelper CanvasHelper;

    [Inject]
    protected IPartCalculationService ParamsClientService {  get; set; }

    [Inject]
    protected IBoardDrawingService BoardDrawingService { get; set; }

    protected ParamsModel? ParamsResult { get; set; }

    protected BoardDrawingData DrawingData { get; set; }

    protected string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await ParamsClientService.GetParams();
            if (result != null)
            {
                ParamsResult = result;
                DrawingData = await BoardDrawingService.PrepareDrawingData(ParamsResult);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // BeCanvas
            Ctx = await CanvasReference.CreateCanvas2DAsync();
            // Initialize the helper
            if (CanvasHelper != null)
            {
                await CanvasHelper.Initialize();
            }

            // If you need to apply a transformation such as zooming
            _scaleFactor = 0.35; // Adjust as needed
            if (Ctx != null)
            {
                await Ctx.SetTransformAsync(_scaleFactor, 0, 0, _scaleFactor, 0, 0);
            }
        }
    }


    // BeCanvas Method
    public async Task RenderFrame(double fps)
    {
        _fPS = fps;

        // Reset transformations before clearing to ensure the entire canvas is cleared
        await Ctx.SetTransformAsync(1, 0, 0, 1, 0, 0); // Set to identity matrix
        await Ctx.ClearRectAsync(0, 0, _size.Width, _size.Height);

        // Reapply transformations after clearing
        await Ctx.SetTransformAsync(_scaleFactor, 0, 0, _scaleFactor, _globalX, _globalY);

        // Drawing the parts
        if (ParamsResult != null)
        {
            await DrawBoardsAndPartsAsync();
        }
    }

    public void CanvasResized(Size size)
    {
        _size = size;
    }

    // #region Mouse Methods
    void MouseDown(CanvasMouseArgs args)
    {
        // Start dragging
        _isDragging = true;
        _lastDragPosition = args; // Store the starting point of the drag
    }

    async Task MouseUp(CanvasMouseArgs args)
    {
        // End dragging
        _isDragging = false;

        // Clear the entire canvas before drawing the new frame
        await Ctx.ClearRectAsync(0, 0, _size.Width, _size.Height);
    }

    void MouseMove(CanvasMouseArgs args)
    {
        if (_isDragging)
        {
            // Calculate how much the mouse has moved since the last event
            double deltaX = args.ClientX - _lastDragPosition.ClientX;
            double deltaY = args.ClientY - _lastDragPosition.ClientY;

            // Update the global translation offsets
            _globalX += deltaX;
            _globalY += deltaY;

            // Update the last drag position
            _lastDragPosition = args;
        }
    }

    void MouseWheel(CanvasWheelArgs args)
    {
        var deltaScaleFactor = args.DeltaY < 0 ? 0.1 : -0.1;
        _scaleFactor += deltaScaleFactor;
        _scaleFactor = Math.Max(0.1, Math.Min(_scaleFactor, 10)); // Clamping
    }

    protected async Task SaveCanvasAsImageAsync()
    {
        await CanvasHelper.downloadCanvasAsImage();
    }
    // #endregion Mouse Methods

    // #region Drawers
    // Revised Method to Draw All Parts
    protected async Task DrawBoardsAndPartsAsync()
    {
        if (DrawingData?.Boards == null) return;

        double currentYOffset = InitialYOffset;

        // Iterate through each board in the drawing data
        for (int i = 0; i < DrawingData.Boards.Count; i++)
        {
            var board = DrawingData.Boards[i];
            double currentXOffset = InitialXOffset + (BoardGap + ParamsResult.TotalWidth) * i;

            // Draw each part in the current board with adjusted positions
            foreach (var part in board.Parts)
            {
                await DrawPartAsync(part.X + currentXOffset, part.Y + currentYOffset, part.Color, part.Width, part.Height, part);
            }

            await DrawBoardDimensionsAsync(currentXOffset, 0, ParamsResult.TotalWidth, ParamsResult.TotalHeight, board.BoardIndex);
        }
    }

    private async Task DrawPartAsync(double currentX, double currentY, string color, double width, double height, PartDrawingInfo part)
    {
        // Fill the part with the specified color
        await Ctx.SetFillStyleAsync(color);
        await Ctx.FillRectAsync(currentX, currentY, width, height);
        await Ctx.SetStrokeStyleAsync("black");
        await Ctx.SetLineWidthAsync(1);
        await Ctx.StrokeRectAsync(currentX, currentY, width, height);

        // Calculate an appropriate font size based on the smallest dimension of the part
        int fontSize = 28;
        string font = $"{fontSize}px Arial";

        // Set the text properties
        await Ctx.SetFillStyleAsync("black");
        await Ctx.SetFontAsync(font);

        // Prepare the text you want to draw
        string text = $"{part.Width}mm x {part.Height}mm";

        if (width < 200)
        {
            // Adjustments for multi-line text
            int lineHeight = fontSize + 4; // 4 is a line spacing value, adjust as needed
            string[] words = text.Split('x');
            string line1 = $"{words[0]}";
            string line2 = $"x {words[1]}";

            // Calculate the X position to center the text within the part
            double textX = currentX + (width / 2) + 50; // Center the text

            // Draw the first line
            double textY1 = currentY + (height / 2) - lineHeight;
            await Ctx.FillTextAsync(line1, textX, textY1);

            // Draw the second line
            double textY2 = currentY + (height / 2);
            await Ctx.FillTextAsync(line2, textX, textY2);
        }
        else
        {
            // Estimate the text width (may need adjustment based on the chosen font)
            double textWidth = fontSize * text.Length / 2; // Rough approximation

            // Calculate the position to center the text within the part
            double textX = currentX + (width + textWidth) / 2;
            double textY = currentY + (height + fontSize) / 2; // Adjust for vertical centering

            // Draw the text
            await Ctx.FillTextAsync(text, textX, textY);
        }
    }

    private async Task DrawBoardDimensionsAsync(double offsetX, double offsetY, double width, double height, int boardIndex)
    {
        // Draw the board dimensions as before
        await Ctx.SetStrokeStyleAsync("#3C3732FF"); // Outline color
        await Ctx.SetLineWidthAsync(2); // Line width
        await Ctx.StrokeRectAsync(offsetX, offsetY, width, height);

        // Prepare the board index text
        string boardText = $"Board {boardIndex}";
        int fontSize = 80; // Choose an appropriate font size
        await Ctx.SetFontAsync($"{fontSize}px Arial");
        await Ctx.SetFillStyleAsync("black");

        // Calculate text position to place it above the board dimensions
        double textX = offsetX + 280; // A little padding from the left edge of the board
        double textY = offsetY - 20; // A little space above the board

        // Draw the text
        await Ctx.FillTextAsync(boardText, textX, textY);
    }
    // #endregion Drawers
}