using Microsoft.Extensions.DependencyInjection;
using Outboxer.External;

namespace Outboxer.FakeEnqueuer;

public static class EnqueuerConfigurer
{
    public static void UseFakeEnqueuer(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBrokerPublisher, Enqueuer>();
    }
}
