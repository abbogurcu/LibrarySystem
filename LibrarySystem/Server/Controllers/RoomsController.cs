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
    public class RoomsController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;

        public RoomsController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<Rooms>> Listele(DateTime selectedDate,int selectedHourID)
        {
            return await Task.Factory.StartNew<IEnumerable<Rooms>>(() =>
            {
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlConnection baglantiNesnesi2 = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select * from Rooms", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<Rooms>();
                while (reader.Read())
                {
                    bool roomActive = false;
                    SqlCommand komutNesnesi2 = new SqlCommand("select * from Appointments where roomID='"+Convert.ToInt32(reader[0])+"' and date=@date and hourID='"+selectedHourID+"'", baglantiNesnesi2);
                    komutNesnesi2.Parameters.AddWithValue("@date", selectedDate);
                    baglantiNesnesi2.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(komutNesnesi2);
                    DataTable dt = new DataTable();
                    adaptor.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        roomActive = true;
                    }
                    list.Add(new Rooms { roomID = Convert.ToInt32(reader[0]), room = reader[1].ToString(), active = roomActive});
                    baglantiNesnesi2.Close();
                }
                baglantiNesnesi.Close();
                return list.ToArray();
            });
        }

    }
}
