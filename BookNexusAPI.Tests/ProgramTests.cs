using BookNexusAPI.Data;
using BookNexusAPI.Services.Books;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace BookNexusAPI.Tests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("program-tests"));
            });
        });
    }

    [Fact]
    public void Program_RegistersBookServiceAsScoped()
    {
        using var firstScope = factory.Services.CreateScope();
        using var secondScope = factory.Services.CreateScope();
        var firstService = firstScope.ServiceProvider.GetRequiredService<IBookService>();
        var sameScopeService = firstScope.ServiceProvider.GetRequiredService<IBookService>();
        var secondScopeService = secondScope.ServiceProvider.GetRequiredService<IBookService>();

        Assert.IsType<BookService>(firstService);
        Assert.Same(firstService, sameScopeService);
        Assert.NotSame(firstService, secondScopeService);
    }

    [Fact]
    public async Task Program_MapsDevelopmentOpenApiEndpoint()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
