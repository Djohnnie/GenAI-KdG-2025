using AgentFramework.Common;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using SemanticKernelDemo.AzureOpenAiAgent;
using System.ClientModel;

var endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? string.Empty;
var key = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? string.Empty;

var tools = GeneralPlugin.GetTools();
var name = "TimeAgent";
var description = "Agent that knows about the current date and time.";
var instructions = "You should only reply on questions related to the current date and time and never any other questions.";

var openAIClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key));
var chatClient = openAIClient.GetChatClient("gpt-5-chat").AsIChatClient();
var agentClient = new ChatClientAgent(chatClient, name: name, description: description, instructions: instructions, tools: tools);

var chatThread = agentClient.GetNewThread();
var chatHistory = new List<ChatMessage>();

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

    await foreach (var response in agentClient.RunStreamingAsync(chatHistory, chatThread))
    {
        fullMessage += response.Text;
        Console.Write(response.Text);
    }

    Console.WriteLine();

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, fullMessage));
    chatThread.Debug();
}