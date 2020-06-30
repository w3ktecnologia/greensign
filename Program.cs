using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Greensign
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RegistryWriter.SetUrlProtocol();

            if (args.Length == 1)
            {
                Config config = new Config(args[0]);

                string filePath = Download(config.DownloadURL).GetAwaiter().GetResult();

                if (filePath.Length > 0)
                {
                    var cert = CertStore.LoadCertificate();

                    if (cert != null)
                    {
                        var chain = Signature.GetChain(cert);
                        var signature = Signature.GetFromX509Certificate(cert);

                        byte[] signedPDF = PDF.Sign(signature, chain, filePath, cert.FriendlyName, cert.Subject, config.SourceName, config.DocumentLink, config.DocumentName);

                        Upload(config.UploadURL, signedPDF).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Cancel(config.CancelURL).GetAwaiter().GetResult();
                    }

                    File.Delete(filePath);
                }
            }
        }

        static async Task<string> Download(string url)
        {
            var response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                string tempFile = Path.GetTempFileName();

                using (var fs = new FileStream(tempFile,
                    FileMode.OpenOrCreate))
                {
                    await response.Content.CopyToAsync(fs);
                }

                return tempFile;
            }

            return "";
        }

        static async Task Upload(string url, byte[] file)
        {
            using (var content =
             new MultipartFormDataContent("Upload----" + DateTime.Now.ToString()))
            {
                content.Add(new StreamContent(new MemoryStream(file)), "signedfile", "signed.pdf");

                using (
                   var message =
                       await client.PostAsync(url, content))
                {
                    var input = await message.Content.ReadAsStringAsync();
                    Console.WriteLine(input);
                }
            }
        }

        static async Task Cancel(string url)
        {
            using (var content = new StringContent(""))
            {
                using (
                   var message =
                       await client.PostAsync(url, content))
                {
                    var input = await message.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
