using Microsoft.EntityFrameworkCore;
using TheBoard.Application.Misc;
using TheBoard.Infrastructure.Data;
using TheBoard.Misc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString != null)
{
    var dbPath = connectionString.Replace("Data Source=", "");
    var directory = Path.GetDirectoryName(dbPath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }
}

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
    var prodUrl = builder.Configuration.GetValue<string>("BaseUrl");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins(prodUrl!)
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

// Yes, this is a bad practice to run migrations at startup, but this is a demo project running on an sqlite database.
// In a real production app, I would use the SQL script strategy mentioned <a href="https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#sql-scripts">here</a>.

var context = app.Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
await using var dbContext = context.CreateDbContext();
await dbContext.Database.MigrateAsync();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();