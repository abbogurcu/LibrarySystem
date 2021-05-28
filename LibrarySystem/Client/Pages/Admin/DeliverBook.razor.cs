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
    public partial class DeliverBook
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        private List<ItemHistories> ihList;

        RadzenDataGrid<ItemHistories> grid;

        bool ShowScanBarcode { get; set; } = false;
        public string? BarCode { get; set; }
        public string? NameSurname { get; set; }
        public string? BookName { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ihList = await Http.GetFromJsonAsync<List<ItemHistories>>("/api/ItemHistories/ListeleTeslim");
        }

        protected async Task OnDeliverBarcode(string value)
        {
            Console.WriteLine(value);
            if (!string.IsNullOrEmpty(value)&&value.All(Char.IsDigit))
            {
                if (ihList.Where(x => x.itemID == Convert.ToInt32(value)).FirstOrDefault() != null)
                {
                    BarCode = value;
                    ItemHistories item = ihList.Where(x => x.itemID == Convert.ToInt32(value)).FirstOrDefault();
                    NameSurname = "İsim Soyisim : " + item.name + " " + item.surname;
                    BookName= "Kitap Adı : " + item.itemName;
                    var response = await Http.PostAsJsonAsync<ItemHistories>($"/api/ItemHistories/Teslim", item);
                    bool status = await response.Content.ReadFromJsonAsync<bool>();
                    if (status == true)
                    {
                        ToastService.ShowSuccess("Kitap teslim alındı.", "Başarılı");
                    }
                    else
                    {
                        ToastService.ShowError("Kitap zaten teslim alınmış.", "Hata");
                    }

                    ihList.Remove(item);
                    await grid.Reload();
                }
                else
                {
                    ToastService.ShowError("Teslim edilecek böyle bir kitap bulunmamaktadır.", "Hata");
                }
            }
        }

        protected async Task deliverBook(ItemHistories item)
        {
            var response = await Http.PostAsJsonAsync<ItemHistories>($"/api/ItemHistories/Teslim", item);
            bool status = await response.Content.ReadFromJsonAsync<bool>();
            if (status == true)
            {
                ToastService.ShowSuccess("Kitap teslim alındı.", "Başarılı");
            }
            else
            {
                ToastService.ShowError("Kitap zaten teslim alınmış.", "Hata");
            }
            ihList.Remove(item);
            await grid.Reload();
        }
    }
}
