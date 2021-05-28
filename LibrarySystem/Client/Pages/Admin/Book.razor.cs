using Blazored.Toast.Services;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibrarySystem.Client.Pages.Admin
{
    public partial class Book
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }


        private Items addBookModel = new();

        private List<Categories> CategoryList;
        private List<Items> BookList;
        private List<Items> BookListKategorisiz;

        RadzenDataGrid<Items> grid;
        RadzenDataGrid<Items> grid2;
        private string bookInfo;

        protected override async Task OnInitializedAsync()
        {
            BookList = await Http.GetFromJsonAsync<List<Items>>("/api/Items/Listele");
            BookListKategorisiz = await Http.GetFromJsonAsync<List<Items>>("/api/Items/ListeleKategorisiz");
            CategoryList = await Http.GetFromJsonAsync<List<Categories>>("/api/Categories/Listele");
            if (CategoryList != null)
            {
                addBookModel.categoryID = CategoryList.FirstOrDefault().categoryID;
            }
        }

        protected async Task OnClickBook()
        {
            var response = await Http.PostAsJsonAsync<Items>($"/api/Items/Ekle", addBookModel);
            Items responseBook = await response.Content.ReadFromJsonAsync<Items>();
            if (responseBook.item != null)
            {
                BookList.Add(responseBook);
                await grid.Reload();
                bookInfo = addBookModel.item + " kitaplar listesine eklendi.";
            }
            else
            {
                bookInfo = "Böyle bir kitap zaten mevcut.";
            }
        }

        public async Task updateBook(int itemID,string item,int categoryID,int kategorisiz)
        {
            Items book = new();
            if (!string.IsNullOrEmpty(item)&&CategoryList.Where(x=>x.categoryID==categoryID).FirstOrDefault()!=null)
            {
                book.itemID = itemID;
                book.item = item;
                book.categoryID = categoryID;
                var response = await Http.PutAsJsonAsync<Items>($"/api/Items/{itemID}", book);
                bool guncellendiMi = await response.Content.ReadFromJsonAsync<bool>();
                if (guncellendiMi == true)
                {
                    ToastService.ShowSuccess("Kitap güncellendi.", "BAŞARILI");
                    //NotificationService.Notify(NotificationSeverity.Success, summary:"Başarılı", detail: "Kategori adı "+category+" olarak değiştirildi.", duration: 4000);
                    if (kategorisiz == 1)
                    {
                        BookList = await Http.GetFromJsonAsync<List<Items>>("/api/Items/Listele");
                        BookListKategorisiz.Remove(BookListKategorisiz.Where(x=>x.itemID==book.itemID).FirstOrDefault());
                        await grid2.Reload();
                    }
                        

                }
                else
                {
                    ToastService.ShowError("Bu kitap adı zaten mevcut.", "Hata");
                    //NotificationService.Notify(NotificationSeverity.Error, summary: "Hata", detail : "Bu kategori zaten mevcut!", duration : 4000 );
                    BookList = await Http.GetFromJsonAsync<List<Items>>("/api/Items/Listele");
                }
            }
            else
            {
                if (kategorisiz == 1)
                {
                    ToastService.ShowError("Kategori seçmediniz!", "Hata");
                }
                else
                {
                    BookList = await Http.GetFromJsonAsync<List<Items>>("/api/Items/Listele");
                    ToastService.ShowError("Kitap adı boş olmamalı.", "Hata");
                    //NotificationService.Notify(NotificationSeverity.Error, summary : "Hata", detail: "Kategori adını boş bırakmayın.", duration: 4000);
                }
            }
        }

        public async Task deleteBook(Items item)
        {
            await Http.DeleteAsync($"/api/Items/{item.itemID}"); // Veri Silmek için DELETE

            BookList.Remove(BookList.Where(x=>x.itemID==item.itemID).FirstOrDefault());
            BookListKategorisiz.Remove(BookListKategorisiz.Where(x => x.itemID == item.itemID).FirstOrDefault());
            await grid.Reload();
            await grid2.Reload();

            ToastService.ShowError("Kitap silindi.", "BAŞARILI");
        }
    }
}
