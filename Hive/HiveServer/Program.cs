using HiveServer.Repository;
using ZLogger;
using ZLogger.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5229");

IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IRedisDB, RedisDB>();

builder.Services.AddLogging();

builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();

builder.Logging.AddZLoggerRollingFile(options =>
{
    options.FilePathSelector = (timestamp, sequenceNumber) => $"logs/{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";

    options.RollingInterval = RollingInterval.Day;

    options.RollingSizeKB = 1024;
});


var app = builder.Build();

app.MapControllers();

app.Run();
