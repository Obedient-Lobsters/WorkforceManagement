using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkforceManagement.Models
{
    public class TrainingProgram
    {
        [Key]
        public int TrainingProgramId { get; set; }

        [StringLength(80,
            ErrorMessage = "Too Many Characters"), Required]
        public string ProgramName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public int MaximumAttendees { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}