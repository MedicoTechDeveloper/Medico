namespace Medico.Application.Interfaces
{
    public interface IReportService
    {
        byte[] SelectPdfPostWithWebClient(string htmlStringContent);
        byte[] GenerateFromHtmlString(string htmlStringContent);
    }
}
