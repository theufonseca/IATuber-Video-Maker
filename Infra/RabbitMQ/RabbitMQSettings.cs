using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.RabbitMQ
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public int port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
