using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;

namespace Workforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; } = new Employee();

        [Display(Name = "Current Training Programs")]
        public List<SelectListItem> TrainingPrograms { get; }

        [Display(Name = "Current Exercises")]
        public List<SelectListItem> Exercises { get; }

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

            string sql = $@"SELECT Id, Name FROM Cohort";

            //using (IDbConnection conn = Connection) {
            //    List<Cohort> cohorts = (conn.Query<Cohort> (sql)).ToList();

            //    this.Cohorts = cohorts
            //        .Select(li => new SelectListItem {
            //            Text = li.Name,
            //            Value = li.Id.ToString()
            //        }).ToList();
            //}

            string ExerciseSql = $@" SELECT e.Id, e.Name, e.Language FROM Exercise e
;";

            //string StudExerciseSql = $@"SELECT se.Id, s.Id, e.Id
            //                            FROM StudentExercise se
            //                            JOIN Student s ON se.StudentId = s.Id
            //                            JOIN Exercise e ON se.ExerciseId = e.Id
            //                            ";

            using (IDbConnection conn = Connection)
            {
                List<Cohort> cohorts = (conn.Query<Cohort>(sql)).ToList();



                this.Cohorts = cohorts
                    .Select(li => new SelectListItem
                    {
                        Text = li.Name,
                        Value = li.Id.ToString()
                    }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.Cohorts.Insert(0, new SelectListItem
                {
                    Text = "Choose cohort...",
                    Value = "0"
                });

                IEnumerable<Exercise> exercises = (conn.Query<Exercise>(ExerciseSql)).ToList();

                this.Exercises = exercises
                .Select(li => new SelectListItem
                {
                    Text = $"{li.Name}, {li.Language}",
                    Value = li.Id.ToString()
                }).ToList();

                // Add a prompt so that the <select> element isn't blank
                this.Exercises.Insert(0, new SelectListItem
                {
                    Text = "Choose exercise...",
                    Value = "0"
                });
                //List<Exercise> Assignedexercises = (conn.Query<Exercise>(StudExerciseSql)).ToList();
                //this.Student.AssignedExercises = Assignedexercises.Select(li => new Exercise
                //{
                //    Id = li.Id
                //}).ToList();
            }
        }
    }
}
