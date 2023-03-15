using Employees.Models;

namespace Employees.Services.Employees
{
    public interface IProcessEmployeeService
    {
        public void WriteFileToLocalStorage(IFormFile file, string fileName);

        public List<Employee> MapEmployeesFromCsv(string fileName);

        public List<PairOfEmployees> GroupEmployeesByPairs(List<Employee> employees);

        public PairOfEmployees GetLongestWorkingPair(List<PairOfEmployees> pairsOfEmployees);
    }
}
