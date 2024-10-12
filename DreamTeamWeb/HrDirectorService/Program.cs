using HrDirectorService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<HackathonContext>(options =>
    options.UseSqlite("Data Source=hackathon.db"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
