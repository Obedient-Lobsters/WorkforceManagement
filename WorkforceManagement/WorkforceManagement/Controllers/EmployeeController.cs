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
                SELECT TOP 1
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Supervisor,
                    e.DepartmentId,
                    d.DepartmentId,
                    d.DepartmentName,
                    ec.EmployeeComputerId,
                    ec.ComputerId,
                    ec.EmployeeId,
                    c.ComputerId,
                    c.ModelName,
                    c.Manufacturer,
                    c.DatePurchased,
                    c.Working
                FROM Employee e
                JOIN Department d on e.DepartmentId = d.DepartmentId
				JOIN EmployeeComputer ec ON e.EmployeeId = ec.EmployeeId 
                JOIN Computer c on ec.ComputerId = c.ComputerId
                WHERE e.EmployeeId = {id}
                ORDER BY DateReturned ASC;";

            string trainingProgSql = $@"
                    SELECT
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Supervisor,
                    e.DepartmentId,
					tp.TrainingProgramId,
					tp.ProgramName
                FROM Employee e
				JOIN EmployeeTraining et ON e.EmployeeId = et.EmployeeId
				JOIN TrainingProgram tp ON tp.TrainingProgramId = et.TrainingProgramId
                WHERE e.EmployeeId = {id}";


            using (IDbConnection conn = Connection)
            {
                   EmployeeEditViewModel model = new EmployeeEditViewModel(_config);
                Dictionary<int, Employee> EmployeesDictionary = new Dictionary<int, Employee>();
                 var employeesQuery = await conn.QueryAsync<Employee, Department, Computer, Employee>(
                   sql,
                   (employee, department, computer) =>
                   {
                       Employee employeeEntry;
                       if (!EmployeesDictionary.TryGetValue(employee.EmployeeId, out employeeEntry))
                       {
                           employeeEntry = employee;
                           employeeEntry.ComputerId = computer.ComputerId;
                           employeeEntry.Computer = computer;
                           employeeEntry.Department = department;

                           EmployeesDictionary.Add(employeeEntry.EmployeeId, employeeEntry);
                       }
                       return employeeEntry;
                   }, splitOn: "EmployeeId,DepartmentId,ComputerId"
                   );

                model.Employee = employeesQuery.Distinct().Single();



                //using (IDbConnection conn = Connection)
                //{
                //    EmployeeEditViewModel model = new EmployeeEditViewModel(_config);

                //    var employeeDictionary = new Dictionary<int, Employee>();

                //    model.Employee = (await conn.QueryAsync<Employee, Department, Computer, Employee>(
                //        sql,
                //        (employee, department, computer) =>
                //        {

                //            employee.Department = department;
                //            employee.Computer = computer;


                //if (employee.Computer.ModelName == null)
                //{
                //    employee.Computer.ModelName = computer.ModelName;
                //}

                //if (employee.Computer.Manufacturer == null)
                //{
                //    employee.Computer.Manufacturer = computer.Manufacturer;
                //}
                //employee.Computer.Manufacturer = computer.Manufacturer;
                //employee.Computer.ModelName = computer.ModelName;
                //        return employee;
                //    }, splitOn: "DepartmentId,ComputerId"
                //)).First();



                var someStiff = (await conn.QueryAsync<TrainingProgram>(
                                trainingProgSql));


                model.Enrolled = someStiff.Select(progIds => progIds.TrainingProgramId).ToArray();

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

            ModelState.Remove("Employee.Computer.ModelName");
            ModelState.Remove("Employee.Computer.Manufacturer");

            String currentDate = DateTime.Now.ToString("yyyy-MM-dd");



            if (ModelState.IsValid)
            {
                string sql = $@"
                UPDATE Employee
                SET
                    LastName = '{model.Employee.LastName}',
                    DepartmentId = {model.Employee.DepartmentId}

                WHERE EmployeeId = {id};";


                using (IDbConnection conn = Connection)
                {
                    string compSql = $@"
                        SELECT TOP 1 EmployeeComputerId, DateAssigned, DateReturned, ComputerId, EmployeeId 
                        FROM EmployeeComputer 
                        WHERE EmployeeId = {id}
						ORDER BY DateReturned ASC;";

                    Computer currentComp = conn.Query<Computer>(compSql).Single();

                    if (model.Employee.ComputerId != currentComp.ComputerId)
                    {       if (currentComp != null) {
                            sql += " IF (OBJECT_ID('dbo.FK_ComputerEmployee', 'F') IS NOT NULL) BEGIN ALTER TABLE dbo.EmployeeComputer DROP CONSTRAINT FK_ComputerEmployee END UPDATE EmployeeComputer" +
                                $" SET DateReturned = '{currentDate}' ";
                        }
                            //will need to change this line to select employeecomputer id. Will screw up if com changed more than once.
                        sql += $" WHERE ComputerId = {model.Employee.ComputerId} AND EmployeeId = {model.Employee.EmployeeId};" +
                            $" INSERT INTO EmployeeComputer " +
                            $" (ComputerId, EmployeeId, DateAssigned)" +
                            $" VALUES(" +
                            $"'{model.Employee.ComputerId}' , '{model.Employee.EmployeeId}', '{currentDate}')";
                    }
                }




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