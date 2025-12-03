using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFramework.Common;

public static class ChatHistoryExtensions
{
    public static void Debug(this AgentThread chatThread)
    {
        if (chatThread is ChatClientAgentThread agentThread && agentThread.MessageStore is InMemoryChatMessageStore chatMessages)
        {
            foreach (var chat in chatMessages)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{chat.Role}] ");

                if (chat.Role == ChatRole.Tool && chat.Contents[0] is FunctionResultContent functionResult)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{functionResult.CallId} --> {functionResult.Result.ToString()}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{chat.Text.Trim()}");
                }
            }
        }
    }
}