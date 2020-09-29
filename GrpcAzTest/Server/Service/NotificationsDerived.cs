using Grpc.Core;
using GrpcAzTest.protos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Server.Service
{
    public class NotificationsDerived : NotificationsGrpc.NotificationsGrpcBase
    {
        private readonly ILogger<NotificationsDerived> Logger;
        private static BufferBlock<Notification> Notifications = new BufferBlock<Notification>();

        public NotificationsDerived(ILogger<NotificationsDerived> logger) => Logger = logger;

        public static async Task Send(Notification notification) => Notifications.Post(notification);

        public override Task<Empty> Send(NotificationRequest request, ServerCallContext context)
        {
            Notifications.Post(request.Body);
            return Task.FromResult(new Empty());
        }

        public override async Task Subscribe(Empty request, IServerStreamWriter<Notification> responseStream, ServerCallContext context)
        {
            while (true)
            {
                try
                {
                    if (Notifications.TryReceive(out Notification newNotification))
                        await responseStream.WriteAsync(newNotification);

                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
        }
    }
}
