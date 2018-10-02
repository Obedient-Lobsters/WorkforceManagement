using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorkforceManagement.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool Supervisor { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public int ComputerId { get; set; }

        public Computer Computer { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

        [Display(Name = "Employee Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

    }
}