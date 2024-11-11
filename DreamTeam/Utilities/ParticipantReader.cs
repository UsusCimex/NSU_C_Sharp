using DreamTeam.Models;

namespace DreamTeam.Utilities
{
    public static class ParticipantReader
    {
        public static List<Employee> ReadParticipants(string filePath)
        {
            var participants = new List<Employee>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1)) // Пропускаем заголовок
            {
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    int id = int.Parse(parts[0]);
                    string name = parts[1];

                    participants.Add(new Employee(id, name));
                }
            }

            return participants;
        }
    }
}
