using CsvHelper.Configuration.Attributes;

namespace Employees.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public DateOnly DateFrom { get; set; }

        public DateOnly DateTo { get; set; }
    }
}
