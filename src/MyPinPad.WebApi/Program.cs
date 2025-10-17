using MyPinPad.WebApi.Extensions;
using MyPinPad.WebApi.Middlewares;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
         options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
     });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    Converters = { new JsonStringEnumConverter() },
});


builder.Services.AddMyPinPadServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseMiddleware<BadRequestMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
