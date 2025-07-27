using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.DTOs;
using TicTacToe.Api.Interfaces;

namespace TicTacToe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameRequestDto request)
        {
            if (request.BoardSize < 3)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid board size",
                    Detail = "Board size must be at least 3"
                });

            if (request.WinCondition < 3 || request.WinCondition > request.BoardSize)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid win condition",
                    Detail = "Win condition must be between 3 and board size"
                });

            var game = await _gameService.CreateGameAsync(request.BoardSize, request.WinCondition);

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameService.GetGameByIdAsync(id);

            if (game == null)
                return NotFound();

            return Ok(game);
        }
    }
}
