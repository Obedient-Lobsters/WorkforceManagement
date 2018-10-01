//Author:Shu Sajid 
//Purpose:Model for Computer Table
using System;
using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models
{
    public class Computer
    {
        [Key]
        [Display(Name = "Computer Id #")]
        public int ComputerId { get; set; }

        [Required]
        [DataType(DataType.Date), Display(Name="Date of Purchase")]
        public DateTime DatePurchased { get; set; }

        [Display(Name = "Date Decommissioned"),DisplayFormat(NullDisplayText = "In Service", ApplyFormatInEditMode = true)]
        public DateTime? DateDecommissioned { get; set; }

        [Required]
        [Display (Name = "Operational")]
        public bool Working { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Required]
        public string Manufacturer { get; set; }
    }
}