# Azure Function to Bot Conversation

Modified https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore/05.multi-turn-prompt

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 3.1)
- [Azure Account](https://azure.microsoft.com/en-us/free/)
- [CosmosDb](https://docs.microsoft.com/en-us/azure/cosmos-db/) (With two containers)
- [Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/) (Timer Trigger)

## Getting Started

- Create a CosmosDb database with two containers

- Create an Azure Function App, with a [Timer trigger function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer)  Add code from ClearStateTrigger

- Create an Azure [Web App Bot](https://docs.microsoft.com/en-us/azure/bot-service/abs-quickstart) Publish code from TimeoutConversationBot

## Sample Projects Explained

This sample contains two projects:

1) ClearStateTrigger
    
    This is code for a dotnet Azure Timer Trigger Function which will read from a CosmosDb collection, and resume conversations which have had no activity for a specified time period.


2) TimeoutConversationBot
    
    This is the bot code copied from 05.multi-turn-prompt and modified to use CosmosDb for state, and also save (or update) a ConversationReference with LastAccessed

## App Settings

The following App Settings should be configured in the Azure Function and the Web App Bot:

- CosmosUri: uri of your cosmosdb instance
- CosmosKey: key for accessing your cosmosdb instance
- CosmosDb: database name
- ComosDbStateContainer: container for Bot State
- ComosDbTimeoutContainer: container for Conversation References
- ConversationTimeoutSeconds: time period after which to clear state, or notify user
- MicrosoftAppId: bot microsoft app id
- MicrosoftAppPassword: bot microsoft app password