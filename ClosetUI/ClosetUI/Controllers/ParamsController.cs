using ClosetUI.Models.Models;
using ClosetUI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace ClosetUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParamsController : ControllerBase
    {
        private readonly IPartCalculationService _partsService;
        private readonly HttpClient _httpClient;

        public ParamsController(IPartCalculationService PartsService, HttpClient httpClient)
        {
            _partsService = PartsService;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetParams()
        {
            try
            {
                var paramsModel = await _partsService.GetParams();
                return Ok(paramsModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("GeneratePdf")]
        public async Task<IActionResult> GeneratePdf([FromBody] ParamsModel paramsModel)
        {
            try
            {
                //var pdfBytes = await _partsService.GenerateAndDownloadPdf(paramsModel);

                using HttpRequestMessage request = new(HttpMethod.Post, _httpClient.BaseAddress);

                var json = JsonConvert.SerializeObject(paramsModel);

                using StringContent stringContent = new(json, Encoding.UTF8, "application/json");

                request.Content = stringContent;

                using HttpResponseMessage response = await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead
                    );

                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadFromJsonAsync<StreamDto>();
                    
                    return File(result.Content, result.ContentType, "PartsLayout.pdf");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "filed to generate file");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
