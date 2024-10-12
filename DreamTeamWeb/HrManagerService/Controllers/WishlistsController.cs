using Microsoft.AspNetCore.Mvc;
using HrManagerService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace HrManagerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistsController : ControllerBase
    {
        private static readonly List<Wishlist> TeamLeadWishlists = new List<Wishlist>();
        private static readonly List<Wishlist> JuniorWishlists = new List<Wishlist>();
        private readonly IHttpClientFactory _httpClientFactory;

        public WishlistsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWishlist([FromBody] Wishlist wishlist)
        {
            if (wishlist.EmployeeId <= 5)
                TeamLeadWishlists.Add(wishlist);
            else
                JuniorWishlists.Add(wishlist);

            if (TeamLeadWishlists.Count == 5 && JuniorWishlists.Count == 5)
            {
                var teams = BuildTeams();

                await SendDataToHrDirector(teams);
            }

            return Ok();
        }

        private List<Team> BuildTeams()
        {
            var teams = new List<Team>();

            for (int i = 0; i < 5; i++)
            {
                teams.Add(new Team
                {
                    TeamLeadId = TeamLeadWishlists[i].EmployeeId,
                    JuniorId = JuniorWishlists[i].EmployeeId
                });
            }

            return teams;
        }

        private async Task SendDataToHrDirector(List<Team> teams)
        {
            var client = _httpClientFactory.CreateClient();
            var hrDirectorUrl = "http://hr-director-service:5000";

            var data = new
            {
                Wishlists = TeamLeadWishlists.Concat(JuniorWishlists),
                Teams = teams
            };

            await client.PostAsJsonAsync($"{hrDirectorUrl}/data", data);
        }
    }
}
