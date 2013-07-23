using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace Jasen.Framework.WpfProviderPlugins.Common
{
    public static class MessengerExtension
    {
        public static void Send<T>(this IMessenger messenger, T body, object token)
        {
            Messenger.Default.Send<GenericMessage<T>>(new GenericMessage<T>(body), token);
        }

        public static void Register<T>(this Messenger messenger, object recipient, object token, Action<T> action)
        {
            Messenger.Default.Register<GenericMessage<T>>(recipient, token, msg =>
            {
                action(msg.Content);
            });
        }
    }

}
