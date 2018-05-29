using System;

namespace MessageHub
{
    /// <summary>
    /// A class representing an error event raised by the <see cref="IMessageHub"/>
    /// </summary>
    public sealed class MessageHubErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of the <see cref="MessageHubErrorEventArgs"/>
        /// </summary>
        /// <param name="token">
        /// <param name="_userAction">
        /// <param name="ex">The exception thrown by the <see cref="IMessageHub"/></param>
        /// The subscription token of the subscriber to which 
        /// message was published by the <see cref="IMessageHub"/>
        /// </param>
        public MessageHubErrorEventArgs(object sender, string _userAction, Exception ex)
        {
            Token = sender;
            UserAction = _userAction;
            Exception = ex;
        }

        /// <summary>
        /// Gets the exception thrown by the <see cref="IMessageHub"/>
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the subscription token of the subscriber to which 
        /// message was published by the <see cref="IMessageHub"/>
        /// </summary>
        public object Token { get; }
        public string UserAction { get; }
    }
}
