using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace BulkMdToPdf.Domain
{
    class Setting
    {
        private NameValueCollection Configuration = ConfigurationManager.AppSettings;

        public Setting()
        {
            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            WkHtmlToPdfPath = Path.Combine(fi.Directory?.FullName ?? throw new InvalidOperationException(), "Wkhtmltopdf");
        }

        //CSSファイルパス
        public string CssPath => Configuration["CssPath"];

        //HTMLファイル出力
        public bool EnableOutputHtml => bool.Parse(Configuration["EnableOutputHTML"]);

        //WkHtmlToPdfフォルダパス

        public string WkHtmlToPdfPath { get; set; }
    }
}
