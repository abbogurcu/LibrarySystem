using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class Admins
    {
        [Key]
        public int adminID { get; set; }

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
