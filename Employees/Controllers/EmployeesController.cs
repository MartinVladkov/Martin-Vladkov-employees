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
                    employee.DateFrom = Convert.ToDateTime(csvEmployee.DateFrom);

                    if (csvEmployee.DateTo.ToLower() == "null")
                    {
                        employee.DateTo = DateTime.Now;
                    }
                    else
                    {
                        employee.DateTo = Convert.ToDateTime(csvEmployee.DateTo);
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

            List<PairOfEmployees> pairs = new List<PairOfEmployees>();

            foreach (var project in groupedByProject)
            {
                for (int i = 0; i < project.Count(); i++)
                {
                    for (int j = i + 1; j < project.Count(); j++)
                    {
                        var employeeDateFrom1 = project.ElementAt(i).DateFrom;
                        var employeeDateTo1 = project.ElementAt(i).DateTo;
                        var employeeDateFrom2 = project.ElementAt(j).DateFrom;
                        var employeeDateTo2 = project.ElementAt(j).DateTo;

                        bool overlap = employeeDateFrom1 < employeeDateTo2 && employeeDateFrom2 < employeeDateTo1;

                        if (overlap)
                        {
                            var pair = new PairOfEmployees();
                            pair.EmployeeId1 = project.ElementAt(i).Id;
                            pair.EmployeeId2 = project.ElementAt(j).Id;
                            pair.Days = (int)(((employeeDateTo1 < employeeDateTo2 ? employeeDateTo1 : employeeDateTo2) - 
                                             (employeeDateFrom1 > employeeDateFrom2 ? employeeDateFrom1 : employeeDateFrom2)).TotalDays + 1);
                            pair.Id = CalculatePairId(pair);
                            pair.Projects.Add(project.ElementAt(i).ProjectId);

                            pairs.Add(pair);
                        }
                    }
                }
            }

            var temp = pairs
                .GroupBy(p => p.Id)
                .ToList();

            var opa = temp
                .Select(g => new
                {
                    Id = g.First().Id,
                    EmployeeId1 = g.First().EmployeeId1,
                    EmployeeId2 = g.First().EmployeeId2,
                    Days = g.Sum(s => s.Days)
                })
                .ToList();

            //maybe nov model v koito nqma list of projects;

        
            string result = "";

            return result;
        }

        private int CalculatePairId(PairOfEmployees pair)
        {
            string stringId = (Math.Min(pair.EmployeeId1, pair.EmployeeId2)).ToString() + (Math.Max(pair.EmployeeId1, pair.EmployeeId2)).ToString();

            int resultId = int.Parse(stringId);

            return resultId;
        }
    }
}
