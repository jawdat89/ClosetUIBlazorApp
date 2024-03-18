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
                    TotalWidth = 1220,
                    TotalHeight = 2440,
                    BladeThickness = 3,
                    Parts = new()
                    {
                        new () { ID = 1, PartName = "part1", PartWidth = 187, PartHeight = 764, PartQty = 4 },
                        new () { ID = 2, PartName = "part2", PartWidth = 423, PartHeight = 488, PartQty = 2 },
                        new () { ID = 3, PartName = "part3", PartWidth = 414, PartHeight = 575, PartQty = 3 },
                        new () { ID = 4, PartName = "part4", PartWidth = 510, PartHeight = 650, PartQty = 4 },
                        new () { ID = 5, PartName = "part5", PartWidth = 630, PartHeight = 651, PartQty = 5 },
                        new () { ID = 6, PartName = "part6", PartWidth = 350, PartHeight = 450, PartQty = 6 },
                        new () { ID = 7, PartName = "part7", PartWidth = 270, PartHeight = 603, PartQty = 7 },
                        new () { ID = 8, PartName = "part8", PartWidth = 310, PartHeight = 520, PartQty = 8 },
                        new () { ID = 9, PartName = "part9", PartWidth = 398, PartHeight = 967, PartQty = 1 }
                    },
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
