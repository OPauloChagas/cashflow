using RabbitMQ.Client;
using System.Text;

namespace Financeiro.CashFlow.Business
{
    public class RabbitMQPublisher
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQPublisher(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void PublishMessage(string queueName, string message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }

}
