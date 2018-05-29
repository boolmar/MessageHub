# MessageHub
An implementation of the Event Aggregator Pattern.

#### NuGet package: [Bool.MessageHub](https://www.nuget.org/packages/Bool.MessageHub/)

### Usage example:

Start by getting the singleton instance of the hub:
```csharp
var hub = MessageHub.Instance;
```

You can now use the hub to subscribe to any publication of a given type:
```csharp
var token = hub.Subscribe<Person>(p => Console.WriteLine($"Id is: {p.Id}"));
// or    
Action<string> action = message => Console.WriteLine($"Message is: {message}");
var anotherToken = hub.Subscribe(action);
```
You can then use the token to do:

```csharp
hub.IsSubscribed(token); // returns true
hub.UnSubscribe(token);
hub.IsSubscribed(token); // returns false
```
Or you can clear all subscriptions by:
```csharp
hub.ClearSubscriptions();
```
Publication is as easy as:

```csharp
hub.Publish(new Person { Id = "Foo" });
hub.Publish("An important message");
```

#### Error handling:
The hub catches any exception thrown at the time of publication and exposes them by:
```csharp
hub.OnError += (sender, eArgs)
        => Console.WriteLine($"Error Publishing, Token: {eArgs.Token} | Exception: {eArgs.Exception}");
```

#### Global handler:
The hub allows the registration of a single handler which will receive every message published by the hub. This can be useful in scenarios where every message published should be logged or audited.

```csharp
hub.RegisterGlobalHandler((type, eventObject) => Console.WriteLine($"Type: {type} - Event: {eventObject}"));
```

#### Inheritance support:
The hub supports inheritance by allowing to subscribe to a base class or an interface and receiving all the publications of types that inherit or implement the subscribed type. For example, given:

```csharp
public class Order {}
public class NewOrder : Order{}
public class BigOrder : Order{}
```

A subscriber registering against `Ordrer` will also receive events of type `NewOrder` and `BigOrder`.
#### Reference:
#1. http://www.nimaara.com/2016/02/14/cleaner-pub-sub-using-the-event-aggregator-pattern/

#2. https://github.com/NimaAra/Easy.MessageHub/blob/master/README.md

#3. https://www.codeproject.com/Articles/866547/Publisher-Subscriber-pattern-with-Event-Delegate-a
