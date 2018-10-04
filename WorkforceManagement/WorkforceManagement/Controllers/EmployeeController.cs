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
                    ec.DateReturned,
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

                    if (model.Employee.Computer.ComputerId != currentComp.ComputerId)
                    {
                        if (currentComp != null)
                        {
                            sql += " IF (OBJECT_ID('dbo.FK_ComputerEmployee', 'F') IS NOT NULL) BEGIN ALTER TABLE dbo.EmployeeComputer DROP CONSTRAINT FK_ComputerEmployee END UPDATE EmployeeComputer" +
                                $" SET DateReturned = '{currentDate}' ";
                        }
                        //will need to change this line to select employeecomputer id. Will screw up if com changed more than once.
                        sql += $" WHERE ComputerId = {currentComp.ComputerId} AND EmployeeId = {model.Employee.EmployeeId};" +
                            $" INSERT INTO EmployeeComputer " +
                            $" (ComputerId, EmployeeId, DateAssigned)" +
                            $" VALUES(" +
                            $"'{model.Employee.Computer.ComputerId}' , '{model.Employee.EmployeeId}', '{currentDate}')";
                    }

                    Console.WriteLine(model.Enrolled);

                    string trainingProgSql = $@"
                        SELECT TrainingProgramId, EmployeeTrainingId, EmployeeId 
                        FROM EmployeeTraining 
                        WHERE EmployeeId = {id};";

                    List<TrainingProgram> trainingProgs = conn.Query<TrainingProgram>(trainingProgSql).ToList();

                    int[] preselected = trainingProgs.Select(progIds => progIds.TrainingProgramId).ToArray();

                    var preselectedTP = new HashSet<int>(preselected);
                    var newlySelectedTP = new HashSet<int>(model.Enrolled);

                    //var toAdd = preselectedTP.Distinct<newlySelectedTP>;

                    var toAdd = new HashSet<int>(newlySelectedTP.Except(preselectedTP));
                    var toDelete = new HashSet<int>(preselectedTP.Except(newlySelectedTP));

                    foreach (int tp in model.Enrolled) {
                        if (toAdd != null)
                        {

                            //will need to change this line to select employeecomputer id.Will screw up if com changed more than once.
                            foreach (int TPid in toAdd ) {
                                sql += $" WHERE ComputerId = {currentComp.ComputerId} AND EmployeeId = {model.Employee.EmployeeId};" +
                                    $" INSERT INTO EmployeeComputer " +
                                    $" (ComputerId, EmployeeId, DateAssigned)" +
                                    $" VALUES(" +
                                    $"'{model.Employee.Computer.ComputerId}' , '{model.Employee.EmployeeId}', '{currentDate}')";
                            }
                        }
                    if (toDelete != null)
                        {

                        }
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
                WHERE e.EmployeeId = { id } and ec.DateReturned is null;" ;


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