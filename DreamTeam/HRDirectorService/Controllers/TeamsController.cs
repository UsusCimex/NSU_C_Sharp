using Microsoft.AspNetCore.Mvc;
using Shared.Messages;

namespace HRDirectorService.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> _logger;
        private readonly Services.HRDirectorHostedService _hrDirectorService;

        public TeamsController(ILogger<TeamsController> logger, Services.HRDirectorHostedService hrDirectorService)
        {
            _logger = logger;
            _hrDirectorService = hrDirectorService;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveTeams([FromBody] HRManagerData data)
        {
            _logger.LogInformation("Получены данные от HRManager");
            await _hrDirectorService.ProcessHRManagerData(data);
            return Ok();
        }
    }
}
