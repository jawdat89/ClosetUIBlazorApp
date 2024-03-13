using ClosetUI.Models.Dtos;
using ClosetUI.Models.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using ClosetUI.Services;

namespace ClosetUI.Components.Pages
{
    public partial class PartsGeneratorBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IPartCalculationService PartCalculationService { get; set; }
        //[Inject]
        //public IManageParamsLocalStorageService ManageParamsLocalStorageService { get; set; }

        public PartGeneratorDto Params { get; set; }

        public string ErrorMessage { get; set; }

        [Parameter]
        public int LastIndex { get; set; } = -1;

        protected ParamsModel paramsResult { get; set; }

        protected bool invalidForm = false;
        protected ToastMessage ToastMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Params = new PartGeneratorDto
                {
                    TotalWidth = 2200,
                    TotalHeight = 4400,
                    BladeThickness = 3,
                    Parts = [new PartInput()],
                    Direction = 1,
                    HypotenuseType = 1
                };

                UpdateParamsPartsList(Params.Parts);

                paramsResult = new ParamsModel();

                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        //protected async override Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        // Clean the localStorage
        //        await ManageParamsLocalStorageService.RemoveCollection();
        //    }
        //}

        protected async Task Calculate_Click(PartGeneratorDto partGeneratorDto)
        {
            try
            {
                CheckFormValidity();
                if (!invalidForm)
                {
                    paramsResult = await PartCalculationService.ProcessAsync(partGeneratorDto);

                    //// Save jsonData to local storage
                    //await ManageParamsLocalStorageService.AddCollection(paramsResult);

                    // Serialize paramsResult to JSON
                    var jsonData = JsonSerializer.Serialize(paramsResult);

                    // Navigate to the new page
                    NavigationManager.NavigateTo($"/PartsRenderer?val={jsonData}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                StateHasChanged();
            }
        }

        protected async Task AddPart(PartInput partInput)
        {
            var newPart = new PartInput();
            newPart.PartName = partInput.PartName;
            newPart.PartWidth = partInput.PartWidth;
            newPart.PartHeight = partInput.PartHeight;
            newPart.PartQty = partInput.PartQty;

            // Add Input Part to the list
            Params.Parts.Append(newPart);

            // Add a new line
            var partsList = Params.Parts.ToList();
            partsList.Add(new PartInput());
            Params.Parts = partsList;

            // Update PartList
            await Task.Delay(50);
            UpdateParamsPartsList(Params.Parts);
            //await InvokeAsync(StateHasChanged); // Request UI refresh
            StateHasChanged();
        }

        protected async Task DeletePart(PartInput partInput)
        {
            var newParts = Params.Parts.Where(p => p.Id != partInput.Id).ToList();

            Params.Parts = newParts;

            // Update PartList
            UpdateParamsPartsList(newParts);

            await Task.Delay(50);
            StateHasChanged();
        }

        private void UpdateParamsPartsList(IEnumerable<PartInput> partInputs)
        {
            Params.Parts = new List<PartInput>(partInputs);
            LastIndex = partInputs.Count() - 1;
        }

        protected bool CheckIfDeleteDisabled()
        {
            if (Params.Parts.Count() <= 1)
                return true;
            return false;
        }

        private void CheckFormValidity()
        {
            if (Params.Parts.Count <= 1)
            {
                CreateToastMessage(ToastType.Warning, "No Parts", "Please fill in parts");
                invalidForm = true;
            } 
            else 
            { 
                invalidForm = false;
            }
            foreach (var part in Params.Parts)
            {
                if (string.IsNullOrWhiteSpace(part.PartName))
                {
                    ToastMessage = CreateToastMessage(ToastType.Warning, "Invalid Parts", "Please fill in part names");
                    invalidForm = true;
                }
                else
                {
                    invalidForm = false;
                }
            }
        }

        protected void OnPartChange()
        {
            CheckFormValidity();
        }

        protected void ResetValidity()
        {
            ToastMessage = new();
            invalidForm = false;
        }

        protected ToastMessage CreateToastMessage(ToastType toastType, string title, string message)
            => new ToastMessage
            {
                Type = toastType,
                Title = title,
                HelpText = $"{DateTime.Now}",
                Message = $"{message}. DateTime: {DateTime.Now}",
            };
    }
}
