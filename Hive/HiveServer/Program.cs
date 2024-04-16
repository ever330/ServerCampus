using HiveServer.DB;
using ZLogger;
using ZLogger.Providers;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IAccountDB, AccountDB>();

builder.Services.AddLogging();
builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();

builder.Logging.AddZLoggerFile("HiveServerLog.log");
builder.Logging.AddZLoggerRollingFile(options =>
{
    options.FilePathSelector = (timestamp, sequenceNumber) => $"logs/{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";

    options.RollingInterval = RollingInterval.Day;

    options.RollingSizeKB = 1024;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
