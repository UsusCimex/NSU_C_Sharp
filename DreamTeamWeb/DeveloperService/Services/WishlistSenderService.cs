using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperService.Services
{
    public class WishlistSenderService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WishlistSenderService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<Controllers.DeveloperController>();
            await controller.SendWishlist();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
