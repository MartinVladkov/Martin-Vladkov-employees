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
        public IActionResult ProcessEmployees()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ProcessEmployees(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            //List<Employee> values = ReadAsList(file)
            //                            .Select(v => MapEmployeeFromCsv(v))
            //                            .ToList();

            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using(FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream); 
                fileStream.Flush();
            }
            var employees = MapEmployeeFromCsv(fileName);

            return View();
        }

        private List<Employee> MapEmployeeFromCsv(string fileName)
        {
            List<Employee> employees = new List<Employee>();

            var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fileName;

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var employee = csv.GetRecord<Employee>();
                    employees.Add(employee);
                }
            }

            //string[] values = csvLine.Split(',');

            //employee.Id = Convert.ToInt32(values[0]);
            //employee.ProjectId = Convert.ToInt32(values[1]);
            //employee.DateFrom = DateOnly.FromDateTime(Convert.ToDateTime(values[3]));
            //employee.DateTo = DateOnly.FromDateTime(Convert.ToDateTime(values[4]));

            return employees;
        }
    }
}
