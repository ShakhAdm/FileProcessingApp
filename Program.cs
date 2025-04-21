using FileProcessingApp.Configuration;
using FileProcessingApp.Data;
using FileProcessingApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Processing API",
        Version = "v1"
    });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    c.EnableAnnotations();
});

builder.Services.Configure<FileProcessingOptions>(
    builder.Configuration.GetSection("FileProcessing"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("FileProcessingDb"));

builder.Services.AddSingleton<IFileProcessingQueue, FileProcessingQueue>();
builder.Services.AddHostedService<FileWatcherService>();
builder.Services.AddHostedService<FileProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Processing API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseAuthorization();
app.MapControllers();

app.Run();

