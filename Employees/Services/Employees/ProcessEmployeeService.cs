using CsvHelper;
using Employees.Models;
using System.Globalization;

namespace Employees.Services.Employees
{
    public class ProcessEmployeeService : IProcessEmployeeService
    {
        public void WriteFileToLocalStorage(IFormFile file, string fileName)
        {
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
        }

        public List<Employee> MapEmployeesFromCsv(string fileName)
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

                    if (csvEmployee.Id == 0 || csvEmployee.ProjectId == 0 || csvEmployee.DateFrom.ToLower() == "null" || csvEmployee.DateFrom == "")
                    {
                        continue;
                    }

                    employee.Id = csvEmployee.Id;
                    employee.ProjectId = csvEmployee.ProjectId;

                    string[] formats = {"yy/MM/dd", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "MM/DD/YY", "DD/MM/YY", "YY/MM/DD", 
                        "M/D/YY", "D/M/YY", "YY/M/D", "M-d-yyyy", "dd-MM-yyyy", "MM-dd-yyyy", "M.d.yyyy", "dd.MM.yyyy", "MM.dd.yyyy", "dd/MM/yy" };
                    employee.DateFrom = DateTime.ParseExact(csvEmployee.DateFrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    if (csvEmployee.DateTo.ToLower() == "null")
                    {
                        employee.DateTo = DateTime.Now;
                    }
                    else
                    {
                        employee.DateTo = DateTime.ParseExact(csvEmployee.DateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None); ;
                    }

                    employees.Add(employee);
                }
            }

            return employees;
        }

        public List<PairOfEmployees> GroupEmployeesByPairs(List<Employee> employees)
        {
            var groupedByProject = employees
                            .Select(e => e)
                            .GroupBy(e => e.ProjectId)
                            .ToList();

            List<PairOfEmployees> pairsOfEmployees = new List<PairOfEmployees>();

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
                            pair.Projects.Add(project.ElementAt(i).ProjectId);
                            pair.Id = CalculatePairId(pair);

                            pairsOfEmployees.Add(pair);
                        }
                    }
                }
            }

            return pairsOfEmployees;
        }

        public PairOfEmployees GetLongestWorkingPair(List<PairOfEmployees> pairsOfEmployees)
        {
            var groupedByPairs = pairsOfEmployees
                .GroupBy(g => g.Id)
                .Select(g => new PairOfEmployees
                {
                    Id = g.First().Id,
                    EmployeeId1 = g.First().EmployeeId1,
                    EmployeeId2 = g.First().EmployeeId2,
                    Days = g.Sum(s => s.Days)
                })
                .OrderByDescending(s => s.Days)
                .ToList();

            var longestWorkingPair = groupedByPairs.FirstOrDefault();

            return longestWorkingPair;
        }

        private int CalculatePairId(PairOfEmployees pair)
        {
            string stringId = (Math.Min(pair.EmployeeId1, pair.EmployeeId2)).ToString() + (Math.Max(pair.EmployeeId1, pair.EmployeeId2)).ToString();

            int resultId = int.Parse(stringId);

            return resultId;
        }
    }
}
