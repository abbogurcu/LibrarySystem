using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }

    }
    public class LoginResult
    {
        public string message { get; set; }
        public string username { get; set; }
        public string jwtBearer { get; set; }
        public bool success { get; set; }
    }

    public class Users
    {
        [Key]
        public int userID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string studentNo { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string surname { get; set; }


        [Required]
        public int departmentID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(13)]
        public string phone { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string email { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string password { get; set; }
    }
}
