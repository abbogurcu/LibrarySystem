using Blazored.Toast.Services;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibrarySystem.Client.Pages
{
    public partial class Login
	{
		[Inject]
		public HttpClient Http { get; set; }

		[Inject]
		public IToastService ToastService { get; set; }

		[Inject]
		public IJSRuntime jsr { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public NotificationService NotificationService { get; set; }

		LoginModel user = new();
		string message = string.Empty;
		bool isDisabled = false;

		private async Task OnValid()
		{
			isDisabled = true;
			using (var msg = await Http.PostAsJsonAsync<LoginModel>($"/api/Users/", user, System.Threading.CancellationToken.None))
			{
				if (msg.IsSuccessStatusCode)
				{
					LoginResult result = await msg.Content.ReadFromJsonAsync<LoginResult>();
					message = result.message;
					isDisabled = false;
					if (result.success)
					{
						await jsr.InvokeVoidAsync("localStorage.setItem", "user", $"{result.username};{result.jwtBearer}").ConfigureAwait(false);
						ToastService.ShowSuccess(result.message, "Başarılı");
						NavigationManager.NavigateTo("/");
					}
					else
                    {
						ToastService.ShowError(result.message, "Hata");
                    }
				}
			}
		}
	}
}
