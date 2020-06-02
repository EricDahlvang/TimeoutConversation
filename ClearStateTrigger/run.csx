using System;
using Microsoft.Bot.Schema;

public static void Run(TimerInfo myTimer, ILogger log)
{
    
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
}

public class TimeoutConversationReference
{
    public TimeoutConversationReference(ConversationReference conversationReference)
    {
        this.ConversationReference = conversationReference;
        this.LastAccessed = DateTime.UtcNow;
    }

    public DateTime LastAccessed { get; set; }

    public ConversationReference ConversationReference { get; set; }
}
