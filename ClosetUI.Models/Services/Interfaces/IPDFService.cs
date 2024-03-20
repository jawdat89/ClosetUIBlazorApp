using ClosetUI.Models.Models;

namespace ClosetUI.Models.Services
{
    public interface IPDFService
    {
        Task<byte[]> GenerateAndDownloadPdf(ParamsModel paramsModel);
    }
}
