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
    public class ItemHistoriesController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;

        public ItemHistoriesController(DataContext _db,IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
        }

        public async Task<bool> Kontrol(ItemHistories ItemData)
        {
            SqlCommand komutNesnesi = new SqlCommand("select * from ItemHistories where userID=@userID and itemID=@itemID and borrowTime=@borrowTime and deliverTime=@deliverTime", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@userID", ItemData.userID);
            komutNesnesi.Parameters.AddWithValue("@itemID", ItemData.itemID);
            komutNesnesi.Parameters.AddWithValue("@borrowTime", ItemData.borrowTime);
            komutNesnesi.Parameters.AddWithValue("@deliverTime", ItemData.deliverTime);

            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                await baglantiNesnesi.OpenAsync();

            SqlDataReader okumaNesnesi = await komutNesnesi.ExecuteReaderAsync();
            if (okumaNesnesi.HasRows) {
                return await Task.Factory.StartNew(() =>
                {
                    baglantiNesnesi.CloseAsync();
                    return true;
                });
            }
            else
            {
                return await Task.Factory.StartNew(() =>
                {
                    baglantiNesnesi.CloseAsync();
                    return false;
                });
            }
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<ItemHistories>> Listele()
        {
            return await Task.Factory.StartNew<IEnumerable<ItemHistories>>(() =>
            {
                /*return db.ItemHistories.Select(s=>new ItemHistories{ itemHistoryID=s.itemHistoryID,itemID=s.itemID,name=s.user.name, surname = s.user.surname, itemName = s.item.item ,categoryName=s.item.category.category,borrowTime=s.borrowTime,deliverTime=s.deliverTime}).OrderByDescending(e=>e.borrowTime);*/
                //SQL DENE BURAYA
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select ItemHistories.itemHistoryID,Users.name,Users.surname,Items.item,Categories.category,ItemHistories.borrowTime,ItemHistories.deliverTime,ItemHistories.userID,ItemHistories.itemID from ItemHistories inner join Items on Items.itemID=ItemHistories.itemID inner join Categories on Categories.categoryID=Items.categoryID inner join Users on Users.userID=ItemHistories.userID order by ItemHistories.borrowTime desc", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<ItemHistories>();
                while (reader.Read())
                {
                    list.Add(new ItemHistories { itemHistoryID = Convert.ToInt32(reader[0]),name = reader[1].ToString(), surname = reader[2].ToString(), itemName = reader[3].ToString(),categoryName = reader[4].ToString(),borrowTime=Convert.ToDateTime(reader[5]), deliverTime = Convert.ToDateTime(reader[6]),userID=Convert.ToInt32(reader[7]),itemID=Convert.ToInt32(reader[8]) });
                }                
                baglantiNesnesi.CloseAsync();
                return list.ToArray();
            });
        }

        [HttpGet("ListeleTeslim")]
        public async Task<IEnumerable<ItemHistories>> ListeleTeslim()
        {
            return await Task.Factory.StartNew<IEnumerable<ItemHistories>>(() =>
            {
                /*return db.ItemHistories.Select(s=>new ItemHistories{ itemHistoryID=s.itemHistoryID,itemID=s.itemID,name=s.user.name, surname = s.user.surname, itemName = s.item.item ,categoryName=s.item.category.category,borrowTime=s.borrowTime,deliverTime=s.deliverTime}).OrderByDescending(e=>e.borrowTime);*/
                //SQL DENE BURAYA
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("select ih.itemHistoryID,u.name,u.surname,i.item,c.category,ih.borrowTime,ih.deliverTime,u.userID,i.itemID from ItemHistories ih inner join Items i on ih.itemID = i.itemID inner join Users u on ih.userID = u.userID inner join Categories c on i.categoryID = c.categoryID where ih.borrowTime IN (select MAX(borrowTime) from ItemHistories ih inner join Items i on ih.itemID = i.itemID where active = 0 group by ih.itemID) order by ih.borrowTime desc", baglantiNesnesi);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    baglantiNesnesi.Open();

                SqlDataReader reader = komutNesnesi.ExecuteReader();
                var list = new List<ItemHistories>();
                while (reader.Read())
                {
                    list.Add(new ItemHistories { itemHistoryID = Convert.ToInt32(reader[0]), name = reader[1].ToString(), surname = reader[2].ToString(), itemName = reader[3].ToString(), categoryName = reader[4].ToString(), borrowTime = Convert.ToDateTime(reader[5]), deliverTime = Convert.ToDateTime(reader[6]), userID = Convert.ToInt32(reader[7]), itemID = Convert.ToInt32(reader[8]) });
                }
                baglantiNesnesi.CloseAsync();
                return list.ToArray();
            });
        }

        [HttpPost]
        public async Task<bool> Ekle(ItemHistories ItemData)
        {
            if (await Kontrol(ItemData) == false)
            {
                SqlConnection baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
                SqlCommand komutNesnesi = new SqlCommand("insert into ItemHistories(userID,itemID,borrowTime,deliverTime) values(@userID,@itemID,@borrowTime,@deliverTime)", baglantiNesnesi);
                komutNesnesi.Parameters.AddWithValue("@userID", ItemData.userID);
                komutNesnesi.Parameters.AddWithValue("@itemID", ItemData.itemID);
                komutNesnesi.Parameters.AddWithValue("@borrowTime", ItemData.borrowTime);
                komutNesnesi.Parameters.AddWithValue("@deliverTime", ItemData.deliverTime);

                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    await baglantiNesnesi.OpenAsync();

                await komutNesnesi.ExecuteNonQueryAsync();

                SqlCommand komutNesnesiActive = new SqlCommand("update Items set active=0 where itemID=@itemID", baglantiNesnesi);
                komutNesnesiActive.Parameters.AddWithValue("@itemID", ItemData.itemID);

                await komutNesnesiActive.ExecuteNonQueryAsync();

                return await Task.Factory.StartNew(() =>
                {

                    baglantiNesnesi.CloseAsync();
                    return true;
                });
            }
            else
            {
                return await Task.Factory.StartNew(() =>
                {
                    baglantiNesnesi.CloseAsync();
                    return false;
                });
            }
        }

        [HttpPost("Teslim")]
        public async Task<bool> Teslim(ItemHistories item)
        {
            SqlCommand komutNesnesi = new SqlCommand("select itemID from Items where itemID=@itemID and active=0", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@itemID", item.itemID);
            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                await baglantiNesnesi.OpenAsync();

            int result = Convert.ToInt32(komutNesnesi.ExecuteScalar());
            baglantiNesnesi.Close();
            if (result==item.itemID){
                SqlCommand komutNesnesi2 = new SqlCommand("update Items set active=1 where itemID=@itemID", baglantiNesnesi);
                komutNesnesi2.Parameters.AddWithValue("@itemID", item.itemID);
                if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                    await baglantiNesnesi.OpenAsync();

                komutNesnesi2.ExecuteNonQuery();
                baglantiNesnesi.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
