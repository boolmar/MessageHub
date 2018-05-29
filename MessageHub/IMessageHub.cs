using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    /// <summary>
    /// An implementation of the <c>Event Aggregator</c> pattern.
    /// </summary>
    public interface IMessageHub : IDisposable
    {
        /// <summary>
        /// Publishes the <paramref name="message"/> on the <see cref="IMessageHub"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message to published</param>
        void Publish<T>(T message);

        /// <summary>
        /// Subscribes a callback against the <see cref="IMessageHub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="IMessageHub"/></param>
        /// <returns>The subscription</returns>
        Subscription<T> Subscribe<T>(Action<T> action);

        /// <summary>
        /// Subscribes a callback against the <see cref="MessageHub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="MessageHub"/></param>
        /// <param name="throttleBy">The <see cref="TimeSpan"/> specifying the rate at which subscription is throttled</param>
        /// <returns><c>True</c> if UnSubscribe is success otherwise <c>False</c></returns>
        bool UnSubscribe<T>(Subscription<T> subscription);

        /// <summary>
        /// Checks if a specific Type is active on the <see cref="IMessageHub"/>.
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns></returns>
        bool IsRegistered(Type t);
        /// <summary>
        /// Checks if a specific Type is active on the <see cref="IMessageHub"/>.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns><c>True</c> if the Type is active otherwise <c>False</c></returns>
        bool IsRegistered<T>();

        /// <summary>
        /// Checks if a specific Type is active on the <see cref="IMessageHub"/>.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">object of T</param>
        /// <returns><c>True</c> if the Type is active otherwise <c>False</c></returns>
        bool IsRegistered<T>(T obj);

        /// <summary>
        /// Return registered type count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get registered type
        /// </summary>
        /// <returns>return registered type</returns>
        IEnumerable<Type> GetRegisteredType();

        /// <summary>
        /// Clears all the subscriptions from the <see cref="IMessageHub"/>.
        /// <remarks>The global handler and the <see cref="OnError"/> are not affected</remarks>
        /// </summary>
        void ClearSubscriptions();

        /// <summary>
        /// Registers a callback which is invoked on every message published by the <see cref="IMessageHub"/>.
        /// <remarks>Invoking this method with a new <paramref name="onMessage"/>overwrites the previous one.</remarks>
        /// </summary>
        /// <param name="onMessage">
        /// The callback to invoke on every message
        /// <remarks>The callback receives the type of the message and the message as arguments</remarks>
        /// </param>
        void RegisterGlobalHandler(Action<Type, object> onMessage);

        /// <summary>
        /// Invoked if an error occurs when publishing the message to a subscriber.
        /// </summary>
        event EventHandler<MessageHubErrorEventArgs> OnError;
    }

}
