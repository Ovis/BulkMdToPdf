using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Markdig;
using Markdig.SyntaxHighlighting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BulkMdToPdf.Domain
{
    class MdToPdf
    {
        private static Setting Setting = null;

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void ConvertMarkdownToPdf(Setting setting,string path)
        {
            Setting = setting;

            try
            {
                /* ソースフォルダ存在確認 */
                if (!Directory.Exists(path))
                {
                    Logger.Error("Error : Directory does not exist.");
                    Console.Write("Error : Directory does not exist.");
                    return;
                }
                
                /* 出力フォルダ作成 */
                var outputDirectoryPath = Path.Combine(path, "PDF");

                if (!Directory.Exists(outputDirectoryPath))
                {
                    Directory.CreateDirectory(outputDirectoryPath);
                }

                Console.WriteLine("MarkdownFileFolderPath:" + path);
                Console.WriteLine("OutputPath:" + outputDirectoryPath);
                Console.WriteLine("OutputHtml:" + Setting.EnableOutputHtml.ToString());
                Console.WriteLine("CSSPath:" + Setting.CssPath);

                /* Markdownファイルパスを取得 */
                var files = GetMarkdownFile(path);

                if (files.FirstOrDefault() == null)
                {
                    Logger.Error("Error : Markdown file does not exist.");
                    Console.Write("Error : Markdown file does not exist.");
                    return;
                }

                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);

                    var htmlString = MakeHtmlString(filePath);

                    if (Setting.EnableOutputHtml)
                    {
                        /* 確認用にHTML出力 */
                        File.WriteAllText(Path.Combine(outputDirectoryPath, fileName + ".html"), htmlString);
                    }

                    var pdfBytes = MakePdfBytes(htmlString);

                    var pdfFile = Path.Combine(outputDirectoryPath, fileName + ".pdf");
                    File.WriteAllBytes(pdfFile, pdfBytes);
                }
            }
            catch(Exception e)
            {
                Logger.Log(LogLevel.Error, e, e.Message);
            }
        }

        /// <summary>
        /// Markdownファイルパス取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetMarkdownFile(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.md");

            return files;
        }

        /// <summary>
        /// MarkdownファイルをHTMLファイルに一度変換する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string MakeHtmlString(string path)
        {
            var html = new StringBuilder();


            html.AppendLine("<html>").AppendLine("<head>").Append("<title>").Append(Path.GetFileNameWithoutExtension(path));
            html.Append("</title>");

            if (Setting.EnableOutputHtml)
            {
                if (Setting.CssPath != null && File.Exists(Setting.CssPath))
                {
                    html.AppendLine("<style>");

                    using (FileStream fs = new FileStream(Setting.CssPath, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            html.AppendLine(sr.ReadToEnd());
                        }
                    }
                    html.AppendLine("</style>");
                }
            }

            html.AppendLine("</head>").AppendLine("<body>");

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .UseGridTables()
                .UseListExtras()
                .UseMediaLinks()
                .UseSyntaxHighlighting()
                .Build();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    html.AppendLine(Markdown.ToHtml(sr.ReadToEnd(), pipeline));
                }
            }

            if (Setting.EnableOutputHtml)
            {
                html.AppendLine("</body>").AppendLine("</html>");
            }

            return html.ToString();
        }

        /// <summary>
        /// HTMLデータをもとにPDF（バイトデータ）を出力
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static byte[] MakePdfBytes(string html)
        {
            byte[] bytes = null;

            using (var output = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 40, 40, 120, 120))
                {
                    using (var writer = PdfWriter.GetInstance(document, output))
                    {
                        document.Open();

                        var css = "";

                        if (Setting.CssPath != null && File.Exists(Setting.CssPath))
                        {
                            using (FileStream fs = new FileStream(Setting.CssPath, FileMode.Open))
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    css = sr.ReadToEnd();
                                }
                            }
                        }

                        using (var msCss = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                        {
                            using (var msHtml = new MemoryStream(Encoding.UTF8.GetBytes(html)))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, msHtml, msCss);
                            }
                        }
                        document.Close();
                    }
                }
                bytes = output.ToArray();
            }

            return bytes;
        }
    }
}
