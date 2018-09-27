using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Computer
    {
        [Key]
        public int ComputerId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatePurchased { get; set; }


        public DateTime? DateDecommissioned { get; set; }

        [Required]
        public bool Working { get; set; }

        [Required]
        public string ModelName { get; set; }

        [Required]
        public string Manufacturer { get; set; }
    }
}