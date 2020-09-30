using System;
using System.Collections.Generic;
using System.Linq;
using Api.Service;
using GrpcAzTest.protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Notification> Get()
        {
            return NotificationsDerived.Notifications;
        }
    }
}
