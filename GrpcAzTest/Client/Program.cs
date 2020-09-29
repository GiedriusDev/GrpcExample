using Client.Service;
using GrpcAzTest.protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static List<Notification> Messages = new List<Notification>();

        static void RefreshNotifications ()
        {
            Console.Clear();

            foreach (Notification message in Messages)
                Console.WriteLine($"[{message.Sender}] {message.Message}.");
        }

        static void Main()
        {
            const string ServerAddress = "https://localhost:44355";

            NotificationsDerived notifications = new NotificationsDerived(ServerAddress);

            notifications.Subscribe(notifications =>
            {
                Messages.Add(notifications);
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
