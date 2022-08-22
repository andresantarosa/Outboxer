using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Outboxer.Test.API.Context;
using Xunit;

namespace Outboxer.Tests.Integration;

public class Fixture : IClassFixture<WebApplicationFactory<Program>>
{
    protected HttpClient HttpClient;
    protected WebApplicationFactory<Program> Application;

    public Fixture()
    {
        Application = new WebApplicationFactory<Program>();
        HttpClient = Application.CreateClient();

        using (var scope = Application.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<StudentsContext>();
            context.Database.Migrate();
        }
    }
}