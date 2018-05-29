using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageHub.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessageHub hub = MessageHub.Instance;

            var intSub = hub.Subscribe<int>(p =>
            {
                Console.WriteLine($"Id is: {p}");
            });

            Action<string> strAction = message => Console.WriteLine($"Message is: {message}");

            Action<Type, object> globalHandler = (t, obj) =>
            {
                Console.WriteLine($"GlobalHandler: Publish<{t.Name}>({obj.ToString()})");
            };
            hub.RegisterGlobalHandler(globalHandler);

            Action<Type, object> globalHandler2 = (t, obj) =>
            {
                Console.WriteLine($"LoggingHandler: Publish<{t.Name}>({obj.ToString()})");
            };
            hub.RegisterGlobalHandler(globalHandler2);

            hub.OnError += (sender, e) =>
            {
                Console.WriteLine($"OnError: {e.UserAction}({e.Token.ToString()}) throw Exception:{e.Exception.Message.ToString()}");
            };

            var strSub = hub.Subscribe(strAction);

            var orderConfirmationSub = hub.Subscribe<OrderConfirmation>(OrderAction);

            hub.Publish(new Order() { Id = "2001934102", Name = "lalala" });
            hub.Publish(new OrderConfirmation() { Id = "2001934102", Name = "lalala", Status = "Confirmed" });

            hub.Publish(123);
            hub.Publish("this is a string");
            Console.WriteLine($"Hub.IsRegistered<int>: {hub.IsRegistered<int>()}");

            Console.WriteLine();
            Console.WriteLine("Registerd Type List: ");
            hub.GetRegisteredType().ToList().ForEach(f => { Console.WriteLine(f.Name); });
            Console.WriteLine();

            hub.UnSubscribe(intSub);
            hub.Publish(0987654321);
            Console.WriteLine($"Hub.GetGeneration: {GC.GetGeneration(hub)}");

            Console.WriteLine($"Hub.IsRegistered<int>: {hub.IsRegistered(0987654321)}");
            Console.WriteLine($"Hub.IsRegistered<int>: {hub.IsRegistered((0987654321).GetType())}");
            Console.WriteLine($"Hub.IsRegistered<typeof(int)>: {hub.IsRegistered(typeof(int))}");

            Console.ReadKey();
        }

        public static void OrderAction(Order o)
        {
            Console.WriteLine($"OrderId: {o.Id}, Name: {o.Name}");
        }

        public static void OrderConfirmationAction(OrderConfirmation oc)
        {
            Console.WriteLine($"OrderId: {oc.Id}, Name: {oc.Name}, Status: {oc.Status}");
        }
    }
}
