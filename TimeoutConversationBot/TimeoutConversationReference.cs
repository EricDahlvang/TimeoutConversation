using System;
using Microsoft.Bot.Schema;

namespace MultiTurnPromptBot
{
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
}
