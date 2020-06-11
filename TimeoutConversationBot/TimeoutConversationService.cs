using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using MultiTurnPromptBot;
using System;
using System.Threading.Tasks;

namespace TimeoutConversationBot
{
    public class TimeoutConversationService
    {
        readonly string _cosmosUri;
        readonly string _cosmosKey;
        readonly string _databaseId;
        readonly string _collectionId;

        private static bool EnsuredCreated = false;
        private static object LockObject = new object();

        public TimeoutConversationService(IConfiguration config)
        {
            _cosmosUri = config["CosmosUri"];
            _cosmosKey = config["CosmosKey"];
            _databaseId = config["CosmosDb"];
            _collectionId = config["ComosDbTimeoutContainer"];

            Initialize();
        }

        private void Initialize()
        {
            if (!EnsuredCreated)
            {
                lock (LockObject)
                {
                    if (!EnsuredCreated)
                    {
                        using (var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey))
                        {
                            documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseId }).GetAwaiter().GetResult();
                            documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseId), new DocumentCollection { Id = _collectionId }).GetAwaiter().GetResult();
                        }
                        EnsuredCreated = true;
                    }
                }
            }
        }

        public async Task AddOrUpdateConversationReference(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            var timeoutReference = new TimeoutConversationReference(activity.GetConversationReference());

            using (var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey))
            {
                await documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), timeoutReference);
            }
        }
    }
}
