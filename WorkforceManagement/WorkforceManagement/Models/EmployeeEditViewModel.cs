using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;

namespace WorkforceManagement.Models.ViewModels
{
    public class EmployeeEditViewModel
    { 
        public Employee Employee { get; set; } = new Employee();


        [Display(Name = "Department")]
        public List<SelectListItem> Departments { get; }

        [Display(Name = "Assigned Computer")]
        public List<SelectListItem> Computers { get; }

        [Display(Name = "Training Programs")]
        public List<SelectListItem> TrainingPrograms { get; set; } = new List<SelectListItem>();

        public int[] Enrolled { get; set; }

        private readonly IConfiguration _config;

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeEditViewModel() { }

        public EmployeeEditViewModel(IConfiguration config)
        {
            _config = config;

            string sql = $@"SELECT DepartmentId, DepartmentName FROM Department";

            string compSql = $@"SELECT ComputerId, ModelName, Manufacturer FROM Computer";


            string TrainingProgSql = $@"SELECT TrainingProgramId, ProgramName FROM TrainingProgram";


            using (IDbConnection conn = Connection)
            {
                List<Department> departments = (conn.Query<Department>(sql)).ToList();



                this.Departments = departments
                    .Select(li => new SelectListItem
                    {
                        Text = li.DepartmentName,
                        Value = li.DepartmentId.ToString()
                    }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.Departments.Insert(0, new SelectListItem
                {
                    Text = "Choose Department...",
                    Value = "0"
                });

                List<Computer> computers = (conn.Query<Computer>(compSql)).ToList();



                this.Computers = computers
                    .Select(li => new SelectListItem
                    {
                        Text = $"{li.Manufacturer} {li.ModelName}",
                        Value = li.ComputerId.ToString()
                    }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.Computers.Insert(0, new SelectListItem
                {
                    Text = "Choose Computer...",
                    Value = "0"
                });

                List<TrainingProgram> trainingProgs = conn.Query<TrainingProgram>(TrainingProgSql).ToList();

                this.TrainingPrograms = trainingProgs
                .Select(li => new SelectListItem
                {
                    Text = $"{li.ProgramName}",
                    Value = li.TrainingProgramId.ToString()
                }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.TrainingPrograms.Insert(0, new SelectListItem
                {
                    Text = "Choose Training Program...",
                    Value = "0"
                });

            }
        }
    }
}
