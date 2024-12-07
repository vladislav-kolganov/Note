using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Consumer.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddConsumer(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMqListener>();
        }
    }
}
