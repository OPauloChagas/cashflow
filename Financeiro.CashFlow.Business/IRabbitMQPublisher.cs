namespace Financeiro.CashFlow.Business
{
    public interface IRabbitMQPublisher
    {
        void PublishMessage(string queueName, string message);
    }
}
