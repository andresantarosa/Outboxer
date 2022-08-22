using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Outboxer.Enums;
using Outboxer.FakeEnqueuer;
using Outboxer.Models;
using Outboxer.Test.API.Context;
using Outboxer.Test.API.Entities;
using Xunit;

namespace Outboxer.Tests.Integration;

public class Testes : Fixture
{
    [Fact]
    public async Task CallEnqueuer_ShouldAddEntryToOutboxAndDispatchContentAsync()
    {
        // Arrange
        var scope = Application.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudentsContext>();
        var randomName = Helpers.RandomString(20);
        var payload = new
        {
            Name = randomName
        };
        Enqueuer.ReturnStatus = true;
        // Act
        await HttpClient.PostAsJsonAsync("Student", payload);
    
        // Assert
        context.Set<Student>().FirstOrDefault(x => x.Name == randomName).Should().NotBeNull();
        await Task.Delay(1000);
        context.Set<Entry>().FirstOrDefault(x => x.Content.Contains(randomName)).Status.Should().Be(StatusEnum.DELIVERED);
    }
    
    [Fact]
    public async Task CallEnqueuer_ShouldAddEntryToOutboxAndDispatchContentSync()
    {
        // Arrange
        var scope = Application.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudentsContext>();
        var randomName = Helpers.RandomString(20);
        var payload = new
        {
            Name = randomName
        };
        Enqueuer.ReturnStatus = true;
        // Act
        await HttpClient.PostAsJsonAsync("Student/sync", payload);
    
        // Assert
        context.Set<Student>().FirstOrDefault(x => x.Name == randomName).Should().NotBeNull();
        await Task.Delay(1000);
        context.Set<Entry>().FirstOrDefault(x => x.Content.Contains(randomName)).Status.Should().Be(StatusEnum.DELIVERED);
    }
    
    [Fact]
    public async Task CallEnqueuer_ShouldNotAddEntryToOutbox()
    {
        // Arrange
        var scope = Application.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudentsContext>();
        var randomName = Helpers.RandomString(22);
        var payload = new
        {
            Name = randomName
        };
        Enqueuer.ReturnStatus = true;
        // Act
        await HttpClient.PostAsJsonAsync("Student", payload);
    
        // Assert
        context.Set<Student>().FirstOrDefault(x => x.Name == randomName).Should().BeNull();
        await Task.Delay(1000);
        context.Set<Entry>().FirstOrDefault(x => x.Content.Contains(randomName)).Should().BeNull();
    }
    
    [Fact]
    public async Task CallEnqueuer_ShouldAddAddEntryToOutboxAndRetryDispatchingUntilLimit()
    {
        // Arrange
        var scope = Application.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudentsContext>();
        var randomName = Helpers.RandomString(20);
        var payload = new
        {
            Name = randomName
        };
        Enqueuer.ReturnStatus = false;
        // Act
        await HttpClient.PostAsJsonAsync("Student", payload);
    
        // Assert
        context.Set<Student>().FirstOrDefault(x => x.Name == randomName).Should().NotBeNull();
        await Task.Delay(1000);
        context.Set<Entry>().FirstOrDefault(x => x.Content.Contains(randomName)).Status.Should().Be(StatusEnum.FAILED);
        context.Set<Entry>().FirstOrDefault(x => x.Content.Contains(randomName)).RetriesCount.Should().Be(5);


    }
}