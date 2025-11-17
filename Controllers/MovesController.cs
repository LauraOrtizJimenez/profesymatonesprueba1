using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto1.DTOs.Moves;
using Proyecto1.Services.Interfaces;
using System.Security.Claims;

namespace Proyecto1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public MovesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("roll")]
        public async Task<ActionResult<MoveResultDto>> RollDice([FromBody] RollDiceRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                var result = await _gameService.RollDiceAndMoveAsync(request.GameId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("surrender")]
        public async Task<ActionResult> Surrender([FromBody] SurrenderRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                await _gameService.SurrenderAsync(request.GameId, userId);
                return Ok(new { message = "Successfully surrendered" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("get-profesor")]
        public async Task<ActionResult<ProfesorQuestionDto>> GetProfesorQuestion([FromBody] MoveRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var question = await _gameService.GetProfesorQuestionAsync(request.GameId, userId);
            return Ok(question);
        }

        [HttpPost("answer-profesor")]
        public async Task<ActionResult<MoveResultDto>> AnswerProfesor([FromBody] ProfesorAnswerRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _gameService.AnswerProfesorQuestionAsync(request.GameId, userId, request.Answer);
            return Ok(result);
        }
    }
}
