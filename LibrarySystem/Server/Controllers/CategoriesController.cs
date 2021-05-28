using LibrarySystem.Server.Data;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi,baglantiNesnesi2;

        public CategoriesController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
            baglantiNesnesi2 = new SqlConnection(config["ConnectionString"]);
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<Categories>> Listele()
        {
            return await Task.Factory.StartNew<IEnumerable<Categories>>(() =>
            {
                return db.Categories;
            });
        }

        [HttpPost("Ekle")]
        public async Task<Categories> Ekle([FromBody]Categories category)
        {
            SqlCommand komutNesnesi = new SqlCommand("select * from Categories where category=@category", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@category", category.category);
            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                baglantiNesnesi.Open();

            SqlDataReader reader = komutNesnesi.ExecuteReader();
            if (!reader.HasRows)
            {
                SqlCommand komutNesnesi2 = new SqlCommand("insert into Categories(category) OUTPUT inserted.categoryID values(@category)", baglantiNesnesi2);
                komutNesnesi2.Parameters.AddWithValue("@category", category.category);
                if (baglantiNesnesi2 != null && baglantiNesnesi2.State == ConnectionState.Closed)
                    baglantiNesnesi2.Open();

                int categoryID= (int)komutNesnesi2.ExecuteScalar();
                baglantiNesnesi2.Close();
                category.categoryID = categoryID;
                return category;
            }
            baglantiNesnesi.Close();
            return new Categories();
        }

        public async Task<bool> kayitKontrol(Categories kontrol)
        {
            SqlCommand kontrolKomutNesnesi = new SqlCommand("select * from Categories where category=@category", baglantiNesnesi2);
            kontrolKomutNesnesi.Parameters.AddWithValue("@category", kontrol.category);

            if (baglantiNesnesi2 != null && baglantiNesnesi2.State == ConnectionState.Closed)
                baglantiNesnesi2.Open();

            SqlDataReader reader = kontrolKomutNesnesi.ExecuteReader();
            if (!reader.HasRows)
                return true;
            else
                return false;
        }


        [HttpPut("{categoryID}")]
        public async Task<bool> kayitGuncelleMethod(int categoryID, [FromBody] Categories degisecekKategori)
        {
            var eskiKategori = await db.Categories.FindAsync(categoryID); // Kişi kendinin ismi ve soyismi kayıtlıyken diğer bilgilerini değiştirmek isteyebilir.
            if (await kayitKontrol(degisecekKategori) == true && (eskiKategori.category != degisecekKategori.category))
            {
                SqlCommand komutNesnesi = new SqlCommand("update Categories set category=@category where categoryID=@categoryID", baglantiNesnesi);
                komutNesnesi.Parameters.AddWithValue("@category", degisecekKategori.category);

                komutNesnesi.Parameters.AddWithValue("@categoryID", categoryID);

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

        [HttpDelete("{categoryID}")]
        public async Task itemSil(int categoryID)
        {
            SqlCommand komutNesnesi = new SqlCommand("delete from Categories where categoryID=@categoryID", baglantiNesnesi);

            komutNesnesi.Parameters.AddWithValue("@categoryID", categoryID);

            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                await baglantiNesnesi.OpenAsync();

            await komutNesnesi.ExecuteNonQueryAsync();

            await baglantiNesnesi.CloseAsync();
        }
    }
}
