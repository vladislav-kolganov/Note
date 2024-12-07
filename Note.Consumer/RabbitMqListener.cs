using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Note.Domain.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace Note.Consumer
{

    public class RabbitMqListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IOptions<RabbitMqSettings> _options;
        public RabbitMqListener(IOptions<RabbitMqSettings> options)
        {
            _options = options;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_options.Value.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (obj, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                Debug.WriteLine($"Получено сообщение: {content}");
                _channel.BasicAck(args.DeliveryTag, false);
            };
            _channel.BasicConsume(_options.Value.QueueName, false, consumer);

        //    Dispose();

            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            base.Dispose();
        }
    }
}
