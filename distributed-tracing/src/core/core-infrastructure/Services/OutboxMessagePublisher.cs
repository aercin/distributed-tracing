using core_application.Abstractions;
using core_application.Models;
using core_domain.Entities;
using Dapper;
using System.Diagnostics;
using System.Text.Json;

namespace core_infrastructure.Services
{
    public class OutboxMessagePublisher : IOutboxMessagePublisher
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IEnumerable<IDbConnectionFactory> _dbConnectionFactories;
        private readonly ICacheProvider _cacheProvider;
        private readonly ICustomTracing _customTracing;

        public OutboxMessagePublisher(IEventDispatcher eventDispatcher,
                                      IEnumerable<IDbConnectionFactory> dbConnectionFactories,
                                      ICacheProvider cacheProvider,
                                      ICustomTracing customTracing)
        {
            this._eventDispatcher = eventDispatcher;
            this._dbConnectionFactories = dbConnectionFactories;
            this._cacheProvider = cacheProvider;
            this._customTracing = customTracing;
        }

        public async Task PublishOutboxMessages(string dbContext, string toBeSentTopic)
        {
            var hasExistedLock = await this._cacheProvider.GetAsync<bool>("IsOutboxMessagePublisherThreadSafe");

            if (!hasExistedLock)
            {
                await this._cacheProvider.SetAsync("IsOutboxMessagePublisherThreadSafe", true, cacheSettings =>
                {
                    cacheSettings.AbsoluteExpiration = 5;//sn
                });

                var dbConnectionFactory = this._dbConnectionFactories.Single(x => x.Context == dbContext);

                using (var connection = dbConnectionFactory.GetOpenConnection())
                {
                    string sql = $@"SELECT
                                          ""Id"",
                                          ""Type"",
                                          ""Message"",
                                          ""CreatedOn"",
                                          ""TraceContext""
                                        FROM public.""OutboxMessages"" 
                                ";

                    var messages = await connection.QueryAsync<OutboxMessage>(sql);

                    foreach (var relatedOutBoxMessage in messages)
                    {
                        try
                        {
                            var traceHeaders = JsonSerializer.Deserialize<List<MessageHeader>>(relatedOutBoxMessage.TraceContext);

                            await this._customTracing.StartActivity($"send message to {toBeSentTopic}", ActivityKind.Producer, traceHeaders, async () =>
                            {
                                await this._eventDispatcher.DispatchEvent(toBeSentTopic, relatedOutBoxMessage.Message);

                                await connection.ExecuteAsync(@"DELETE FROM public.""OutboxMessages"" WHERE ""Id""=@Id", new { Id = relatedOutBoxMessage.Id });
                            });
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
