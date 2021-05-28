using Blazored.Toast.Services;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace LibrarySystem.Client.Pages.Admin
{
    public partial class Category
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        private Categories addCategoryModel = new();

        private List<Categories> CategoryList;

        private string categoryInfo;

        RadzenDataGrid<Categories> grid;

        protected override async Task OnInitializedAsync()
        {
            CategoryList = await Http.GetFromJsonAsync<List<Categories>>("/api/Categories/Listele");
        }

        protected async Task OnClickCategory()
        {
            var response = await Http.PostAsJsonAsync<Categories>($"/api/Categories/Ekle", addCategoryModel);
            Categories responseCategory = await response.Content.ReadFromJsonAsync<Categories>();
            if (responseCategory.category != null)
            {
                CategoryList.Add(responseCategory);
                await grid.Reload();
                categoryInfo = addCategoryModel.category + " kategoriler listesine eklendi.";
            }
            else
            {
                categoryInfo = "Böyle bir kategori zaten mevcut.";
            }
        }

        public async Task updateCategory(int categoryID, string category)
        {
            Categories cat = new();
            if (!string.IsNullOrEmpty(category))
            {             
                cat.category = category;
                cat.categoryID = categoryID;
                var response = await Http.PutAsJsonAsync<Categories>($"/api/Categories/{categoryID}", cat);
                bool guncellendiMi = await response.Content.ReadFromJsonAsync<bool>();
                if (guncellendiMi == true)
                {
                    ToastService.ShowSuccess("Kategori adı "+category+" olarak güncellendi.", "BAŞARILI");
                    //NotificationService.Notify(NotificationSeverity.Success, summary:"Başarılı", detail: "Kategori adı "+category+" olarak değiştirildi.", duration: 4000);

                }
                else
                {
                    ToastService.ShowError("Bu kategori zaten mevcut.", "Hata");
                    //NotificationService.Notify(NotificationSeverity.Error, summary: "Hata", detail : "Bu kategori zaten mevcut!", duration : 4000 );
                    CategoryList = await Http.GetFromJsonAsync<List<Categories>>("/api/Categories/Listele");
                }
            }
            else
            {
                CategoryList = await Http.GetFromJsonAsync<List<Categories>>("/api/Categories/Listele");
                ToastService.ShowError("Kategori adı boş olmamalı.", "Hata");
                //NotificationService.Notify(NotificationSeverity.Error, summary : "Hata", detail: "Kategori adını boş bırakmayın.", duration: 4000);
            }
        }

        public async Task deleteCategory(Categories cat)
        {
            await Http.DeleteAsync($"/api/Categories/{cat.categoryID}"); // Veri Silmek için DELETE

            CategoryList.Remove(cat);
            await grid.Reload();

            ToastService.ShowError("Kitap silindi.", "BAŞARILI");
        }
    }
}
