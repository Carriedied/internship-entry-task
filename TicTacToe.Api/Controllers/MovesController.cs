using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.DTOs;
using TicTacToe.Api.Interfaces;

namespace TicTacToe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public MovesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("{gameId}")]
        public async Task<IActionResult> MakeMove(Guid gameId, [FromBody] MoveRequestDto move)
        {
            try
            {
                var result = await _gameService.MakeMoveAsync(gameId, move.Row, move.Col);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid move",
                    Detail = ex.Message
                });
            }
        }
    }
}
