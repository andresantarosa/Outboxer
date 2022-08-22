using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Outboxer.Enums;
using Outboxer.Models;
using Outboxer.Outbox;
using Outboxer.Repository;
using Outboxer.Tests.Unit.Factory;
using Outboxer.Transactional;
using Xunit;

namespace Outboxer.Tests.Unit;

public class SenderTests
{
    private readonly AutoMocker _mocker;

    public SenderTests()
    {
        _mocker = new AutoMocker();
    }

    [Fact]
    public async Task PublishSingleEntry_ShouldAddToAddToRepositoryAndContainer_WithNoErrors()
    {
        // Arrange
        Publisher outboxer = _mocker.CreateInstance<Publisher>();
        Entry entry = EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe"));
        
        // Act
        await outboxer.Publish(entry);
        
        // Assert
        _mocker.GetMock<IEntryRepository>().Verify(x => x.Add(entry), Times.Once);
        _mocker.GetMock<ITransactionalMessageContainer>().Verify(x => x.AddToPendingEntries(entry), Times.Once);
    }
    
    [Fact]
    public async Task PublishEntryList_ShouldAddToAddToRepositoryAndContainer_WithNoErrors()
    {
        // Arrange
        Publisher outboxer = _mocker.CreateInstance<Publisher>();
        List<Entry> entries = new List<Entry>()
        {
            EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe")),
            EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("Jane", "Doe"))
        };
        
        // Act
        await outboxer.Publish(entries);
        
        // Assert
        _mocker.GetMock<IEntryRepository>().Verify(x => x.Add(entries), Times.Once);
        _mocker.GetMock<ITransactionalMessageContainer>().Verify(x => x.AddToPendingEntries(entries), Times.Once);
    }
    
    
    [Fact]
    public void SetDelivery_ShouldChangeDeliveryStatusAndUpdateDatabase_WithNoErrors()
    {
        // Arrange
        Publisher outboxer = _mocker.CreateInstance<Publisher>();
        Entry entry = EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe"));
        
        // Act
        outboxer.SetDelivered(entry);
        
        // Assert
        entry.Status.Should().Be(StatusEnum.DELIVERED);
        _mocker.GetMock<IEntryRepository>().Verify(x => x.Update(entry), Times.Once);
    }
    
    [Fact]
    public void SetFailure_ShouldIncreaseRetryCountStatusAndUpdateDatabase_WithNoErrors()
    {
        // Arrange
        Publisher outboxer = _mocker.CreateInstance<Publisher>();
        Entry entry = EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe"));
        
        // Act
        outboxer.SetFailure(entry);
        
        // Assert
        entry.Status.Should().Be(StatusEnum.PENDING);
        entry.RetriesCount.Should().Be(1);
        _mocker.GetMock<IEntryRepository>().Verify(x => x.Update(entry), Times.Once);
    }
}