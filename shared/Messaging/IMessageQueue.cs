using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Messaging
{
    public interface IMessageQueue
    {
        void Publish<T>(string queueName, T message) where T : class;
        void Subscribe<T>(string queueName, Action<T> onMessageReceived) where T : class;
    }
}
