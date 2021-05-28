using LibrarySystem.Server.Data;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoursController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;

        public HoursController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
        }

        [HttpGet("Listele")]
        public async Task<IEnumerable<Hours>> Listele()
        {
            return await Task.Factory.StartNew<IEnumerable<Hours>>(() =>
            {
                return db.Hours;
            });
        }

    }
}
