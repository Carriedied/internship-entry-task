using TicTacToe.Api.Models;

namespace TicTacToe.Api.Interfaces
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(int boardSize, int winCondition);
        Task<Game?> GetGameByIdAsync(Guid id);
        Task<Game> MakeMoveAsync(Guid gameId, int row, int col);
    }
}
