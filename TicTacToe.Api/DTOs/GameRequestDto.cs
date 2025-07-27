namespace TicTacToe.Api.DTOs
{
    public class GameRequestDto
    {
        public int BoardSize { get; set; }
        public int WinCondition { get; set; }
    }
}
