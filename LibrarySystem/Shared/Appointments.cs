using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared
{
    public class Appointments
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int userID { get; set; }

        [Required]
        public int roomID { get; set; }

        [Required]
        public int hourID { get; set; }

        [Required]
        public DateTime date { get; set; }

        [NotMapped]
        public string user { get; set; }
        [NotMapped]
        public string room { get; set; }
        [NotMapped]
        public string hour { get; set; }
        [NotMapped]
        public string dateString { get; set; }
    }
}
