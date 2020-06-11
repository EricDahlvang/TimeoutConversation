using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;

namespace TimeoutConversationBot
{
    public static class CosmosDbStorageInitializer
    {
        public static IStorage GetStorage(CosmosDbPartitionedStorageOptions options, int timeToLiveInSeconds = -1)
        {
            using (var client = new CosmosClient(
                options.CosmosDbEndpoint,
                options.AuthKey,
                options.CosmosClientOptions ?? new CosmosClientOptions()))
            {
                var containerResponse =  (client
                    .CreateDatabaseIfNotExistsAsync(options.DatabaseId).GetAwaiter().GetResult())
                    .Database
                    .DefineContainer(options.ContainerId, "/id")
                    .WithDefaultTimeToLive(timeToLiveInSeconds)
                    .WithIndexingPolicy().WithAutomaticIndexing(false).Attach()
                    .CreateIfNotExistsAsync(options.ContainerThroughput)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return new CosmosDbPartitionedStorage(options);
        }
    }
}
