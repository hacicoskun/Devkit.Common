using Devkit.Common.Caching.Extensions;
using Devkit.Common.Jobs.Extensions;
using Devkit.Common.Messaging.Extensions;
using Devkit.Sample.Api.Data;
using Devkit.Sample.Api.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMessagingWithOutbox<AppDbContext>(
    builder.Configuration,
    consumerAssembly: typeof(Program).Assembly,
    useConsumers: true
);

builder.Services.AddCacheProvider(builder.Configuration);

builder.Services.AddJobScheduler(builder.Configuration); 
builder.Services.AddTransient<DateTimeLoggerJob>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHangfireDashboard();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 

app.MapControllers();

app.Run();