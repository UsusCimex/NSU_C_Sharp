using MassTransit;
using Microsoft.EntityFrameworkCore;
using HRDirectorService.Database;
using HRDirectorService.Services;

namespace HRDirectorService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Другие сервисы
            services.AddControllers();

            // Настройка базы данных
            services.AddDbContext<HackathonDbContext>(options =>
                options.UseMySql(
                    "Server=mysql;Database=hackathon_db;User=root;Password=pass;",
                    ServerVersion.AutoDetect("Server=mysql;Database=hackathon_db;User=root;Password=pass;")
                )
            );


            // Настройка MassTransit
            services.AddMassTransit(x =>
            {
                // Configure MassTransit here
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });

            // Регистрация HRDirectorService
            services.AddSingleton<HRDirectorHostedService>();
            services.AddHostedService<HRDirectorHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<HackathonDbContext>();
                context.Database.EnsureCreated();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
