using LibrarySystem.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibrarySystem.Client.Pages
{
    public partial class Appointment
    {
        [Inject]
        public HttpClient Http { get; set; }

        private List<Rooms> RoomList;
        private List<Hours> HourList;
        private List<Appointments> AppoList;
        private DateTime selectedDate;
        private int selectedHourID,selectedRoomID;
        public string selectedRoom,addedAppoRoom;

        protected override async Task OnInitializedAsync()
        {
            HourList = await Http.GetFromJsonAsync<List<Hours>>("/api/Hours/Listele");
            selectedDate = DateTime.Now.Date;
            selectedHourID = HourList.FirstOrDefault().hourID;
            RoomList = await Http.GetFromJsonAsync<List<Rooms>>($"/api/Rooms/Listele?selectedDate={selectedDate.ToString("yyyy-MM-dd")}&selectedHourID={selectedHourID}");

            AppoList = await Http.GetFromJsonAsync<List<Appointments>>("/api/Appointments/Listele");
        }

        protected async Task OnChangeCalendar(DateTime? value)
        {
            selectedRoom = null;
            selectedRoomID = 0;
            if (value >= DateTime.Now.Date)
            {
                selectedDate = Convert.ToDateTime(value);
                RoomList = await Http.GetFromJsonAsync<List<Rooms>>($"/api/Rooms/Listele?selectedDate={selectedDate.ToString("yyyy-MM-dd")}&selectedHourID={selectedHourID}");
            }
            else
            {
                selectedDate = DateTime.Now.Date;
                RoomList = await Http.GetFromJsonAsync<List<Rooms>>($"/api/Rooms/Listele?selectedDate={selectedDate.ToString("yyyy-MM-dd")}&selectedHourID={selectedHourID}");
            }
        }

        protected async Task OnChangeHour(object value)
        {
            selectedRoom = null;
            selectedRoomID = 0;
            RoomList = await Http.GetFromJsonAsync<List<Rooms>>($"/api/Rooms/Listele?selectedDate={selectedDate.ToString("yyyy-MM-dd")}&selectedHourID={selectedHourID}");
            addedAppoRoom = null;
        }

        protected void OnClick(object room,object roomID)
        {
            selectedRoom = room.ToString();
            selectedRoomID = Convert.ToInt32(roomID);
            addedAppoRoom = null;
        }

        protected async Task OnClickRandevu()
        {
            Appointments appo = new Appointments();
            appo.roomID = selectedRoomID;
            appo.userID = 1;
            appo.hourID = selectedHourID;
            appo.date = selectedDate;
            var response = await Http.PostAsJsonAsync<Appointments>($"/api/Appointments/Ekle", appo);

            bool eklendiMi = await response.Content.ReadFromJsonAsync<bool>();
            if (eklendiMi == true)
            {
                RoomList = await Http.GetFromJsonAsync<List<Rooms>>($"/api/Rooms/Listele?selectedDate={selectedDate.ToString("yyyy-MM-dd")}&selectedHourID={selectedHourID}");
                selectedRoomID=0;
                addedAppoRoom = selectedRoom;
                selectedRoom = null;
                //AppoList.Add(appo);
                AppoList = await Http.GetFromJsonAsync<List<Appointments>>("/api/Appointments/Listele");
            }
        }
    }
}
