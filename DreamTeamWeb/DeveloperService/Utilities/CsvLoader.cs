using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeveloperService.Utilities
{
    public static class CsvLoader
    {
        public static List<Employee> LoadEmployeesFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines.Select((line, index) => new Employee
            {
                Id = index + 1,
                Name = line.Trim()
            }).ToList();
        }

        public static List<int> LoadPreferencesFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines.Select(line => int.Parse(line.Trim())).ToList();
        }
    }
}
