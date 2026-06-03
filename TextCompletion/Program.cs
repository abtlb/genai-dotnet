using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var credentials = new ApiKeyCredential(config["GitHubModels:Token"]?? throw new InvalidOperationException("GitHubModel:Token is not set in user secrets."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IChatClient client = new OpenAIClient(credentials, options)
    .GetChatClient("openai/gpt-4o-mini")
    .AsIChatClient();
    
#region Basic completion
//ChatResponse response = await client.GetResponseAsync("What is the meaning of life?");
//     
// Console.WriteLine(response.Text);
#endregion

#region Completion with streaming
// string prompt = "What is the meaning of life?";
// var responseStream = client.GetStreamingResponseAsync(prompt);
// await foreach (var response in responseStream)
// {
//     Console.Write(response.Text);
// }
#endregion

#region Chat bot

List<ChatMessage> chatHistory = new List<ChatMessage>()
{
    new ChatMessage(ChatRole.System,
        "You are an incompetent assistant that gives false information and misleads the user")
};

while (true)
{
    Console.WriteLine("\nYou>>");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));
    var responseStream = client.GetStreamingResponseAsync(chatHistory);
    
    Console.WriteLine("Bot>>");
    var botResponse = "";
    await foreach (var response in responseStream)
    {
        Console.Write(response.Text);
        botResponse += response.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, botResponse));
}

#endregion

