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
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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
        // Author: Evan Lusky
        // Simple dapper query for Computer
        // Returns and IEnumerable of Computer objects to pass to View.
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(
                    "select ComputerId, ModelName, Manufacturer, Working from Computer;"
                );
                return View(computers);
            }
        }

        // GET: Computer/Details/5
        //Author: Shu Sajid
        //Purpose:This provides the Details view with a Computer object with the DepartmentId {id}
        //Since this dapper code returns an ienumerable and the Details view needs a single Department object we use Single() on the query.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            SELECT
                c.ComputerId,
                c.DatePurchased,
                c.DateDecommissioned,
                c.Working,
                c.ModelName,
                c.Manufacturer
            FROM Computer c
            WHERE ComputerId = {id};";

            using (IDbConnection conn = Connection)
            {
                Computer computerQuery = await conn.QuerySingleAsync<Computer>(sql);

                if (computerQuery == null)
                {
                    return NotFound();
                }
                return View(computerQuery);
            }
        }

        // Author: Evan Lusky
        // Dapper runs the Insert and pulls the info from the form presented on the Razor view.
        // So long as it works (rows affected > 0) it redirects back to the index, otherwise the form is reloaded.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatePurchased, DateDecommissioned, Working, ModelName, Manufacturer")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Computer
                        ( DatePurchased, DateDecommissioned, Working, ModelName, Manufacturer )
                        VALUES
                        ( '{computer.DatePurchased}', null, '{computer.Working}', '{computer.ModelName}', '{computer.Manufacturer}' )
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

            return View(computer);
        }

       

        // GET: Computer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computer/Edit/5
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

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Computer/Delete/5
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