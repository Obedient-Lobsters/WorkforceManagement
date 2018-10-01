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

        //[Display(Name = "Current Training Programs")]
        //public List<SelectListItem> TrainingPrograms { get; }

        [Display(Name = "Current Department")]
        public List<SelectListItem> Departments { get; }

        [Display(Name = "Current Computer")]
        public List<SelectListItem> Computers { get; }

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

            string compSql = $@"SELECT ComputerId, ModelName FROM Computer";


            string TrainingProgramSql = $@" SELECT tp.TrainingProgramId, tp.ProgramName FROM TrainingProgram tp;";


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
                        Text = li.ModelName,
                        Value = li.ComputerId.ToString()
                    }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.Computers.Insert(0, new SelectListItem
                {
                    Text = "Choose Computer...",
                    Value = "0"
                });

                //IEnumerable<Exercise> exercises = (conn.Query<Exercise>(ExerciseSql)).ToList();

                //this.Exercises = exercises
                //.Select(li => new SelectListItem
                //{
                //    Text = $"{li.Name}, {li.Language}",
                //    Value = li.Id.ToString()
                //}).ToList();

                //// Add a prompt so that the <select> element isn't blank
                //this.Exercises.Insert(0, new SelectListItem
                //{
                //    Text = "Choose exercise...",
                //    Value = "0"
                //});
                ////List<Exercise> Assignedexercises = (conn.Query<Exercise>(StudExerciseSql)).ToList();
                ////this.Student.AssignedExercises = Assignedexercises.Select(li => new Exercise
                ////{
                ////    Id = li.Id
                ////}).ToList();
            }
        }
    }
}
