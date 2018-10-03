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
        // Purpose: Execute a sql statement that gets required information about an employee and the Department that they belong to, and then send that information to the view that corresponds to the Employee model
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
                        (employee, department) =>
                        {
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

        //Author: Leah Gwin
        //Purpose: Load Dropdown for Dept. Create
        private async Task<SelectList> DepartmentList(int? selected)
        {
            using (IDbConnection conn = Connection)
            {
                // Get all department data
                List<Department> departments = (await conn.QueryAsync<Department>("SELECT DepartmentId, DepartmentName FROM Department")).ToList();

                // Add a prompting department for dropdown
                departments.Insert(0, new Department() { DepartmentId = 0, DepartmentName = "Select department..." });

                // Generate SelectList from department
                var selectList = new SelectList(departments, "DepartmentId", "DepartmentName", selected);
                return selectList;
            }
        }
        //Author: Leah Gwin
        //Purpose: Enables the Dept list to be in the dropdown for post.
        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            using (IDbConnection conn = Connection)
            {
                ViewData["DepartmentId"] = await DepartmentList(null);
                return View();
            }
        }

        // POST: Employee/Create
        //Author: Leah Gwin
        //Purpose: For HR to be able to add a new employee 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Employee
                        ( FirstName, LastName, Email, StartDate, DepartmentId, Supervisor)
                        VALUES
                        (  
                              '{employee.FirstName}'
                            , '{employee.LastName}'
                            , '{employee.Email}'
                            , '{employee.StartDate}'
                            , '{employee.DepartmentId}'
                            , 0
)
                    ";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            //ModelState was invalid, or saving the Employee data failed. Show the form again.
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> departments = (await conn.QueryAsync<Department>("SELECT DepartmentId, DepartmentName FROM Department")).ToList();
                ViewData["DepartmentId"] = await DepartmentList(employee.DepartmentId);
                return View(employee);
            }
        }
        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit (int id)
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
                        (employee, department) =>
                        {
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
                    d.DepartmentId,
                    d.DepartmentName,
                    c.ComputerId,
                    c.Manufacturer,
                    c.ModelName,
                    t.TrainingProgramId,
                    t.ProgramName,
                    t.StartDate,
                    t.EndDate
                FROM Employee e
                JOIN Department d ON e.DepartmentId = d.DepartmentId
                LEFT JOIN EmployeeComputer ec ON ec.EmployeeId = e.EmployeeId
                LEFT JOIN Computer c ON ec.ComputerId = c.ComputerId
                LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.EmployeeId
                LEFT JOIN TrainingProgram t ON et.TrainingProgramId = t.TrainingProgramId
                WHERE e.EmployeeId = { id };";


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
    }
}