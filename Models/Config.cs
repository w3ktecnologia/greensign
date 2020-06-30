using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greensign
{
    class Config
    {
        public string UploadURL { get; set; }
        public string DownloadURL { get; set; }
        public string CancelURL { get; set; }
        public string SourceName { get; set; }
        public string DocumentLink { get; set; }
        public string DocumentName { get; set; }

        public Config(string parameters)
        {
            parameters = parameters.Replace("greensign://", "");

            string[] values = parameters.Split(';');
            string baseUrl = "", upload = "", download = "";

            foreach (var item in values)
            {
                if (item.StartsWith("baseurl="))
                    baseUrl = item.Replace("baseurl=", "");
                if (item.StartsWith("upload="))
                    upload = item.Replace("upload=", "");
                if (item.StartsWith("download="))
                    download = item.Replace("download=", "");
                if (item.StartsWith("source="))
                    SourceName = item.Replace("source=", "");
                if (item.StartsWith("link="))
                    DocumentLink = item.Replace("link=", "");
                if (item.StartsWith("name="))
                    DocumentName = item.Replace("name=", "");
            }

            UploadURL = baseUrl + upload;
            DownloadURL = baseUrl + download;
            CancelURL = baseUrl + upload + "&cancel=true";
        }
    }
}
