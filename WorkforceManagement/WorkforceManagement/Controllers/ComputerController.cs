using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

        // GET: Computer/Delete/5
        //Author:Shuaib Sajid
        //Purpose: This method creates the model to display on Delete Confirm
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
                          SELECT ec.ComputerId,
                                 c.ComputerId,
                                 c.DatePurchased,
                                 c.DateDecommissioned,
                                 c.Working,
                                 c.ModelName,
                                 c.Manufacturer
                          FROM Computer c
                          LEFT JOIN EmployeeComputer ec ON ec.ComputerId = c.ComputerId
                          WHERE c.ComputerId = {id}";

            using (IDbConnection conn = Connection)
            {
                Computer computer = await conn.QueryFirstAsync<Computer>(sql);

                if (computer == null) return NotFound("Inside DeleteConfirm");

                return View(computer);
            }
        }


        // POST: Computer/Delete/5
        //Author:Shuaib Sajid Purpose:This method checks if the computer exists on the EmployeeComputer table
        //If it does exist then it does not allow a delete, it if doesn't then a delete is allowed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ComputerId)
        {

            string sql = $@"
                          SELECT ec.ComputerId,
                                 c.ComputerId
                          FROM EmployeeComputer ec
                          JOIN Computer c ON ec.ComputerId = c.ComputerId
                          WHERE c.ComputerId = {ComputerId}";
            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> rowsReturned = await conn.QueryAsync(sql);
                int count = rowsReturned.Count();
                if (count == 0)
                {
                    sql = $@"DELETE FROM Computer WHERE ComputerId = {ComputerId};";
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View("DeleteDenied");
                }
                throw new Exception("Computer is currently or has been previously been assigned");
            }
        }
    }
}