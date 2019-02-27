using System;
using System.Collections.Specialized;
using System.Configuration;

namespace BulkMdToPdf.Domain
{
    class Setting
    {
        private NameValueCollection Configuration = ConfigurationManager.AppSettings;

        public Setting()
        {
        }

        //CSSファイルパス
        public string CssPath => Configuration["CssPath"];

        //HTMLファイル出力
        public bool EnableOutputHtml => Boolean.Parse(Configuration["EnableOutputHTML"]);
    }
}
