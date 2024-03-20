using ClosetUI.Models.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;

namespace ClosetUI.Models.Services
{
    public class PDFClientService : IPDFService
    {
        private readonly ILogger<PDFClientService> _logger;
        private readonly HttpClient _httpClient;

        public PDFClientService(ILogger<PDFClientService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<byte[]> GenerateAndDownloadPdf(ParamsModel paramsModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Params/GeneratePdf", paramsModel);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    // Directly return the byte array content if successful
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status code: {response.StatusCode} message: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Params Client] - GeneratePdfAsync failed: {ex}");
                throw;
            }

        }
    }
}
