using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Models;
using System.Threading.Tasks;

namespace HRManagerService.Consumers
{
    public class WishlistConsumer : IConsumer<Wishlist>
    {
        private readonly ILogger<WishlistConsumer> _logger;
        private readonly Services.HRManagerService _hrManagerService;

        public WishlistConsumer(ILogger<WishlistConsumer> logger, Services.HRManagerService hrManagerService)
        {
            _logger = logger;
            _hrManagerService = hrManagerService;
        }

        public Task Consume(ConsumeContext<Wishlist> context)
        {
            var wishlist = context.Message;
            _logger.LogInformation($"HRManager: Получен вишлист от сотрудника {wishlist.EmployeeId} для хакатона {wishlist.HackathonId}");
            _hrManagerService.ProcessWishlist(wishlist);
            return Task.CompletedTask;
        }
    }
}
