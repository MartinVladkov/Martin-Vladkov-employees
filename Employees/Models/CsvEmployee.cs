using CsvHelper.Configuration.Attributes;

namespace Employees.Models
{
    public class CsvEmployee
    {
        [Index(0)]
        public int Id { get; set; }

        [Index(1)]
        public int ProjectId { get; set; }

        [Index(2)]
        public string DateFrom { get; set; }

        [Index(3)]
        public string DateTo { get; set; }
    }
}
