using Medico.Application.Interfaces;

namespace Medico.Application.Services
{
    public class DevextremeReportService : IReportService
    {
        public byte[] GenerateFromHtmlString(string htmlStringContent)
        {
            // the implementation uses DevExpress.XtraRichEdit that was not approved by Glenn
            // consider to use when license will be purchased
            return new[] {(byte) 1};
            /*using (var richEditDocumentServer = new RichEditDocumentServer {HtmlText = htmlStringContent})
            {
                var document = richEditDocumentServer.Document;
                const int marginValue = 80;
                
                foreach (var section in document.Sections)
                {
                    section.Margins.Bottom = marginValue;
                    section.Margins.Right = marginValue;
                    section.Margins.Left = marginValue;
                    section.Margins.Top = marginValue;
                }
                
                using (var ms = new MemoryStream())
                {
                    richEditDocumentServer.ExportToPdf(ms);
                    return ms.ToArray();
                }
            }*/
        }

        public byte[] SelectPdfPostWithWebClient(string htmlStringContent)
        {
            throw new System.NotImplementedException();
        }
    }
}