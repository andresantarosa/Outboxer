using Microsoft.Extensions.DependencyInjection;
using Outboxer.Channels;
using Outboxer.Context;
using Outboxer.Outbox;
using Outboxer.Repository;
using Outboxer.Transactional;
using Outboxer.Workers;

namespace Outboxer;

public static class Initialize
{
    public static void AddOutboxer<TContext>(this IServiceCollection serviceCollection) where TContext : OutboxerContext<TContext>
    {
        serviceCollection.AddScoped<IEntryRepository, EntryRepository<TContext>>();
        serviceCollection.AddScoped<IPublisher, Publisher>();
        serviceCollection.AddScoped<ITransactionalMessageContainer, TransactionalMessageContainer>();
        serviceCollection.AddSingleton<IOutboxChannel, OutboxChannel>();
        serviceCollection.AddHostedService<OutboxWorker<TContext>>();
    }
}