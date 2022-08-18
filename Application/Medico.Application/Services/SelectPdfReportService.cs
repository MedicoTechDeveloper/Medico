using System;
using System.IO;
using System.Net;
using System.Text;
using Medico.Application.Interfaces;
using Newtonsoft.Json;
using SelectPdf;

namespace Medico.Application.Services
{
    public class SelectPdfReportService : IReportService
    {
        public static string apiEndpoint = "https://selectpdf.com/api2/convert/";
        public static string apiKey = "01abdf36-3aa3-4732-9839-51fb9dc158af";
        // public static string testUrl = "https://selectpdf.com";

        // POST JSON example using WebClient (and Newtonsoft for JSON serialization)
        public byte[] SelectPdfPostWithWebClient(string htmlStringContent)
        {
            System.Console.WriteLine("Starting conversion with WebClient ...");

            // set parameters
            SelectPdfParameters parameters = new SelectPdfParameters
            {
                key = apiKey,
                html = htmlStringContent,
                base_url= "https://medicophysicians.azurewebsites.net/"
            };

            // JSON serialize parameters
            string jsonData = JsonConvert.SerializeObject(parameters);
            byte[] byteData = Encoding.UTF8.GetBytes(jsonData);

            // create WebClient object
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            // POST parameters (if response code is not 200 OK, a WebException is raised)
            try
            {
                byte[] result = webClient.UploadData(apiEndpoint, "POST", byteData);

                return result;
                //// all ok - read PDF and write on disk (binary read!!!!)
                //MemoryStream ms = new MemoryStream(result);

                //// write to file
                //FileStream file = new FileStream("test2.pdf", FileMode.Create, FileAccess.Write);
                //ms.WriteTo(file);
                //file.Close();
            }
            catch (WebException webEx)
            {
                // an error occurred
                System.Console.WriteLine("Error: " + webEx.Message);

                HttpWebResponse response = (HttpWebResponse)webEx.Response;
                Stream responseStream = response.GetResponseStream();

                // get details of the error message if available (text read!!!)
                StreamReader readStream = new StreamReader(responseStream);
                string message = readStream.ReadToEnd();
                responseStream.Close();
                throw webEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateFromHtmlString(string htmlStringContent)
        {
            var converter = new HtmlToPdf();

            converter.Options.MarginLeft = 15;
            converter.Options.MarginRight = 15;
            converter.Options.MarginBottom = 15;
            converter.Options.MarginTop = 15;

            if (string.IsNullOrEmpty(htmlStringContent))
                throw new InvalidOperationException("Report content is empty");

            var doc = converter.ConvertHtmlString(htmlStringContent);
            var pdfContent = doc.Save();
            doc.Close();

            return pdfContent;
        }
    }

    // API parameters - add the rest here if needed
    public class SelectPdfParameters
    {
        public string key { get; set; }
        public string url { get; set; }
        public string html { get; set; }
        public string base_url { get; set; }
    }
}