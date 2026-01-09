using Devkit.Common.Caching.Extensions;
using Devkit.Common.Identity.Extensions;
using Devkit.Common.Jobs.Extensions;
using Devkit.Common.Logging.Extensions;
using Devkit.Common.Messaging.Extensions;
using Devkit.Sample.Api.Data;
using Devkit.Sample.Api.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMessagingWithOutbox<AppDbContext>(
    builder.Configuration,
    consumerAssembly: typeof(Program).Assembly,
    useConsumers: true
);

builder.Services.AddCacheProvider(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration, builder.Environment); //Keycloak
//builder.Services.AddIdentity<AppDbContext>(builder.Configuration, builder.Environment); AspnetIdentity
builder.Services.AddJobScheduler(builder.Configuration);
builder.Services.AddTransient<DateTimeLoggerJob>();
builder.Services.AddCustomLogging(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Devkit API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();
app.UseHangfireDashboard();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();