using BookNexusAPI.Data;
using BookNexusAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext with SQL Server, reading the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our Service: "when someone asks for IBookService, give them BookService".
// Scoped = one instance per HTTP request (the right lifetime for services using DbContext).
builder.Services.AddScoped<IBookService, BookService>();

//Enable Controllers
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); //connects [Route] attributes on controllers to the pipeline

app.Run();
