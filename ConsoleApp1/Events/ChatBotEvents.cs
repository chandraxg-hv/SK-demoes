using System;

namespace ConsoleApp1.Events;

public static class ChatBotEvents
{
    public static string StartProcess => "StartProcess";
    public static string UserInputReceived => "UserInputReceived";
    public static string ResponseGenerated => "ResponseGenerated";
    public static string Exit => "Exit";
    public static string ProcessCompleted => "ProcessCompleted";
    public static string ProcessError => "ProcessError";
    public static string ProcessStarted => "ProcessStarted";
    public static string ProcessStopped => "ProcessStopped";
    public static string ProcessResumed => "ProcessResumed";
    public static string ProcessPaused => "ProcessPaused";
    public static string ProcessTerminated => "ProcessTerminated";
    public static string ProcessCancelled => "ProcessCancelled";
    public static string ProcessRestarted => "ProcessRestarted";
    public static string ProcessStateChanged => "ProcessStateChanged";

    public static string ProcessArchiveData => "ProcessArchiveData";
    public static string ProcessArchiveDataComplete => "ProcessArchiveDataComplete";
}
