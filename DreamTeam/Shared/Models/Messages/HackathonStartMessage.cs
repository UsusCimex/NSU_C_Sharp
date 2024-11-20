using System.Collections.Generic;
using Shared.Models;

namespace Shared.Messages
{
    public class HackathonStartMessage
    {
        public int HackathonId { get; set; }
        public List<Employee> TeamLeads { get; set; }
        public List<Employee> Juniors { get; set; }

        public HackathonStartMessage()
        {
            TeamLeads = new List<Employee>();
            Juniors = new List<Employee>();
        }
    }
}
