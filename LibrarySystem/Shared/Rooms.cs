using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class Rooms
    {
        [Key]
        public int roomID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string room { get; set; }

        [NotMapped]
        public bool active { get; set; }
    }
}
