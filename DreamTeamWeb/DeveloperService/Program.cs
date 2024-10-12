using DeveloperService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<WishlistSenderService>();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
