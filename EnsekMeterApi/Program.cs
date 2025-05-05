using EnsekMeter.Services;
using EnsekMeter.Validation;
using EnsekMeterApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<EnsekMeterApi.Data.MeterDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMeterReadingRepository, SqlMeterReadingRepository>();
builder.Services.AddScoped<IMeterReadingUploadService, MeterReadingUploadService>();
builder.Services.AddScoped<IMeterReadingValidator, MeterReadingAccountValidator>();
builder.Services.AddScoped<IMeterReadingValidator, MeterReadingValueValidator>();
builder.Services.AddScoped<IMeterReadingValidator, NewerThanLatestReadingValidator>();
builder.Services.AddScoped<IMeterReadingValidator, DuplicateReadingValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();