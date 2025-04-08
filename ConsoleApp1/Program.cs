// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;
using OpenTelemetry.Exporter;
using ConsoleApp1.Steps;

using ConsoleApp1;
using ConsoleApp1.Processes;

var builder = Kernel.CreateBuilder();

//Services
/*         var config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build(); */

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

#pragma warning disable CS8604 // Possible null reference argument.

/*         Kernel kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
            deploymentName: configuration["AzureOpenAI:deploymentName"],
            endpoint: configuration["AzureOpenAI:endpoint"],
            apiKey: configuration["AzureOpenAI:key"]
        ).Build(); */
// Get configuration values


//Logging Factory
// Declare LoggerFactory outside the conditional block
ILoggerFactory? loggerFactory = null;

//ask a question if you want the OpenTelemetry to be enabled
Console.WriteLine("Do you want to enable OpenTelemetry? (y/n)");
string? answerSensitive = Console.ReadLine();
if (answerSensitive == "y")
{
    var resourceBuilder = ResourceBuilder
       .CreateDefault()
       .AddService("TelemetryConsoleQuickstart");

    // Enable model diagnostics with sensitive data.
    AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", false);

    using var traceProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddSource("Microsoft.SemanticKernel*")
        .AddConsoleExporter()
        .Build();

    using var meterProvider = Sdk.CreateMeterProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddMeter("Microsoft.SemanticKernel*")
        .AddConsoleExporter()
        .Build();

    loggerFactory = LoggerFactory.Create(builder =>
    {
        // Add OpenTelemetry as a logging provider
        builder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.AddConsoleExporter();
            // Format log messages. This is default to false.
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
        });
        builder.SetMinimumLevel(LogLevel.Information);
    });

    builder.Services.AddSingleton(loggerFactory);
}

builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: configuration["AzureOpenAI:deploymentName"],
    endpoint: configuration["AzureOpenAI:endpoint"],
    apiKey: configuration["AzureOpenAI:key"]
);

//Plugins
builder.Plugins.AddFromType<ConsoleApp1.NewsPlugin>();
builder.Plugins.AddFromType<ConsoleApp1.ArchivePlugin>();
builder.Plugins.AddFromType<ConsoleApp1.WeatherPlugin>();
builder.Plugins.AddFromType<ConsoleApp1.ExchangeRatePlugin>();


//builder.Services.AddSingleton<BlogGeneration>();
builder.Services.AddTransient<IntroStep>();
builder.Services.AddKernel()
    .Plugins.AddFromType<IntroStep>("PrintIntroMessage");

builder.Services.AddTransient<UserInputStep>();
builder.Services.AddKernel()
    .Plugins.AddFromType<UserInputStep>("GetUserInputAsync");

builder.Services.AddTransient<ChatBotResponseStep>();
builder.Services.AddKernel()
    .Plugins.AddFromType<ChatBotResponseStep>("GetChatResponseAsync");

builder.Services.AddTransient<ArchiveProcessStep>();
builder.Services.AddKernel()
    .Plugins.AddFromType<ArchiveProcessStep>("ArchiveDataAsync");

builder.Services.AddTransient<ExitStep>();
builder.Services.AddKernel()
    .Plugins.AddFromType<ExitStep>("HandleExit");

// builder.Services.AddKernel()
//     .Plugins.AddFromType<BlazorSKApp.Processes.BlogGeneration>("BlogGeneration");

//builder.Services.AddSingleton<IServiceProvider>(sp => sp);

builder.Services.AddSingleton<ProcessFlow>(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();
    return new ProcessFlow(kernel);
});

var kernel = builder.Build();

//create the process framework

ProcessFlow processFlow = new ProcessFlow(kernel);

//Start the workflow
await processFlow.StartWorkflowAsync("Blog Generation");

Console.WriteLine ("press any key to continue...");
Console.ReadKey();


//Create a chat completion service

/* var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
//Create a chat history
ChatHistory chatHistory = new ChatHistory();

while (true)
{
    //Get user input
    Console.WriteLine("*****************************************************************");
    Console.Write("Prompt   : ");
    string? userInput = Console.ReadLine();
    if (userInput == null)
    {
        Console.WriteLine("Input cannot be null. Please try again.");
        continue;
    }

    //Add user message to chat history
    chatHistory.AddUserMessage(userInput);

    var completion = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatHistory,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel: kernel
        );

    string fullmessage = "";

    await foreach (var message in completion)
    {
        fullmessage += message.Content;
        Console.Write(message.Content);
    }

    chatHistory.AddAssistantMessage(fullmessage);
    Console.WriteLine();
    //Console.WriteLine("****************************************************************");

} */

