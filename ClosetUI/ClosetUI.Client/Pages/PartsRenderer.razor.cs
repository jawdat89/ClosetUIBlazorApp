using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using ClosetUI.Models.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Drawing;

namespace ClosetUI.Client.Pages;

public partial class PartsRenderer : ComponentBase
{
    private Size Size = new Size();
    private double FPS;
    private double ScaleFactor;

    private double GlobalX = 0;
    private double GlobalY = 0;
    private bool isDragging = false;
    private CanvasMouseArgs lastDragPosition;

    protected Canvas2DContext Ctx;
    protected BECanvasComponent CanvasReference;
    protected CanvasHelper CanvasHelper;

    const string JsModulePath = "./Pages/PartsRenderer.razor.js";

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    //[Inject]
    //public ILogger<PartsRenderer> Log { get; set; }

    //[Inject]
    //public IManageParamsLocalStorageService ManageParamsLocalStorage { get; set; }

    Lazy<Task<IJSObjectReference>> moduleTask;

    public ParamsModel? ParamsResult { get; set; }

    public string? ParamsJson { get; set; }

    public string? ErrorMessage { get; set; }

    // [Parameter]
    [SupplyParameterFromQuery(Name = "val")]
    public string? EncodedParams { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await Task.Delay(0);
        if (!string.IsNullOrWhiteSpace(EncodedParams))
        {
            try
            {
                // URL-decode the JSON string
                var decodedJson = System.Net.WebUtility.UrlDecode(EncodedParams);

                // Deserialize the JSON string to ParamsModel
                ParamsResult = JsonSerializer.Deserialize<ParamsModel>(decodedJson);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deserializing parameters: {ex.Message}";
            }
        }
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath).AsTask());

            //if (ParamsResult == null)
            //{
            //    ParamsResult = await ManageParamsLocalStorage.GetCollection();
            //}


            var module = await moduleTask.Value;

            // BeCanvas
            Ctx = await CanvasReference.CreateCanvas2DAsync();
            // Initialize the helper
            await CanvasHelper.Initialize();

            // If you need to apply a transformation such as zooming
            ScaleFactor = 0.5; // Adjust as needed
            await Ctx.SetTransformAsync(ScaleFactor, 0, 0, ScaleFactor, 0, 0);
        }
    }


    // BeCanvas Method
    public async Task RenderFrame(double fps)
    {
        FPS = fps;

        // Reset transformations before clearing to ensure the entire canvas is cleared
        await Ctx.SetTransformAsync(1, 0, 0, 1, 0, 0); // Set to identity matrix
        await Ctx.ClearRectAsync(0, 0, Size.Width, Size.Height);

        // Reapply transformations after clearing
        await Ctx.SetTransformAsync(ScaleFactor, 0, 0, ScaleFactor, GlobalX, GlobalY);

        // Drawing the parts
        if (ParamsResult != null)
        {
            await DrawBoardsAndPartsAsync();
        }
    }

    public void CanvasResized(Size size)
    {
        Size = size;
    }

    // #region Mouse Methods
    void MouseDown(CanvasMouseArgs args)
    {
        // Start dragging
        isDragging = true;
        lastDragPosition = args; // Store the starting point of the drag
    }

    async Task MouseUp(CanvasMouseArgs args)
    {
        // End dragging
        isDragging = false;

        // Clear the entire canvas before drawing the new frame
        await Ctx.ClearRectAsync(0, 0, Size.Width, Size.Height);
    }

    void MouseMove(CanvasMouseArgs args)
    {
        if (isDragging)
        {
            // Calculate how much the mouse has moved since the last event
            double deltaX = args.ClientX - lastDragPosition.ClientX;
            double deltaY = args.ClientY - lastDragPosition.ClientY;

            // Update the global translation offsets
            GlobalX += deltaX;
            GlobalY += deltaY;

            // Update the last drag position
            lastDragPosition = args;
        }
    }

    void MouseWheel(CanvasWheelArgs args)
    {
        var deltaScaleFactor = args.DeltaY < 0 ? 0.1 : -0.1;
        ScaleFactor += deltaScaleFactor;
        ScaleFactor = Math.Max(0.1, Math.Min(ScaleFactor, 10)); // Clamping
    }
    // #endregion Mouse Methods

    // #region Drawers
    // Revised Method to Draw All Parts
    private async Task DrawBoardsAndPartsAsync()
    {
        double currentX = 0, currentY = 0, maxHeightInRow = 0, boardOffsetX = 0;
        var parts = ParamsResult.Parts;
        double boardWidth = ParamsResult.TotalWidth;
        double boardHeight = ParamsResult.TotalHeight;

        // Loop through each part
        foreach (var part in parts)
        {
            for (int qty = 0; qty < part.PartQty; qty++)
            {
                if (currentX + part.Wt > boardWidth)
                {
                    currentY += maxHeightInRow; // Move to next row
                    currentX = 0;
                    maxHeightInRow = 0;
                }

                if (currentY + part.Ht > boardHeight)
                {
                    boardOffsetX += boardWidth + 10; // Start new board
                    currentX = 0;
                    currentY = 0;
                    maxHeightInRow = 0;
                }

                string color = GenerateColorBySize(part.Wt);
                await DrawPartAsync(currentX + boardOffsetX, currentY, color, part.Wt, part.Ht, part);

                currentX += part.Wt;
                maxHeightInRow = Math.Max(maxHeightInRow, part.Ht);
            }
        }

        // Draw board outlines after all parts are drawn for visual clarity
        double boardsCount = Math.Ceiling((currentY + maxHeightInRow) / boardHeight);
        for (int i = 0; i < boardsCount; i++)
        {
            await DrawBoardDimensionsAsync(i * (boardWidth + 10), 0, boardWidth, boardHeight);
        }
    }

    // Modified to accept a ClosetPart parameter and match your provided JS function signature
    private async Task DrawPartAsync(double currentX, double currentY, string color, double width, double height, ClosetPart part)
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
        string text = $"{part.PartWidth}mm x {part.PartHeight}mm";

        // Estimate the text width (may need adjustment based on the chosen font)
        double textWidth = fontSize * text.Length / 2; // Rough approximation

        // Calculate the position to center the text within the part
        double textX = currentX + (width - textWidth) / 2;
        double textY = currentY + (height + fontSize) / 2; // Adjust for vertical centering

        // Draw the text
        await Ctx.FillTextAsync(text, textX, textY);
    }

    private async Task DrawBoardDimensionsAsync(double offsetX, double offsetY, double width, double height)
    {
        await Ctx.SetStrokeStyleAsync("#3C3732FF"); // Outline color
        await Ctx.SetLineWidthAsync(2); // Line width
        await Ctx.StrokeRectAsync(offsetX, offsetY, width, height);
    }

    private string GenerateColorBySize(double size)
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
    // #endregion Drawers
}