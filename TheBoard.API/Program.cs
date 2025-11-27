using Microsoft.EntityFrameworkCore;
using TheBoard.Application.Misc;
using TheBoard.Infrastructure.Data;
using TheBoard.Misc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddServices();

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins("https://your-production-domain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

// Use NSwag for OpenAPI generation
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TheBoard API";
    config.Version = "v1";
    config.SchemaSettings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();      // Serves the OpenAPI/Swagger document
    app.UseSwaggerUi();    // Serves the Swagger UI
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();