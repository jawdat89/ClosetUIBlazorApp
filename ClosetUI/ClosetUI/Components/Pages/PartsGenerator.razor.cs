using ClosetUI.Models.Dtos;
using ClosetUI.Models.Models;
using Microsoft.AspNetCore.Components;
using ClosetUI.Services;

namespace ClosetUI.Components.Pages
{
    public partial class PartsGeneratorBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IPartCalculationService PartCalculationService { get; set; }

        public PartGeneratorDto Params { get; set; }

        public string ErrorMessage { get; set; }

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
                    TotalWidth = 1220,
                    TotalHeight = 2440,
                    BladeThickness = 3,
                    Parts = [new ()],
                    Direction = 1,
                    HypotenuseType = 2
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

        protected async Task Calculate_Click(PartGeneratorDto partGeneratorDto)
        {
            try
            {
                CheckFormValidity();
                if (!invalidForm)
                {
                    paramsResult = await PartCalculationService.ProcessAsync(partGeneratorDto);

                    if (paramsResult != null)
                    {
                        // Navigate to the new page
                        NavigationManager.NavigateTo("PartsRenderer");
                    }
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
            PartInput newPart = new()
            {
                PartName = partInput.PartName,
                PartWidth = partInput.PartWidth,
                PartHeight = partInput.PartHeight,
                PartQty = partInput.PartQty
            };


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
            var newParts = Params.Parts.Where(p => p.ID != partInput.ID).ToList();

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
            if (Params.Parts.Count <= 1)
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
