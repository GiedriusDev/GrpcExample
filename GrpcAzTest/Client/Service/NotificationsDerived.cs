﻿using GrpcAzTest.protos;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http;
using System;
using Grpc.Core;
using System.Threading.Tasks;

namespace Client.Service
{
    public class NotificationsDerived
    {
        private NotificationsGrpc.NotificationsGrpcClient _client = null;
        public int DelayInMiliseconds;

        public NotificationsDerived(string address, int delayInMiliseconds = 1000)
        {
            GrpcWebHandler handler = new GrpcWebHandler(new HttpClientHandler());
            GrpcChannel channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });
            _client = new NotificationsGrpc.NotificationsGrpcClient(channel);
            DelayInMiliseconds = delayInMiliseconds;
        }

        public void Send(Notification notification)
        {
            _client.Send(new NotificationRequest() { Body = notification });
        }

        public void Subscribe(Func<Notification, object> callback)
        {
            Task.Run(async () =>
            {
                try
                {
                    AsyncServerStreamingCall<Notification> stream = _client.Subscribe(new Empty());

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
