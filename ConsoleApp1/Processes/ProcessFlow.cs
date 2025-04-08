using System;
using Microsoft.SemanticKernel;
using ConsoleApp1.Steps;
using ConsoleApp1.Events;
using OpenTelemetry.Trace;


namespace ConsoleApp1.Processes;

public class ProcessFlow
{
#pragma warning disable SKEXP0080 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    private readonly Kernel _kernel;

    public ProcessFlow( Kernel kernel)
    {
        _kernel = kernel;
        Console.WriteLine("BlogGeneration constructor");
        Console.WriteLine($"_kernel: {_kernel}");

    }
    [KernelFunction]
    public async Task StartWorkflowAsync(string blogtitle)
    {
        try
        {
            ProcessBuilder processBuilder = new("AdvancedChatBot");

            var introStep = processBuilder.AddStepFromType<IntroStep>();
            var userInputStep = processBuilder.AddStepFromType<UserInputStep>();
            var responseStep = processBuilder.AddStepFromType<ChatBotResponseStep>();
            var archiveStep = processBuilder.AddStepFromType<ArchiveProcessStep>();
            var exitStep = processBuilder.AddStepFromType<ExitStep>();

            // Start with the intro step
            processBuilder.OnInputEvent(ChatBotEvents.StartProcess)
                .SendEventTo(new ProcessFunctionTargetBuilder(introStep));


            // After intro, proceed to user input
            introStep.OnFunctionResult(nameof(IntroStep.PrintIntroMessage))
                .SendEventTo(new ProcessFunctionTargetBuilder(userInputStep));

            // When user input is received, process it
            userInputStep.OnEvent(ChatBotEvents.UserInputReceived)
                .SendEventTo(new ProcessFunctionTargetBuilder(responseStep, parameterName: "userMessage"));

            // After bot response, loop back to user input
            responseStep.OnEvent(ChatBotEvents.ResponseGenerated)
                .SendEventTo(new ProcessFunctionTargetBuilder(userInputStep));

/*             //Handle Archive event
            userInputStep.OnEvent(ChatBotEvents.ResponsePublish)
                .SendEventTo(new ProcessFunctionTargetBuilder(archiveStep, parameterName: "userMessage" )); */

            responseStep.OnEvent(ChatBotEvents.ProcessArchiveData)
                .SendEventTo(new ProcessFunctionTargetBuilder(archiveStep, functionName: "ArchiveData", parameterName: "llmresponse" )); 


            // After archive action, loop back to user input
            archiveStep.OnEvent(ChatBotEvents.ProcessArchiveDataComplete)
                .SendEventTo(new ProcessFunctionTargetBuilder(userInputStep));

            // Handle exit event
            userInputStep.OnEvent(ChatBotEvents.Exit)
                .SendEventTo(new ProcessFunctionTargetBuilder(exitStep));

            // Stop process after exit step
            exitStep.OnFunctionResult(nameof(ExitStep.HandleExit))
                .StopProcess();


            KernelProcess kernelProcess = processBuilder.Build();

            await kernelProcess.StartAsync(_kernel, new KernelProcessEvent { Id = ChatBotEvents.StartProcess });
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
#pragma warning restore SKEXP0080 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
