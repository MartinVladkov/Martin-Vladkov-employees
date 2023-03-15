using CsvHelper;
using Employees.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Web;

namespace Employees.Controllers
{
    public class EmployeesController : Controller
    {
        [HttpGet]
        public IActionResult ProcessEmployees(List<Employee> employees = null)
        {
            employees = employees == null ? new List<Employee>() : employees;
            return View(employees);
        }

        [HttpPost]
        public IActionResult ProcessEmployees(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using(FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream); 
                fileStream.Flush();
            }
            var employees = MapEmployeeFromCsv(file.FileName);

            var longestWorkingPair = LongestWorkingPair(employees);

            return View(employees);
        }

        private List<Employee> MapEmployeeFromCsv(string fileName)
        {
            List<Employee> employees = new List<Employee>();

            var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fileName;

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var line = csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var csvEmployee = csv.GetRecord<CsvEmployee>();

                    var employee = new Employee();
                    employee.Id = csvEmployee.Id;
                    employee.ProjectId = csvEmployee.ProjectId;
                    employee.DateFrom = DateOnly.FromDateTime(Convert.ToDateTime(csvEmployee.DateFrom));

                    if (csvEmployee.DateTo.ToLower() == "null")
                    {
                        employee.DateTo = DateOnly.FromDateTime(DateTime.Now);
                    }

                    employees.Add(employee);
                }
            }

            return employees;
        }

        private string LongestWorkingPair(List<Employee> employees)
        {
            var groupedByProject = employees
                            .Select(e => e)
                            .GroupBy(e => e.ProjectId)
                            .ToList();

            var pairs = groupedByProject
                            .Select(e => e);

            foreach (var project in groupedByProject)
            {
                for(int i = 0; i < project.Count(); i++)
                {
                    for(int j = i + 1; j < project.Count(); j++)
                    {
                        bool overlap = project.ElementAt(i).DateFrom < project.ElementAt(j).DateTo && project.ElementAt(j).DateFrom < project.ElementAt(i).DateTo;
                        if (overlap)
                        {

                        }
                    }
                }

            }

            string result = "";

            return result;
        }
    }
}
