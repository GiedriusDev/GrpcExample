using GrpcAzTest.protos;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http;
using System;
using Grpc.Core;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Api.Service
{
    public static class NotificationsDerived
    {
        private static NotificationsGrpc.NotificationsGrpcClient Client;
        public static int DelayInMiliseconds;
        public static List<Notification> Notifications { get; set; } = new List<Notification>();

        public static void Initialize(string address, int delayInMiliseconds = 1000)
        {
            GrpcWebHandler handler = new GrpcWebHandler(new HttpClientHandler());
            GrpcChannel channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });
            Client = new NotificationsGrpc.NotificationsGrpcClient(channel);
            DelayInMiliseconds = delayInMiliseconds;

            Subscribe(notification =>
            {
                Notifications.Add(notification);
                return null;
            });
        }

        private static void Subscribe(Func<Notification, object> callback)
        {
            Task.Run(async () =>
            {
                try
                {
                    AsyncServerStreamingCall<Notification> stream = Client.Subscribe(new Empty());

                    while (await stream.ResponseStream.MoveNext().ConfigureAwait(false))
                    {
                        if (stream.ResponseStream.Current != null)
                            callback.Invoke(stream.ResponseStream.Current);

                        await Task.Delay(DelayInMiliseconds);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }
    }
}
