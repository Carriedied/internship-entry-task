using TicTacToe.Api.Enums;

namespace TicTacToe.Api.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public int BoardSize { get; set; }
        public int WinCondition { get; set; }
        public string[][] Board { get; set; }
        public string CurrentPlayer { get; set; }
        public GameStatus Status { get; set; }
        public int MoveCount { get; set; }
    }
}
