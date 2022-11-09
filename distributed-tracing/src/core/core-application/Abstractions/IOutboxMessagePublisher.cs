namespace core_application.Abstractions
{
    public interface IOutboxMessagePublisher
    {
        public Task PublishOutboxMessages(string dbContext, string toBeSentTopic);
    }
}
