using Client.Service;
using GrpcAzTest.protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static List<Notification> _messages = new List<Notification>();

        static void RefreshNotifications ()
        {
            Console.Clear();

            foreach (Notification message in _messages)
                Console.WriteLine($"[{message.Sender}] {message.Message}.");
        }

        static void Main()
        {
            const string serverAddress = "https://wa-grpctest-skylight001-dev.azurewebsites.net";

            NotificationsDerived notifications = new NotificationsDerived(serverAddress);

            notifications.Subscribe(notifications =>
            {
                _messages.Add(notifications);
                return null;
            });

            do
            {
                RefreshNotifications();
                Task.Delay(1000).GetAwaiter().GetResult();
            } while (true);
        }
    }
}
