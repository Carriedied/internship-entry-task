using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TicTacToe.Api.Data;
using TicTacToe.Api.Enums;
using TicTacToe.Api.Interfaces;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(int boardSize, int winCondition)
        {
            var board = new string[boardSize][];

            for (int i = 0; i < boardSize; i++)
            {
                board[i] = new string[boardSize];
            }

            var game = new Game
            {
                Id = Guid.NewGuid(),
                BoardSize = boardSize,
                WinCondition = winCondition,
                Board = board,
                CurrentPlayer = "X",
                Status = GameStatus.InProgress,
                MoveCount = 0
            };

            _context.Games.Add(game);

            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Game?> GetGameByIdAsync(Guid id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game> MakeMoveAsync(Guid gameId, int row, int col)
        {
            var game = await ProcessMove(gameId, row, col);

            await _context.SaveChangesAsync();

            return game;
        }

        private async Task<Game> ProcessMove(Guid gameId, int row, int col)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                throw new Exception("Game not found.");
            }
            if (game.Status != GameStatus.InProgress)
            {
                throw new Exception("Game is already finished.");
            }
            if (row < 0 || row >= game.BoardSize || col < 0 || col >= game.BoardSize)
            {
                throw new Exception("Move coordinates are out of bounds.");
            }
            if (game.Board[row][col] != null)
            {
                throw new Exception("Cell is already occupied");
            }

            string player = game.CurrentPlayer;

            if (game.MoveCount > 0 && (game.MoveCount + 1) % 3 == 0 && new Random().NextDouble() < 0.1)
            {
                player = player == "X" ? "O" : "X";
                
            }

            game.Board[row][col] = player;
            game.MoveCount++;

            if (CheckWin(game, row, col, player))
            {
                game.Status = player == "X" ? GameStatus.XWon : GameStatus.OWon;
            }
            else if (game.MoveCount == game.BoardSize * game.BoardSize)
            {
                game.Status = GameStatus.Draw;
            }
            else
            {
                game.CurrentPlayer = player == "X" ? "O" : "X";
            }

            return game;
        }

        private bool CheckWin(Game game, int row, int col, string player)
        {
            int winCondition = game.WinCondition;
            int boardSize = game.BoardSize;
            var board = game.Board;

            int count = 1;

            for (int c = col - 1; c >= 0 && board[row][c] == player; c--) count++;

            for (int c = col + 1; c < boardSize && board[row][c] == player; c++) count++;

            if (count >= winCondition) return true;

            count = 1;

            for (int r = row - 1; r >= 0 && board[r][col] == player; r--) count++;

            for (int r = row + 1; r < boardSize && board[r][col] == player; r++) count++;

            if (count >= winCondition) return true;

            count = 1;

            for (int i = 1; row - i >= 0 && col - i >= 0 && board[row - i][col - i] == player; i++) count++;

            for (int i = 1; row + i < boardSize && col + i < boardSize && board[row + i][col + i] == player; i++) count++;

            if (count >= winCondition) return true;

            count = 1;

            for (int i = 1; row + i < boardSize && col - i >= 0 && board[row + i][col - i] == player; i++) count++;

            for (int i = 1; row - i >= 0 && col + i < boardSize && board[row - i][col + i] == player; i++) count++;

            if (count >= winCondition) return true;

            return false;
        }
    }
}
