using LibrarySystem.Server.Data;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibrarySystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext db;
        private readonly IConfiguration config;
        private readonly SqlConnection baglantiNesnesi;

        public UsersController(DataContext _db, IConfiguration _config)
        {
            db = _db;
            config = _config;
            baglantiNesnesi = new SqlConnection(config["ConnectionString"]);
        }

        private string CreateJWT(Users user)
        {
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("THIS IS THE SECRET KEY")); // NOTE: SAME KEY AS USED IN Startup.cs FILE
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            var claims = new[] // NOTE: could also use List<Claim> here
			{
                new Claim(JwtRegisteredClaimNames.Sub, user.username), // this would be the username
                new Claim(JwtRegisteredClaimNames.Jti, user.userID.ToString()) // this could a unique ID assigned to the user by a database
			};

            var token = new JwtSecurityToken(issuer: "domain.com", audience: "domain.com", claims: claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost]
        public async Task<LoginResult> Login([FromBody] LoginModel log)
        {
            Users user = new();
            SqlCommand komutNesnesi = new SqlCommand("select userID,username from Users where username=@username and password=@password", baglantiNesnesi);
            komutNesnesi.Parameters.AddWithValue("@username", log.username);
            komutNesnesi.Parameters.AddWithValue("@password", log.password);
            if (baglantiNesnesi != null && baglantiNesnesi.State == ConnectionState.Closed)
                baglantiNesnesi.Open();
            SqlDataReader reader = komutNesnesi.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    user.userID = (int)reader[0];
                    user.username = reader[1].ToString();
                }
                return new LoginResult { message = "Giriş başarılı.", jwtBearer = CreateJWT(user), username = log.username, success = true };
            }
            else
                return new LoginResult { message = "Kullanıcı adı veya şifre yanlış." ,success=false};
        }
    }
}
