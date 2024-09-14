namespace DreamTeamConsole.Models
{
    public class Hackaton
    {
        public List<TeamLead> TeamLeads { get; }
        public List<Junior> Juniors { get; }

        public Hackaton(string teamLeadsCsvPath, string juniorsCsvPath)
        {
            TeamLeads = CsvLoader.LoadEmployeesFromCsv(teamLeadsCsvPath, (id, name) => new TeamLead(id, name))
                .Cast<TeamLead>()
                .ToList();
            Juniors = CsvLoader.LoadEmployeesFromCsv(juniorsCsvPath, (id, name) => new Junior(id + 20, name)) // Костыль
                .Cast<Junior>()
                .ToList();

            if (TeamLeads.Count != Juniors.Count) 
            {
                throw new Exception("Тимлидов и джунов должно быть одинаковое количество!");
            }

            if (TeamLeads.Count == 0) 
            {
                throw new Exception("В хакатоне должно учавствовать хотябы 2 участника!");
            }
        }

        public Hackaton(List<TeamLead> teamLeads, List<Junior> juniors)
        {
            TeamLeads = teamLeads;
            Juniors = juniors;
        }
    }
}
