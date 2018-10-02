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

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
                        ( FirstName, LastName, StartDate, DepartmentId, Supervisor)
                        VALUES
                        (  
                              '{employee.FirstName}'
                            , '{employee.LastName}'
                            , '{employee.StartDate}'
                            , '{employee.DepartmentId}'
                            , 0                        )
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
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