using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;


namespace WorkforceManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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
        // Author: Shu Sajid Purpose: GET all Departments
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> departments = await conn.QueryAsync<Department>(
                    "SELECT DepartmentId, DepartmentName, ExpenseBudget FROM Department;"
                );
                return View(departments);
            }
        }

        // GET: Department/Details/5
        // Author: Evan Lusky
        // This provides the Details view with a Department object with the DepartmentId {id}
        // This method also adds all employees of that department to the object in the employees list property.
        // Since this dapper code returns an ienumerable and the Details view needs a single Department object we use Single() on the query.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            select
                d.DepartmentId,
                d.DepartmentName,
                d.ExpenseBudget,
				e.EmployeeId,
				e.FirstName,
				e.LastName,
				e.Email,
				e.Supervisor,
				e.DepartmentId
            from Department as d
            join Employee as e ON e.DepartmentId = d.DepartmentId
			where d.DepartmentId = {id}";

            using (IDbConnection conn = Connection)
            {

                Dictionary<int, Department> departmentEmployees = new Dictionary<int, Department>();

                var departmentsQuery = await conn.QueryAsync<Department, Employee, Department>(
                    sql,
                    (department, employee) =>
                    {
                        Department departmentEntry;

                        if (!departmentEmployees.TryGetValue(department.DepartmentId, out departmentEntry))
                        {
                            departmentEntry = department;
                            departmentEntry.Employees = new List<Employee>();
                            departmentEmployees.Add(departmentEntry.DepartmentId, departmentEntry);
                        }

                        departmentEntry.Employees.Add(employee);
                        return departmentEntry;
                    }, splitOn: "DepartmentId, EmployeeId"
                    );
                return View(departmentsQuery.Distinct().First());

            }
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
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

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Department/Edit/5
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

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
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