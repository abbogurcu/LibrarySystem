using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class Hours
    {
        [Key]
        public int hourID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string hour { get; set; }
    }
}
