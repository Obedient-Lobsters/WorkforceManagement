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

        //GET(SINGLE): Employee Details
        //Purpose: Excute SQL statement that gets single employee detail and returns value of of columns in of single row.
        //Author: Aaron Miller 

        public async Task<IActionResult> Details(int? id)
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
                e.DepartmentId,
				d.DepartmentId,
				d.DepartmentName,
				ec.EmployeeComputerId,
				ec.EmployeeId,
				ec.ComputerId,
				c.ComputerId,
				c.Manufacturer,
				c.ModelName,
				et.EmployeeId,
				et.TrainingProgramId, 
				tp.TrainingProgramId,
				tp.ProgramName,
				tp.StartDate,
				tp.EndDate
            FROM Employee e
            JOIN Department d 
			ON e.DepartmentId = d.DepartmentId
			JOIN EmployeeComputer ec
			ON e.EmployeeId= ec.EmployeeId
			JOIN Computer c 
			ON ec.ComputerId= c.ComputerId
			JOIN EmployeeTraining et 
			ON e.EmployeeId= et.EmployeeId
			JOIN TrainingProgram tp 
			ON et.TrainingProgramId= tp.TrainingProgramId";


            using (IDbConnection conn = Connection)
            {
                Dictionary<int, Employee> EmployeesDictionary = new Dictionary<int, Employee>();
                var employeesQuery = await conn.QueryAsync<Employee, Department, Computer, TrainingProgram, Employee>(
                   sql,
                   (employee, department, computer, trainingProgram) =>
                   {
                       Employee employeeEntry;
                       if (!EmployeesDictionary.TryGetValue(employee.EmployeeId, out employeeEntry))
                       {
                    employeeEntry = employee;
                           employeeEntry.Computer = computer;
                           employeeEntry.Department = department;
                 
                    employeeEntry.TrainingPrograms = new List<TrainingProgram>();
                           EmployeesDictionary.Add(employeeEntry.EmployeeId, employeeEntry);
                       }
                employeeEntry.TrainingPrograms.Add(trainingProgram);
                       return employeeEntry;
                   }, splitOn: "EmployeeId, DepartmentId, ComputerId, TrainingProgramId"
                   );
                return View(employeesQuery.Distinct().First());
            }
        }


        //// GET: Employee
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: Employee/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Employee/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Employee/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Employee/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Employee/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Employee/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Employee/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
    }
    }