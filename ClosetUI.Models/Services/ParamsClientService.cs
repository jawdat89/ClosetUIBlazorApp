using ClosetUI.Models.Dtos;
using ClosetUI.Models.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace ClosetUI.Services
{
    public class ParamsClientService : IPartCalculationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ParamsClientService> _logger;

        public ParamsClientService(HttpClient httpClient, ILogger<ParamsClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
                        return default(byte[]);
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

        public async Task<ParamsModel> GetParams()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Params");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(ParamsModel);
                    }

                    var paramsModel = await response.Content.ReadFromJsonAsync<ParamsModel>();
                    return paramsModel;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status code: {response.StatusCode} message: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Params Client] - GetParams failed: {ex}");
                throw;
            }
        }

        public Task<ParamsModel> ProcessAsync(PartGeneratorDto parameters)
        {
            throw new NotImplementedException();
        }
    }
}
