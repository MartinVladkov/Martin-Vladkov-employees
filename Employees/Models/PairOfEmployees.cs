namespace Employees.Models
{
    public class PairOfEmployees
    {
        public int Id { get; set; }

        public int EmployeeId1 { get; set; }

        public int EmployeeId2 { get; set; }

        public int Days { get; set; }

        public List<int> Projects { get; set; } = new List<int>();
    }
}
