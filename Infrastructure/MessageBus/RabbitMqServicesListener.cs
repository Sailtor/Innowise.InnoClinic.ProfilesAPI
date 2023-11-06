using Domain.Entities;
using Domain.Repositories;
using Infrastructure.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.Abstractions;
using System.Text;
using System.Threading.Channels;

namespace MessageBus
{
    public class RabbitMqServicesListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName;
        private readonly string _hostName;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqServicesListener(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _queueName = configuration.GetRequiredSection("RabbitMQ:QueueName").Value;
            _hostName = configuration.GetRequiredSection("RabbitMQ:HostName").Value;

            var factory = new ConnectionFactory { HostName = _hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArgs) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();
                await RabbitMQServicesMessageHandler.HandleMessage(repositoryManager, model, eventArgs, stoppingToken);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}