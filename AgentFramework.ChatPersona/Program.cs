using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

var endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? string.Empty;
var key = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? string.Empty;

var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key));
var chatClient = client.GetChatClient("gpt-5-chat");
var agentClient = chatClient.CreateAIAgent(name: "Chat", description: "Just a chat");

var chatHistory = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You should answer as a 10-year old child.")
};

int age = 10;

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("User > ");
    Console.ForegroundColor = ConsoleColor.White;
    var request = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, request!));

    string fullMessage = "";
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Assistant > ");

    await foreach (var response in agentClient.RunStreamingAsync(chatHistory))
    {
        fullMessage += response.Text;
        Console.Write(response.Text);
    }

    Console.WriteLine();

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, fullMessage));

    age -= 2;
    chatHistory.Add(new ChatMessage(ChatRole.System, $"You should answer as a {age}-year old child and tell us your age after every response."));
}