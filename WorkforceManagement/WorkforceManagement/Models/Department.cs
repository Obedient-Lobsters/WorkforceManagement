//Author: Shuaib Sajid
//Purpose: Model for Department
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Departments")]

        public string DepartmentName { get; set; }
        public int ExpenseBudget { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}