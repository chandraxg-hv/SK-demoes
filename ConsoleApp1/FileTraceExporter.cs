using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ConsoleApp1
{
    //Create a otel File exporter
    public class FileTraceExporter<Activity> : BaseExporter<Activity> where Activity : class
    {
        private readonly string _filePath;

        public FileTraceExporter(string filePath)
        {
            _filePath = filePath;
        }

        public override ExportResult Export(in Batch<Activity> batch)
        {
            using (var writer = new StreamWriter(_filePath, append: true))
            {
                foreach (var activity in batch)
                {
                    writer.WriteLine(activity.ToString());
                }
            }
            return ExportResult.Success;
        }
    }
}
