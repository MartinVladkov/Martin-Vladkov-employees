using CsvHelper;
using Employees.Models;
using Employees.Services.Employees;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Web;

namespace Employees.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IProcessEmployeeService processEmployeeService;

        public EmployeesController(IProcessEmployeeService processEmployeeService)
        {
            this.processEmployeeService = processEmployeeService;
        }

        [HttpGet]
        public IActionResult ProcessEmployees(PairOfEmployees pair = null)
        {
            pair = pair == null ? new PairOfEmployees() : pair;
            return View(pair);
        }

        [HttpPost]
        public IActionResult ProcessEmployees(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            processEmployeeService.WriteFileToLocalStorage(file, fileName);

            var employees = processEmployeeService.MapEmployeesFromCsv(file.FileName);

            var pairsOfEmployees = processEmployeeService.GroupEmployeesByPairs(employees);

            var longestWorkingPair = processEmployeeService.GetLongestWorkingPair(pairsOfEmployees);

            return View(longestWorkingPair);
        }
    }
}
