using System;

namespace HRDirectorService.Models
{
    public class HackathonResult
    {
        public int Id { get; set; }
        public int HackathonId { get; set; }
        public double AverageHappiness { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
