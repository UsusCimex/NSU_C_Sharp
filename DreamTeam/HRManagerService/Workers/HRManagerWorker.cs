using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HRManagerService.Workers
{
    public class HRManagerWorker : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Дополнительная логика при необходимости
            return Task.CompletedTask;
        }
    }
}
