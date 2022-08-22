
# Outboxer  ![outboxerIcon](https://user-images.githubusercontent.com/32839273/185821189-73cce7bf-cce6-45f4-96b3-a50e5dc356f0.png)


Available at [Nuget](https://www.nuget.org/packages/Outboxer/) 

This lib intends to provide a simple and broker agnostic Outbox Pattern implementation.

The only requirement to use Outboxer is to use Entity Framework as we use its built-in Unit-Of-Work to assure everything is published once and guarantee consistency.

To use Outboxer follow these steps

* Make sure your DbContext inherits `OutboxerContext<T>`. If the name of your context is StudentsContext, your class should look like this.

```
public class StudentsContext : OutboxerContext<StudentsContext>
{
    public StudentsContext(DbContextOptions<StudentsContext> options,
        ITransactionalMessageContainer transactionalMessageContainer) : base(options, transactionalMessageContainer)
    {
    }

    // Your code
}
```

* Run `dotnet ef migrations add`/`dotnet ef database update` to create the outbox table

* Register Outboxer using `services.AddOutboxer<StudentsContext>();` at your Startup.cs or Program.cs(.net core 6+)

* To be broker agnostic means that you'll have to implement your own broker communication mechanism. Make sure your class implements the `IBrokerPublisher` interface. All your sending logic should be inside `Publish` method.

* Register your custom broker communication class on DI container using `services.AddScoped<IBrokerPublisher, MyBrokerImplementation>()`. You can use AddSingleton as well.

* To publish a message inject the interface `IPublisher` and use the `Publish` method like this `await _publisher.Publish(new Entry("studentsQueue", messageContent));`. The content will just be added to outbox and enqueued on your broker if the database commit runs fine.

***If the BackgroundWorker that sends the messages gets restarted it will re-enqueue ALL the messages with ENQUEUED status at the outbox table. This normaly just occurs when the app gets restarted, yet it is a good idea to implement idempotency at your consumers.***
