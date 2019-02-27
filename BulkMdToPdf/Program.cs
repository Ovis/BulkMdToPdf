using System;
using System.Linq;
using BulkMdToPdf.Domain;
using NLog;

namespace BulkMdToPdf
{
    class Program
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var setting = new Setting();

            if (args.FirstOrDefault() == null)
            {
                Logger.Error("Error: DocumentPath is invalid.");
                Console.WriteLine("Error: DocumentPath is invalid.");
                return;
            }

            MdToPdf.ConvertMarkdownToPdf(setting, args.FirstOrDefault());
        }
    }
}
