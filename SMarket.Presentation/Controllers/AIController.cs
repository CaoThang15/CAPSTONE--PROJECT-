using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.AI;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("suggest-description")]
        public async Task<ActionResult<Response>> SuggestDescription(ProductSuggestionInput input)
        {
            if (input == null)
                return BadRequest(new Response
                {
                    Message = "Invalid product data.",
                    Data = null
                });

            try
            {
                string description = await _aiService.GenerateProductDescriptionAsync(input);
                return Ok(new Response
                {
                    Message = "Generate Product Description successfully.",
                    Data = description
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "AI suggestion failed",
                    error = ex.Message
                });
            }
        }
    }
}
