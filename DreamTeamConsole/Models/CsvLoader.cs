namespace DreamTeamConsole.Models {
    public class CsvLoader
    {
        public static List<Employee> LoadEmployeesFromCsv(string filePath, Func<int, string, Employee> employeeFactory)
        {
            var employees = new List<Employee>();
            
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    var values = line.Split(';');
                    
                    int id = int.Parse(values[0]);
                    string name = values[1];

                    var employee = employeeFactory(id, name);
                    employees.Add(employee);
                }
            }
            
            return employees;
        }
    }
}