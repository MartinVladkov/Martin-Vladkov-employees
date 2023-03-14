using CsvHelper.Configuration.Attributes;

namespace Employees.Models
{
    public class Employee
    {
        [Index(0)]
        public int Id { get; set; }

        [Index(1)]
        public int ProjectId { get; set; }

        [Index(2)]
        public DateOnly DateFrom { get; set; }

        [Index(3)]
        public DateOnly DateTo { get; set; }
    }
}
