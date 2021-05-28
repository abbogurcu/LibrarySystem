using LibrarySystem.Server.Data;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;
        private readonly SqlConnection baglantiNesnesi2;

        public AppointmentsController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]); 
            baglantiNesnesi2 = new SqlConnection(config["ConnectionString"]);
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<Appointments>> Listele()
        {
            return await Task.Factory.StartNew<IEnumerable<Appointments>>(() =>
            {
                /*return db.Appointments.Select(s=>new Appointments{ itemHistoryID=s.itemHistoryID,itemID=s.itemID,name=s.user.name, surname = s.user.surname, itemName = s.item.item ,categoryName=s.item.category.category,borrowTime=s.borrowTime,deliverTime=s.deliverTime}).OrderByDescending(e=>e.borrowTime);*/
                //SQL DENE BURAYA
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select Appointments.AppointmentID,Users.name,Users.surname,Rooms.room,Appointments.date,Hours.hour from Appointments inner join Hours on Hours.hourID=Appointments.hourID inner join Users on Users.userID=Appointments.userID inner join Rooms on Rooms.roomID=Appointments.roomID order by Appointments.date desc,Appointments.hourID desc", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<Appointments>();
                while (reader.Read())
                { 
                    list.Add(new Appointments { AppointmentID = Convert.ToInt32(reader[0]), user = reader[1].ToString()+ " " +reader[2].ToString(), room = reader[3].ToString(), dateString = Convert.ToDateTime(reader[4]).Date.ToShortDateString(), hour=reader[5].ToString() });
                }
                baglantiNesnesi.CloseAsync();
                return list.ToArray();
            });
        }


        [HttpPost("Ekle")]
        public async Task<bool> Ekle([FromBody]Appointments ItemData)
        {
            SqlCommand komutNesnesi = new SqlCommand("select * from Appointments where date=@date and hourID=@hourID and roomID=@roomID", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@hourID", ItemData.hourID);
            komutNesnesi.Parameters.AddWithValue("@date", ItemData.date);
            komutNesnesi.Parameters.AddWithValue("@roomID", ItemData.roomID);

            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                await baglantiNesnesi.OpenAsync();

            SqlDataReader okumaNesnesi = await komutNesnesi.ExecuteReaderAsync();
            if (!okumaNesnesi.HasRows)
            {
                SqlCommand komutNesnesi2 = new SqlCommand("insert into Appointments values(@userID,@roomID,@hourID,@date)", baglantiNesnesi2);
                komutNesnesi2.Parameters.AddWithValue("@userID", ItemData.userID);
                komutNesnesi2.Parameters.AddWithValue("@roomID", ItemData.roomID);
                komutNesnesi2.Parameters.AddWithValue("@hourID", ItemData.hourID);
                komutNesnesi2.Parameters.AddWithValue("@date", ItemData.date);

                if (baglantiNesnesi2 != null && baglantiNesnesi2.State == ConnectionState.Closed)
                    await baglantiNesnesi2.OpenAsync();

                komutNesnesi2.ExecuteNonQuery();
                baglantiNesnesi2.Close();
                return true;
            }
            baglantiNesnesi.Close();
            return false;
        }
    }
}
