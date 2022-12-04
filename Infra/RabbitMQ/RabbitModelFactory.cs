using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.RabbitMQ
{
    public class RabbitModelFactory : IDisposable
    {
        private readonly IConnection connection;
        private readonly RabbitMQSettings settings;
        public RabbitModelFactory(IConnectionFactory connectionFactory, RabbitMQSettings settings)
        {
            this.connection = connectionFactory.CreateConnection();
            this.settings = settings;
        }

        public IModel CreateChannel()
        {
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: settings.ExchangeName, type: settings.ExchangeType);
            return channel;
        }

        public void Dispose() => this.connection.Dispose();
        
    }
}
