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
        public IActionResult ProcessEmployees(PairOfEmployeesViewModel pair = null)
        {
            pair = pair == null ? new PairOfEmployeesViewModel() : pair;
            return View(pair);
        }

        [HttpPost]
        public IActionResult ProcessEmployees(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            if(file == null)
            {
                //show error
                //ModelState.AddModelError(nameof(file), "A CSV file must be attached.");
            }

            var extension = file.FileName.Split(".");
            if (extension[extension.Length - 1].ToLower() != "csv")
            {
                //throw model view error
                var a = 0;
            }

            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            processEmployeeService.WriteFileToLocalStorage(file, fileName);

            var employees = processEmployeeService.MapEmployeesFromCsv(file.FileName);

            var pairsOfEmployees = processEmployeeService.GroupEmployeesByPairs(employees);

            var longestWorkingPair = processEmployeeService.GetLongestWorkingPair(pairsOfEmployees);

            var viewModel = new PairOfEmployeesViewModel
            {
                EmployeeId1 = longestWorkingPair.EmployeeId1,
                EmployeeId2 = longestWorkingPair.EmployeeId2,
                Days = longestWorkingPair.Days,
                PairsOfEmployees = pairsOfEmployees
            };

            return View(viewModel);
        }
    }
}
