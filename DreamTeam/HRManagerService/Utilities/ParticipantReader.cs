using System.Collections.Generic;
using System.IO;
using Shared.Models;

namespace HRManagerService.Utilities
{
    public static class ParticipantReader
    {
        public static List<Employee> ReadParticipants(string filePath, EmployeeRole role)
        {
            var participants = new List<Employee>();
            Console.WriteLine("Parse from: " + Path.Combine(Directory.GetCurrentDirectory(), filePath));
            var lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), filePath));
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    int id = int.Parse(parts[0]);
                    string name = parts[1];
                    participants.Add(new Employee(id, name, role));
                }
            }
            return participants;
        }
    }
}
