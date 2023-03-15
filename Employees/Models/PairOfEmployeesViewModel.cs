namespace Employees.Models
{
    public class PairOfEmployeesViewModel
    {
        public int EmployeeId1 { get; set; }

        public int EmployeeId2 { get; set; }

        public int Days { get; set; }

        public List<PairOfEmployees> PairsOfEmployees { get; set; } = new List<PairOfEmployees>();
    }
}
