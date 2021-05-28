using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class Items
    {
        [Key]
        public int itemID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string item { get; set; }

        [Required]
        public int categoryID { get; set; }

        [Required]
        public int active { get; set; }

        [NotMapped]
        public string category { get; set; }
    }
}
