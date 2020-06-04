#r "Newtonsoft.Json"
using System;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;

public static void Run(TimerInfo myTimer, ILogger log)
{
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

    var value = Environment.GetEnvironmentVariable("your_key_here");

}

public class ClearConversationStateService
{
    readonly string _cosmosUri;
    readonly string _cosmosKey;
    readonly string _databaseId;
    readonly string _collectionId;
    readonly int _timeoutSeconds;
    readonly string _botId;

    readonly ConversationState _conversationState;
    readonly BotFrameworkAdapter _adapter;

    public ClearConversationStateService()
    {
        _cosmosUri = Environment.GetEnvironmentVariable("CosmosUri");
        _cosmosKey = Environment.GetEnvironmentVariable("CosmosKey");
        _databaseId = Environment.GetEnvironmentVariable("CosmosDb");
        _collectionId = Environment.GetEnvironmentVariable("ComosDbContainer");
        _timeoutSeconds = int.Parse(Environment.GetEnvironmentVariable("ConversationTimeoutSeconds"));

        _botId = Environment.GetEnvironmentVariable("MicrosoftAppId");
        var appPassword = Environment.GetEnvironmentVariable("MicrosoftAppPassword");

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
