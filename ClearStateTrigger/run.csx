#r "Newtonsoft.Json"
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector.Authentication;

public static async Task Run(TimerInfo myTimer, ILogger log)
{
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    
    var cosmosUri = Environment.GetEnvironmentVariable("CosmosUri");
    var cosmosKey = Environment.GetEnvironmentVariable("CosmosKey");
    var databaseId = Environment.GetEnvironmentVariable("CosmosDb");
    var stateCollection = Environment.GetEnvironmentVariable("ComosDbStateContainer");
    var timeoutCollection = Environment.GetEnvironmentVariable("ComosDbTimeoutContainer");
    var timeoutSeconds = int.Parse(Environment.GetEnvironmentVariable("ConversationTimeoutSeconds"));
    var botId = Environment.GetEnvironmentVariable("MicrosoftAppId");
    var appPassword = Environment.GetEnvironmentVariable("MicrosoftAppPassword");

    var clearConversationStateService = new ClearConversationStateService(cosmosUri, cosmosKey, databaseId, stateCollection, timeoutCollection, timeoutSeconds, botId, appPassword);
    await clearConversationStateService.ClearExpiredConversations();

    log.LogInformation($"C# Timer trigger function completed at: {DateTime.Now}");
}

public class ClearConversationStateService
{
    readonly string _cosmosUri;
    readonly string _cosmosKey;
    readonly string _databaseId;
    readonly string _stateCollection;
    readonly string _timeoutCollection;
    readonly int _timeoutSeconds;
    readonly string _botId;

    readonly ConversationState _conversationState;
    readonly BotFrameworkAdapter _adapter;

    public ClearConversationStateService(string cosmosUri, string cosmosKey, string databaseId, string stateCollection, string timeoutCollection, int timeoutSeconds, string botId, string appPassword)
    {
        _cosmosUri = cosmosUri;
        _cosmosKey = cosmosKey;
        _databaseId = databaseId;
        _stateCollection = stateCollection;
        _timeoutCollection = timeoutCollection;
        _timeoutSeconds = timeoutSeconds;
        _botId = botId;

        var options = new CosmosDbPartitionedStorageOptions()
            {
                AuthKey = _cosmosKey,
                ContainerId = _stateCollection,
                CosmosDbEndpoint = _cosmosUri,
                DatabaseId = _databaseId,
                CompatibilityMode = false,
            };

        _conversationState = new ConversationState(new CosmosDbPartitionedStorage(options));
        _adapter = new BotFrameworkAdapter(new SimpleCredentialProvider(_botId, appPassword));
    }

        public async Task ClearExpiredConversations()
        {
            using (var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey))
            {
                var docs = documentClient.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(_databaseId, _timeoutCollection),
                    new SqlQuerySpec(
                        "SELECT * FROM TimeoutContainer r WHERE r.LastAccessed < @lastAccessedTimeout",
                        new SqlParameterCollection(new[] { new SqlParameter 
                                                            { 
                                                                Name = "@lastAccessedTimeout", 
                                                                Value = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeoutSeconds))
                                                            }
                                                          })));

                var dialogStateProperty = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
                foreach (var doc in docs)
                {
                    var dynamicDoc = (dynamic)doc;
                    var timeoutConveration = (TimeoutConversationReference)dynamicDoc;
                    await (_adapter as BotAdapter).ContinueConversationAsync(_botId, timeoutConveration.ConversationReference, async (turnContext, cancellationToken) => 
                    {
                        await turnContext.SendActivityAsync("Hello.  Are you still there?  Please provide the requested information.");

                        // NOTE: Uncommend below to clear the conversation state

                        // await dialogStateProperty.DeleteAsync(turnContext, cancellationToken);
                        // await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
                    }, CancellationToken.None);

                    await documentClient.DeleteDocumentAsync(dynamicDoc._self);
                }
            }
        }
}

public class TimeoutConversationReference
{
    public TimeoutConversationReference(ConversationReference conversationReference)
    {
        this.ConversationReference = conversationReference;
        this.Id = conversationReference.Conversation.Id;
        this.LastAccessed = DateTime.UtcNow;
    }

    [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
    public virtual string Id { get; set; }
    
    public DateTime LastAccessed { get; set; }

    public ConversationReference ConversationReference { get; set; }
}
