using ClosetUI.Models.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ClosetUI.Client.Pages;

public partial class PartsRenderer : ComponentBase
{
    const string JsModulePath = "./Pages/PartsRenderer.razor.js";
    const string CanvasId = "partsCanvas";

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    //[Inject]
    //public IManageParamsLocalStorageService ManageParamsLocalStorage { get; set; }

    Lazy<Task<IJSObjectReference>> moduleTask;

    public ElementReference CanvasReference { get; set; }

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

            // Set canvas size based on screen width
            await module.InvokeVoidAsync("setCanvasSize", CanvasId, DotNetObjectReference.Create(this));

            await module.InvokeVoidAsync("drawParts", CanvasId, ParamsResult, DotNetObjectReference.Create(this));
            StateHasChanged();
        }
    }

    [JSInvokable]
    public void OnCanvasError(string message)
    {
        Console.WriteLine($"Error from JS: {message}");
        ErrorMessage = message;
    }
}