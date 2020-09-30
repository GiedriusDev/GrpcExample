using Grpc.Core;
using GrpcAzTest.protos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GrpcService.Service
{
    public class NotificationsDerived : NotificationsGrpc.NotificationsGrpcBase
    {
        private static ConcurrentDictionary<Guid, BufferBlock<Notification>> _notifications = new ConcurrentDictionary<Guid, BufferBlock<Notification>>();

        public override Task<Empty> Send(NotificationRequest request, ServerCallContext context)
        {
            foreach (KeyValuePair<Guid, BufferBlock<Notification>> connection in _notifications)
                connection.Value.Post(request.Body);
            return Task.FromResult(new Empty());
        }

        public override async Task Subscribe(Empty request, IServerStreamWriter<Notification> responseStream, ServerCallContext context)
        {
            Guid guid = Guid.NewGuid();
            _notifications.TryAdd(guid, new BufferBlock<Notification>());

            while (true)
            {
                try
                {
                    if (_notifications.TryGetValue(guid, out BufferBlock<Notification> notificationBlock))
                        if (notificationBlock.TryReceive(out Notification newNotification))
                            await responseStream.WriteAsync(newNotification);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
        }
    }
}
