using Microsoft.AspNetCore.Mvc;
using DeveloperService.Models;
using DeveloperService.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DeveloperService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeveloperController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DeveloperController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("send-wishlist")]
        public async Task<IActionResult> SendWishlist()
        {
            string type = Environment.GetEnvironmentVariable("TYPE");
            int id = int.Parse(Environment.GetEnvironmentVariable("ID"));

            var wishlist = await GenerateWishlistAsync(type, id);

            var client = _httpClientFactory.CreateClient();
            var hrManagerUrl = _configuration["HrManagerUrl"];

            var response = await client.PostAsJsonAsync($"{hrManagerUrl}/wishlists", wishlist);

            if (response.IsSuccessStatusCode)
                return Ok("Wishlist sent successfully.");
            else
                return StatusCode((int)response.StatusCode, "Failed to send wishlist.");
        }

        private async Task<Wishlist> GenerateWishlistAsync(string type, int id)
        {
            var filePath = type == "teamlead" ? "../csvFiles/Juniors5.csv" : "../csvFiles/Teamleads5.csv";
            var employees = CsvLoader.LoadEmployeesFromCsv(filePath);

            var random = new Random();
            var preferences = employees.OrderBy(e => random.Next()).Select(e => e.Id).ToList();

            return new Wishlist
            {
                EmployeeId = id,
                DesiredEmployeeIds = preferences
            };
        }
    }
}
