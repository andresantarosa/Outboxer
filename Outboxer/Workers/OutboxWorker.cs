using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outboxer.Channels;
using Outboxer.Enums;
using Outboxer.External;
using Outboxer.Outbox;
using Outboxer.Repository;

namespace Outboxer.Workers;

public class OutboxWorker<TContext> : BackgroundService where TContext: DbContext
{
    private readonly IOutboxChannel _outboxChannel;
    private readonly IServiceProvider _serviceProvider;

    public OutboxWorker(IOutboxChannel outboxChannel, IServiceProvider serviceProvider)
    {
        _outboxChannel = outboxChannel;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var entryRepository = scope.ServiceProvider.GetRequiredService<IEntryRepository>();
            var pendingMessages = await entryRepository.GetEnqueued();
            foreach (var message in pendingMessages)
                await _outboxChannel.Enqueue(message.Id);
        }

        
        while (!_outboxChannel.EnqueuedItems.Reader.Completion.IsCompleted)
        {
            var entryId =await _outboxChannel.Peek();

            using (var scope = _serviceProvider.CreateScope())
            {
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IEntryRepository>();
                var brokerPublisher = scope.ServiceProvider.GetRequiredService<IBrokerPublisher>();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                var entry = await outboxRepository.Get(entryId);
                
                if(await brokerPublisher.Publish(entry))
                    publisher.SetDelivered(entry);
                else
                    publisher.SetFailure(entry);
                
                await context.SaveChangesAsync();

                if (entry.Status == StatusEnum.ENQUEUED)
                    await _outboxChannel.Enqueue(entryId);
            }
        }
    }
}