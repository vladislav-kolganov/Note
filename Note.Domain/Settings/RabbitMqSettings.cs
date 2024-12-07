using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Settings
{
    public class RabbitMqSettings
    {
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
    }
}
