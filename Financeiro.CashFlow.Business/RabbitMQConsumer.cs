using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Financeiro.CashFlow.Business
{
    public class RabbitMQConsumer
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQConsumer(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void StartConsuming(string queueName, Action<string> onMessageReceived)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                onMessageReceived(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }

}
