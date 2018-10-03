//Authors: Leah Gwin, William Kimball, and Aaron Miller
//Purpose: Model for Employee

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
        [DataType(DataType.Date), Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date), Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        public bool Supervisor { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public Department Department { get; set; }

        [Display(Name = "Assigned Computer")]
        public Computer Computer { get; set; } = new Computer();

        [Display(Name = "Employee Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Display(Name = "Assigned Training Programs")]
        public List<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

    }
}