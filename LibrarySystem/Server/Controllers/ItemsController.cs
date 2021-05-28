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
    public class ItemsController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;
        private readonly SqlConnection baglantiNesnesi2;

        public ItemsController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
            baglantiNesnesi2 = new SqlConnection(config["ConnectionString"]);
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<Items>> Listele()
        {
            return await Task.Factory.StartNew<IEnumerable<Items>>(() =>
            {
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select Items.itemID,Items.item,Categories.categoryID,Categories.category from Items inner join Categories on Categories.categoryID=Items.categoryID", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<Items>();
                while (reader.Read())
                {
                    list.Add(new Items { itemID = Convert.ToInt32(reader[0]), item = reader[1].ToString(), categoryID = Convert.ToInt32(reader[2]), category = reader[3].ToString() });
                }
                baglantiNesnesi.CloseAsync();
                return list.ToArray();
            });
        }

        [HttpGet("ListeleKategorisiz")]
        public async Task<IEnumerable<Items>> ListeleKategorisiz()
        {
            return await Task.Factory.StartNew<IEnumerable<Items>>(() =>
            {
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select DISTINCT Items.itemID,Items.item,Items.categoryID from Items inner join Categories on Categories.categoryID!=Items.categoryID where Items.categoryID NOT IN (select categoryID from Categories)", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<Items>();
                while (reader.Read())
                {
                    list.Add(new Items { itemID = Convert.ToInt32(reader[0]), item = reader[1].ToString(), categoryID = Convert.ToInt32(reader[2])});
                }
                baglantiNesnesi.CloseAsync();
                return list.ToArray();
            });
        }

        [HttpGet("ListeleKategoriden/{categoryID}")]
        public async Task<IEnumerable<Items>> ListeleKategoriden(string categoryID)
        {
            return await Task.Factory.StartNew<IEnumerable<Items>>(() =>
            {
                IEnumerable<Items> filteredList;
                filteredList=db.Items.Where(c=>c.categoryID==Convert.ToInt32(categoryID));
                return filteredList.Where(c => c.active == 1);
            });
        }

        [HttpPost("Ekle")]
        public async Task<Items> Ekle([FromBody] Items item)
        {
            SqlCommand komutNesnesi = new SqlCommand("select * from Items where item=@item and categoryID=@categoryID", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@categoryID", item.categoryID);
            komutNesnesi.Parameters.AddWithValue("@item", item.item);
            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                baglantiNesnesi.Open();

            SqlDataReader reader = komutNesnesi.ExecuteReader();
            if (!reader.HasRows)
            {
                SqlCommand komutNesnesi2 = new SqlCommand("insert into Items(categoryID,item,active) OUTPUT inserted.itemID values(@categoryID,@item,1)", baglantiNesnesi2);
                komutNesnesi2.Parameters.AddWithValue("@categoryID", item.categoryID);
                komutNesnesi2.Parameters.AddWithValue("@item", item.item);
                if (baglantiNesnesi2 != null && baglantiNesnesi2.State == ConnectionState.Closed)
                    baglantiNesnesi2.Open();

                item.itemID=Convert.ToInt32(komutNesnesi2.ExecuteScalar());
                baglantiNesnesi2.Close();
                return item;
            }
            baglantiNesnesi.Close();
            return new Items();
        }

        public async Task<bool> kayitKontrol(Items kontrol)
        {
            SqlCommand kontrolKomutNesnesi = new SqlCommand("select * from Items where item=@item EXCEPT select * from Items where itemID=@itemID", baglantiNesnesi2);
            kontrolKomutNesnesi.Parameters.AddWithValue("@item", kontrol.item);
            kontrolKomutNesnesi.Parameters.AddWithValue("@itemID", kontrol.itemID);

            if (baglantiNesnesi2 != null && baglantiNesnesi2.State == ConnectionState.Closed)
                baglantiNesnesi2.Open();

            SqlDataReader reader = kontrolKomutNesnesi.ExecuteReader();
            if (!reader.HasRows)
                return true;
            else
                return false;
        }


        [HttpPut("{itemID}")]
        public async Task<bool> kayitGuncelleMethod(int itemID, [FromBody] Items degisecekKitap)
        {
            if (await kayitKontrol(degisecekKitap) == true)
            {
                SqlCommand komutNesnesi = new SqlCommand("update Items set categoryID=@categoryID,item=@item where itemID=@itemID", baglantiNesnesi);
                komutNesnesi.Parameters.AddWithValue("@item", degisecekKitap.item);
                komutNesnesi.Parameters.AddWithValue("@categoryID", degisecekKitap.categoryID);

                komutNesnesi.Parameters.AddWithValue("@itemID", itemID);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    await baglantiNesnesi.OpenAsync();

                await komutNesnesi.ExecuteNonQueryAsync();

                await baglantiNesnesi.CloseAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpDelete("{itemID}")]
        public async Task itemSil(int itemID)
        {
            SqlCommand komutNesnesi = new SqlCommand("delete from Items where itemID=@itemID", baglantiNesnesi);

            komutNesnesi.Parameters.AddWithValue("@itemID", itemID);

            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                await baglantiNesnesi.OpenAsync();

            await komutNesnesi.ExecuteNonQueryAsync();

            await baglantiNesnesi.CloseAsync();
        }
    }
}
