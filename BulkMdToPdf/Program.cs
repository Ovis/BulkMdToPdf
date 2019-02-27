using System;
using System.Linq;
using BulkMdToPdf.Domain;

namespace BulkMdToPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var setting = new Setting();

            if (args.FirstOrDefault() == null)
            {
                Console.WriteLine("Error: DocumentPath is invalid.");
            }

            MdToPdf.ConvertMarkdownToPdf(setting, args.FirstOrDefault());
        }
    }
}
