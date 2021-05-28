using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class ItemHistories
    {
        [Key]
        public int itemHistoryID { get; set; }

        [Required]
        public int userID { get; set; }

        [Required]
        public int itemID { get; set; }

        [Required]
        public DateTime borrowTime { get; set; } = DateTime.Now.Date;

        public DateTime deliverTime { get; set; }

        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string surname { get; set; }
        [NotMapped]
        public string categoryName { get; set; }
        [NotMapped]
        public string itemName { get; set; }

    }
}
