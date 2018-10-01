using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WorkforceManagement.Models;
using WorkforceManagement.Models.ViewModels;

namespace WorkforceManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employee
        // Purpose: Execute a sql statement that gets required information about an employee and the Department that they bolong to, and then send that information to the view that corresponds to the Employee model
        // Author: William K. Kimball
        public async Task<IActionResult> Index()
        {

            string sql = @"
            SELECT
                e.EmployeeId,
                e.FirstName,
                e.LastName,
				e.Email,
                d.DepartmentId,
                d.DepartmentName
            FROM Employee e
            JOIN Department d ON e.DepartmentId = d.DepartmentId
        ";

            using (IDbConnection conn = Connection)
            {
                Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

                var employeeQuerySet = await conn.QueryAsync<Employee, Department, Employee>(
                        sql,
                        (employee, department) => {
                            if (!employees.ContainsKey(employee.EmployeeId))
                            {
                                employees[employee.EmployeeId] = employee;
                            }
                            employees[employee.EmployeeId].Department = department;
                            return employee;
                        }, splitOn: "EmployeeId, DepartmentId"
                    );
                return View(employees.Values);

            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //Edit Employee
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
                SELECT
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Supervisor,
                    e.StartDate,
                    e.EndDate,
                    e.DepartmentId,
                    d.DepartmentId,
                    d.DepartmentName
                FROM Employee e
                JOIN Department d on e.DepartmentId = d.DepartmentId
                WHERE e.EmployeeId = {id}";

            string compSql = $@"
                SELECT
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Supervisor,
                    e.StartDate,
                    e.EndDate,
                    c.ComputerId,
                    c.ModelName,
                    c.Manufacturer
                FROM Employee e
				JOIN EmployeeComputer ec ON e.EmployeeId = ec.EmployeeId 
                JOIN Computer c on ec.ComputerId = c.ComputerId
                WHERE e.EmployeeId = {id}
                ";

            string exerciseSql = $@"
                    SELECT
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.SlackHandle,
                    s.CohortId,
                    c.Id,
                    c.Name,
					e.Id,
					e.Name,
					e.Language
                FROM Student s
                JOIN Cohort c on s.CohortId = c.Id
				JOIN StudentExercise se ON s.Id = se.StudentId
				JOIN Exercise e ON se.ExerciseId = e.Id
                WHERE s.Id = {id}";

            using (IDbConnection conn = Connection)
            {
                EmployeeEditViewModel model = new EmployeeEditViewModel(_config);

                model.Employee = (await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    }, splitOn: "EmployeeId, DepartmentId"
                )).Single();

                model.Employee = (await conn.QueryAsync<Employee, Computer, Employee>(
                compSql,
                (employee, computer) => {
                     employee.Computer = computer;
                     return employee;
                }, splitOn: "EmployeeId, ComputerId"
                )).Single();

                //model.Employee.Tra = (await conn.QueryAsync<Student, Exercise, Exercise>(
                //    exerciseSql,
                //    (student, exercise) =>
                //    {
                //        student.AssignedExercises.Add(exercise);
                //        return exercise;
                //    }
                //)).ToList();

                //model.SelectedExerciseIds = model.Student.AssignedExercises.Select(exercise => { exercise.Id } ;);



                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel model)
        {
            if (id != model.Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = $@"
                UPDATE Employee
                SET
                    LastName = '{model.Employee.LastName}',
                    DepartmentId = '{model.Employee.DepartmentId}'
                WHERE EmployeeId = {id}";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    throw new Exception("No rows affected");
                }
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}