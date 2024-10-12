using Microsoft.AspNetCore.Mvc;
using HrDirectorService.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace HrDirectorService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly HackathonContext _context;
        private readonly ILogger<DataController> _logger;

        public DataController(HackathonContext context, ILogger<DataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult ReceiveData([FromBody] HrData data)
        {
            _context.Teams.AddRange(data.Teams);
            _context.Wishlists.AddRange(data.Wishlists);
            _context.SaveChanges();

            var harmonyScore = CalculateHarmonyScore(data.Teams, data.Wishlists);

            _logger.LogInformation($"Harmony Score: {harmonyScore}");

            return Ok();
        }

        private double CalculateHarmonyScore(List<Team> teams, List<Wishlist> wishlists)
        {
            double totalSatisfaction = 0;
            int participantCount = 0;

            foreach (var team in teams)
            {
                var teamLeadWishlist = wishlists.FirstOrDefault(w => w.EmployeeId == team.TeamLeadId);
                var juniorWishlist = wishlists.FirstOrDefault(w => w.EmployeeId == team.JuniorId);

                int teamLeadSatisfaction = CalculateSatisfaction(teamLeadWishlist, team.JuniorId);
                int juniorSatisfaction = CalculateSatisfaction(juniorWishlist, team.TeamLeadId);

                if (teamLeadSatisfaction > 0)
                {
                    totalSatisfaction += teamLeadSatisfaction;
                    participantCount++;
                }

                if (juniorSatisfaction > 0)
                {
                    totalSatisfaction += juniorSatisfaction;
                    participantCount++;
                }
            }

            return participantCount != 0 ? totalSatisfaction / participantCount : 0;
        }

        private int CalculateSatisfaction(Wishlist wishlist, int partnerId)
        {
            if (wishlist == null) return 0;

            int index = wishlist.DesiredEmployeeIds.IndexOf(partnerId);
            return index >= 0 ? 5 - index : 0;
        }
    }

    public class HrData
    {
        public List<Wishlist> Wishlists { get; set; }
        public List<Team> Teams { get; set; }
    }
}
