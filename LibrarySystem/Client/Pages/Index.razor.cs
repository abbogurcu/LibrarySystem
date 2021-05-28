using LibrarySystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ZXingBlazor.Components;

namespace LibrarySystem.Client.Pages
{
    [Authorize]
    public partial class Index
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJSRuntime jsr { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        private IEnumerable<ItemHistories> ItemList;
        private IEnumerable<Categories> CategoryList;
        private IEnumerable<Items> BookList;
        private int selectedCategoryID,selectedItemID;
        private string itemDdlText, categoryDdlText;
        private bool btnStatus = true;

        string userdata = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            userdata = await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false);
            

            if (!string.IsNullOrWhiteSpace(userdata))
            {
                ItemList = await Http.GetFromJsonAsync<IEnumerable<ItemHistories>>("/api/ItemHistories/Listele");

                CategoryList = await Http.GetFromJsonAsync<IEnumerable<Categories>>("/api/Categories/Listele");

                if (CategoryList != null)
                {
                    selectedCategoryID = CategoryList.FirstOrDefault().categoryID;
                    categoryDdlText = null;
                    btnStatus = false;
                    await OnChange(selectedCategoryID);
                }
                else
                {
                    categoryDdlText = "Seçebileceğiniz kategori bulunmamaktadır.";
                    selectedItemID = -1;
                    btnStatus = true;
                    itemDdlText = "Seçebileceğiniz kitap bulunmamaktadır.";
                }
            }
            else
            {
                NavigationManager.NavigateTo("/Login");
            }
        }
        protected async Task OnChange(object value)
        {
            BookList=await Http.GetFromJsonAsync<IEnumerable<Items>>("/api/Items/ListeleKategoriden/"+value.ToString());
            if (BookList.FirstOrDefault() != null)
            {
                selectedItemID = BookList.FirstOrDefault().itemID;
                btnStatus = false;
                itemDdlText = null;
            }
            else
            {
                selectedItemID = -1;
                btnStatus = true;
                itemDdlText = "Seçebileceğiniz kitap bulunmamaktadır.";
            }
        }

        protected async Task OnClick()
        {
            if (selectedItemID != -1)
            {
                ItemHistories ItemData = new();
                ItemData.itemID = selectedItemID;
                ItemData.userID = 1;
                ItemData.borrowTime = DateTime.Now;
                ItemData.deliverTime = DateTime.Now.AddDays(30);
                await Http.PostAsJsonAsync<ItemHistories>("/api/ItemHistories", ItemData);
                ItemList = await Http.GetFromJsonAsync<IEnumerable<ItemHistories>>("/api/ItemHistories/Listele");
                await OnChange(selectedCategoryID);
            }
        }
    }
}
