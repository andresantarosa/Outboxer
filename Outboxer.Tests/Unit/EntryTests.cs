using System;
using FluentAssertions;
using Newtonsoft.Json;
using Outboxer.Enums;
using Outboxer.Models;
using Outboxer.Test.API.Entities;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Outboxer.Tests.Unit;

public class EntryTests
{
    [Fact]
    public void Instantiate_WithDefaultValues_ShoudlHaveIdNotEmpty()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        // Act
        Entry entry = new Entry("defaultQueue", content);

        // Assert
        entry.Id.Should().NotBe(Guid.Empty);
    }
    
    
    [Fact]
    public void Instantiate_WithDefaultValues_ShoudlHaveFutureDateSet()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        // Act
        Entry entry = new Entry("defaultQueue", content);

        // Assert
        entry.DateCreated.Should().NotBeAfter(DateTime.Now);
    }
    
    [Fact]
    public void Instantiate_WithDefaultValues_ShoudlHaveStatusPending()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        // Act
        Entry entry = new Entry("defaultQueue", content);

        // Assert
        entry.Status.Should().Be(StatusEnum.PENDING);
    }

    [Fact]
    public void Instantiate_WithDefaultValues_ShoudlHaveRetryEquals5()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        // Act
        Entry entry = new Entry("defaultQueue", content);

        // Assert
        entry.Retries.Should().Be(5);
    }

    [Fact]
    public void Instantiate_WithDestinationAndContent_ShoudlHaveCorrectDestinationAndContent()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var destination = "defaultQueue";
        var serializedContent = JsonSerializer.Serialize(content);
        // Act
        Entry entry = new Entry(destination, content);

        // Assert
        entry.Destination.Should().Be(destination);
        entry.Content.Should().BeEquivalentTo(serializedContent);
    }

    [Fact]
    public void Instantiate_WithDestinationAndContentAndRetries_ShoudlHaveCorrectDestinationAndContentAndRetries()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var destination = "defaultQueue";
        var serializedContent = JsonSerializer.Serialize(content);
        var retries = 10;
        // Act
        Entry entry = new Entry(destination, content, retries);

        // Assert
        entry.Destination.Should().Be(destination);
        entry.Content.Should().BeEquivalentTo(serializedContent);
        entry.Retries.Should().Be(retries);
    }

    [Fact]
    public void SetFailure_ShouldChangeStatusToFailuere()
    {
        // Arrange
        var entry = new Entry("defaultQueue", new
        {
            myProp1 = 1,
            myProp2 = "2"
        });

        // Act
        entry.SetFailure();
        
        // Assert
        entry.Status.Should().Be(StatusEnum.FAILED);
    }
    
    
    [Fact]
    public void SetFailure_ShouldChangeStatusToSuccess()
    {
        // Arrange
        var entry = new Entry("defaultQueue", new
        {
            myProp1 = 1,
            myProp2 = "2"
        });

        // Act
        entry.SetDelivered();
        
        // Assert
        entry.Status.Should().Be(StatusEnum.DELIVERED);
    }
    
     
    [Fact]
    public void IncreaseRetryCount_ShouldCorrectlyIncreaseRetryCount()
    {
        // Arrange
        var entry = new Entry("defaultQueue", new
        {
            myProp1 = 1,
            myProp2 = "2"
        });

        // Act
        entry.IncreaseRetryCount();
        entry.IncreaseRetryCount();
        
        // Assert
        entry.RetriesCount.Should().Be(2);
    }
    
    [Fact]
    public void TryAgain_WithRetryCountEquals0_ShouldReturnTrue()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var entry = new Entry("defaultQueue",content , 0);

        // Act
        var canRetry = entry.HasReachedRetryThreshold();
        
        // Assert
        canRetry.Should().BeFalse();
    }
    
    
    [Fact]
    public void TryAgain_WithNegativeRetryCount_ShouldReturnTrue()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var entry = new Entry("defaultQueue",content , -1);

        // Act
        var canRetry = entry.HasReachedRetryThreshold();
        
        // Assert
        canRetry.Should().BeFalse();
    }
    
    [Fact]
    public void TryAgain_WithUnreachedRetryCount_ShouldReturnTrue()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var entry = new Entry("defaultQueue",content, 3);
        entry.IncreaseRetryCount();
        entry.IncreaseRetryCount();

        // Act
        var canRetry = entry.HasReachedRetryThreshold();
        
        // Assert
        canRetry.Should().BeFalse();
    }
    
    [Fact]
    public void TryAgain_WithReachedRetryCount_ShouldReturnTrue()
    {
        // Arrange
        var content = new
        {
            myProp1 = 1,
            myProp2 = "2"
        };
        var entry = new Entry("defaultQueue",content, 3);
        entry.IncreaseRetryCount();
        entry.IncreaseRetryCount();
        entry.IncreaseRetryCount();

        // Act
        var canRetry = entry.HasReachedRetryThreshold();
        
        // Assert
        canRetry.Should().BeTrue();
    }

    [Fact]
    public void SetContent_ShouldSetContent()
    {
        // Arrange
        var content = new Student()
        {
            Name = "John Doe"
        };
        var entry = new Entry("defaultQueue",content, 3);

        // Act
        entry.SetContent(new Student()
        {
            Name = "Jane Doe"
        });
        
        // Assert
        JsonConvert.DeserializeObject<Student>(entry.Content).Name.Should().Be("Jane Doe");
    }
}