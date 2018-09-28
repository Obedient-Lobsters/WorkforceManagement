using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;
using System.Data.SqlClient;

namespace WorkforceManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        //Author:Shu Sajid Purpose:These methods create connection
        //to the database
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // Author:Shu Sajid Purpose: GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // Author:Shu Sajid Purpose: POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentName, ExpenseBudget")] Department department)
        {
            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Department
                        ( DepartmentName, ExpenseBudget )
                        VALUES
                        ( '{department.DepartmentName}', 0 )
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

            return View(department);
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