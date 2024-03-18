using ClosetUI.Models.Models;
using ClosetUI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClosetUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParamsController : ControllerBase
    {
        private readonly IPartCalculationService _partsService;

        public ParamsController(IPartCalculationService PartsService)
        {
            _partsService = PartsService;
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
    }
}
