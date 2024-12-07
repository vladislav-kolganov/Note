using Microsoft.Extensions.DependencyInjection;
using Note.Producer.Interfaces;

namespace Note.Producer.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddProducer(this IServiceCollection services)
        {
            services.AddScoped<IMessageProducer, Producer>();
        }
    }
}
