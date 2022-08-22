// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Moq;
// using Moq.AutoMock;
// using Outboxer.Channels;
// using Outboxer.Models;
// using Outboxer.Tests.Unit.Factory;
// using Outboxer.Transactional;
// using Xunit;
//
// namespace Outboxer.Tests.Unit;
//
// public class TransactionalMessageContainerTests
// {
//     private readonly AutoMocker _mocker;
//
//     public TransactionalMessageContainerTests()
//     {
//         _mocker = new AutoMocker();
//     }
//
//     [Fact]
//     public async Task DispatchSingleMessage_ShouldCallChannelSingleTime_WithNoErrors()
//     {
//         // Arrange
//         var transactionalContainer = _mocker.CreateInstance<TransactionalMessageContainer>();
//         Entry entry = EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe"));
//         await transactionalContainer.AddToPendingEntries(entry);
//         // Act
//         await transactionalContainer.Dispatch();
//         // Assert
//         _mocker.GetMock<IOutboxChannel>().Verify(x => x.Enqueue(entry.Id), Times.Once);
//     }
//     
//     [Fact]
//     public async Task Dispatch3Message_ShouldCallChannel3Times_WithNoErrors()
//     {
//         // Arrange
//         var transactionalContainer = _mocker.CreateInstance<TransactionalMessageContainer>();
//         List<Entry> entries = new List<Entry>()
//         {
//             EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("John", "Doe")),
//             EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("Jane", "Doe")),
//             EntryFactory.CreateEntry("myDestination", EntryFactory.CreateEntryPayload("Dick", "Doe"))
//         };
//         await transactionalContainer.AddToPendingEntries(entries);
//         // Act
//         await transactionalContainer.Dispatch();
//         // Assert
//         foreach (var entry in entries)
//         {
//             _mocker.GetMock<IOutboxChannel>().Verify(x => x.Enqueue(entry.Id), Times.Once);
//         }
//     }
// }