using AgentFramework.Common;
using AgentFramework.OllamaAgent;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434/";
var model = "llama3.2:3b";

var tools = GeneralPlugin.GetTools();
var name = "TimeAgent";
var description = "Agent that knows about the current date and time.";
var instructions = "You should only reply on questions related to the current date and time and never any other questions.";

var ollamaClient = new OllamaApiClient(new Uri(endpoint), model);
var agentClient = new ChatClientAgent(ollamaClient, name: name, description: description, instructions: instructions, tools: tools);

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