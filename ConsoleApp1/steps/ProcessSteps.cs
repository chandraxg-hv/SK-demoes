using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ConsoleApp1.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace ConsoleApp1.Steps;

#pragma warning disable SKEXP0080 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class IntroStep : KernelProcessStep
{
    [KernelFunction]
    public void PrintIntroMessage()
    {
        Console.WriteLine("Welcome to the Advanced Semantic Kernel Chatbot!\nType 'exit' at any time to quit.\n");
    }
}

public class UserInputStep : KernelProcessStep<SharedState>
{
    //replaced all UserInputState with SharedState
    private SharedState? _state;

    public override ValueTask ActivateAsync(KernelProcessStepState<SharedState> state)
    {
        _state = state.State ?? new SharedState();
        return ValueTask.CompletedTask;
    }

    [KernelFunction("GetUserInput")]
    public async Task GetUserInputAsync(KernelProcessStepContext context)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            input = "Hello"; // Default input
        }

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            await context.EmitEventAsync(new() { Id = ChatBotEvents.Exit });
            return;
        }

/*         if (input.Equals("Archive", StringComparison.OrdinalIgnoreCase))
        {
            _state!.UserInputs.Add(input);
            //await context.EmitEventAsync(new() { Id = ChatBotEvents.ResponsePublish, Data = _state });
            await context.EmitEventAsync(new() {Id = ChatBotEvents.ResponsePublish, Data = this._state.ChatMessages});
            return;
        } */

        _state!.UserInputs.Add(input);
        await context.EmitEventAsync(new() { Id = ChatBotEvents.UserInputReceived, Data = input });
    }
}

public class ChatBotResponseStep : KernelProcessStep<SharedState>
{
    //Replaced all ChatBotState with SharedState
    private SharedState? _state;

    public override ValueTask ActivateAsync(KernelProcessStepState<SharedState> state)
    {
        _state = state.State ?? new SharedState();
        _state.ChatMessages ??= new();
        return ValueTask.CompletedTask;
    }

    [KernelFunction("GetChatResponse")]
    public async Task GetChatResponseAsync(KernelProcessStepContext context, string userMessage, Kernel _kernel)
    {
        string errorMessage = string.Empty;
        ChatMessageContent? response = null;
        ChatMessageContent? lastMessage = null;
        _state!.ChatMessages.Add(new(AuthorRole.User, userMessage));

        if (userMessage.Equals("Archive", StringComparison.OrdinalIgnoreCase))
        {
            //await context.EmitEventAsync(new() { Id = ChatBotEvents.ResponsePublish, Data = _state });

            lastMessage = _state?.ChatMessages.LastOrDefault(msg => msg.Role == AuthorRole.Assistant);

            if (lastMessage != null)
            {
                await context.EmitEventAsync(new() { Id = ChatBotEvents.ProcessArchiveData, Data = lastMessage.Content });
                return;
            }
            else
            {
                response = new ChatMessageContent { Content = "No previous assistant message found. Nothing to archive." };
            }

        } 
        else 
        {
            IChatCompletionService chatService = _kernel.Services.GetRequiredService<IChatCompletionService>();
            response = await chatService.GetChatMessageContentAsync(this._state.ChatMessages);
        }
        
        if (response != null)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _state.ChatMessages.Add(new(AuthorRole.Assistant, response.Content));
            Console.WriteLine($"Bot: {response.Content}\n");
        }

        // Emit event to continue the conversation
        await context.EmitEventAsync(new() { Id = ChatBotEvents.ResponseGenerated, Data = response.Content });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}

public class ArchiveProcessStep : KernelProcessStep<SharedState>
    // ArchiveProcessStep now inherits from KernelProcessStep<llmresponse, SharedState> to handle the response type
{
    //replace all ChatBotState with SharedState
    private SharedState? _state;

    public override ValueTask ActivateAsync(KernelProcessStepState<SharedState> state)
    {
        _state = state.State ?? new SharedState();
        _state.ChatMessages ??= new(); 
        return ValueTask.CompletedTask;
    }

    [KernelFunction("ArchiveData")]
    //write a async task method to archive data into a file
    public async Task ArchiveDataAsync(KernelProcessStepContext context, string  llmresponse, Kernel _kernel, string filename = "blogpost.html")
    {

        Console.WriteLine ("I am into the Archive Data...");
  
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filename));
        }

        try
        {
            using (StreamWriter writer = new StreamWriter($"/Users/cganapathy/Demoes/skpf-demo/ConsoleApp1/{filename}", append: true))
            {
                //var lastMessage = _state?.ChatMessages != null ? _state.ChatMessages.LastOrDefault() : null;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Console.WriteLine (llmresponse);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                if (llmresponse != null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    await writer.WriteLineAsync(llmresponse);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            }
            await context.EmitEventAsync(new() { Id = ChatBotEvents.ProcessArchiveDataComplete });
            //_state!.ChatMessages.Add(new(AuthorRole.User, userMessage));
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            Console.WriteLine($"An error occurred while archiving data: {ex.Message}");
        }
    }
}
public class ExitStep : KernelProcessStep
{
    [KernelFunction]
    public void HandleExit()
    {
        Console.WriteLine("Thank you for using the chatbot. Goodbye!");
    }
}

/* public class UserInputState
{
    public List<string> UserInputs { get; set; } = new();
} */

/* public class ChatBotState
{
    public ChatHistory ChatMessages { get; set; } = new();
} */

public class SharedState
{
    public List<string> UserInputs { get; set; } = new();
    public ChatHistory ChatMessages { get; set; } = new();
}

public class LastChatMessage
{
    public string Content { get; set; }

    public LastChatMessage()
    {
        Content = string.Empty; // Default value for Content
    }

    public LastChatMessage(string content)
    {
        Content = content;
    }   

    public override string ToString()
    {
        return $" {Content}";
    }   
}
    #pragma warning restore SKEXP0080 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

