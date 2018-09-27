﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;
using WorkforceManagement.Models.ViewModels;
using System.Data.SqlClient;

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
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        private async Task<SelectList> DepartmentList(int? selected)
        {
            using (IDbConnection conn = Connection)
            {
                // Get all cohort data
                List<Department> departments = (await conn.QueryAsync<Department>("SELECT Id, Name FROM Department")).ToList();

                // Add a prompting cohort for dropdown
                departments.Insert(0, new Department() { DepartmentId = 0, DepartmentName = "Select department..." });

                // Generate SelectList from cohorts
                var selectList = new SelectList(departments, "Id", "Name", selected);
                return selectList;
            }
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
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
                        ( FirstName, LastName, StartDate, DepartmentId )
                        VALUES
                        ( null 
                            , '{employee.FirstName}'
                            , '{employee.LastName}'
                            , '{employee.StartDate}'
                            , '{employee.DepartmentId}'
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

            // ModelState was invalid, or saving the Employee data failed. Show the form again.
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> departments = (await conn.QueryAsync<Department>("SELECT Id, Name FROM Department")).ToList();
                ViewData["DepartmentId"] = await DepartmentList(department.DepartmentId);
                return View(department);
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