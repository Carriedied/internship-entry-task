using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicTacToe.Api.Data;
using TicTacToe.Api.Enums;
using TicTacToe.Api.Services;

namespace TicTacToe.Tests.UnitTests
{
    public class GameServiceTests
    {
        [Fact]
        public async Task CreateGameAsync_Should_Create_Game_With_Correct_Parameters()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateGameTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);

            Assert.NotNull(game);
            Assert.Equal(3, game.BoardSize);
            Assert.Equal(3, game.WinCondition);
            Assert.Equal("X", game.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, game.Status);
            Assert.Equal(0, game.MoveCount);
            Assert.NotNull(game.Board);
            Assert.Equal(3, game.Board.Length);

            for (int i = 0; i < 3; i++)
            {
                Assert.NotNull(game.Board[i]);
                Assert.Equal(3, game.Board[i].Length);
                Assert.All(game.Board[i], cell => Assert.Null(cell));
            }
        }

        [Fact]
        public async Task GetGameByIdAsync_Should_Return_Correct_Game()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GetGameTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var createdGame = await service.CreateGameAsync(3, 3);
            var gameId = createdGame.Id;
            var retrievedGame = await service.GetGameByIdAsync(gameId);

            Assert.NotNull(retrievedGame);
            Assert.Equal(gameId, retrievedGame.Id);
            Assert.Equal(3, retrievedGame.BoardSize);
        }

        [Fact]
        public async Task GetGameByIdAsync_Should_Return_Null_For_NonExistent_Game()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GetNonExistentGameTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);

            var game = await service.GetGameByIdAsync(Guid.NewGuid());
            Assert.Null(game);
        }

        [Fact]
        public async Task MakeMoveAsync_Should_Throw_Exception_For_NonExistent_Game()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MoveNonExistentGameTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);

            await Assert.ThrowsAnyAsync<Exception>(() =>
                service.MakeMoveAsync(Guid.NewGuid(), 0, 0));
        }

        [Fact]
        public async Task MakeMoveAsync_Should_Throw_Exception_For_Occupied_Cell()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MoveOccupiedCellTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);

            await service.MakeMoveAsync(game.Id, 0, 0);

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                service.MakeMoveAsync(game.Id, 0, 0));

            Assert.Contains("Cell is already occupied", exception.Message);
        }

        [Fact]
        public async Task MakeMoveAsync_Should_Switch_Players()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SwitchPlayersTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);

            var result1 = await service.MakeMoveAsync(game.Id, 0, 0);
            Assert.Equal("X", result1.Board[0][0]);
            Assert.Equal("O", result1.CurrentPlayer);

            var result2 = await service.MakeMoveAsync(game.Id, 0, 1);
            Assert.Equal("O", result2.Board[0][1]);
            Assert.Equal("X", result2.CurrentPlayer);
        }

        [Fact]
        public async Task MakeMoveAsync_Should_Detect_Win_Horizontal()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "WinHorizontalTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);

            await service.MakeMoveAsync(game.Id, 0, 0);
            await service.MakeMoveAsync(game.Id, 1, 0);
            await service.MakeMoveAsync(game.Id, 0, 1);
            await service.MakeMoveAsync(game.Id, 1, 1);

            var result = await service.MakeMoveAsync(game.Id, 0, 2);

            Assert.Equal(GameStatus.XWon, result.Status);
            Assert.Equal("X", result.Board[0][0]);
            Assert.Equal("X", result.Board[0][1]);
            Assert.Equal("X", result.Board[0][2]);
        }

        [Fact]
        public async Task MakeMoveAsync_Should_Detect_Draw()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DrawTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);
            var gameId = game.Id;

            var move1 = await service.MakeMoveAsync(gameId, 0, 0);
            Assert.Equal(GameStatus.InProgress, move1.Status);

            var move2 = await service.MakeMoveAsync(gameId, 0, 1);
            Assert.Equal(GameStatus.InProgress, move2.Status);

            var move3 = await service.MakeMoveAsync(gameId, 0, 2);
            Assert.Equal(GameStatus.InProgress, move3.Status);

            var move4 = await service.MakeMoveAsync(gameId, 1, 1);
            Assert.Equal(GameStatus.InProgress, move4.Status);

            var move5 = await service.MakeMoveAsync(gameId, 1, 0);
            Assert.Equal(GameStatus.InProgress, move5.Status);

            var move6 = await service.MakeMoveAsync(gameId, 1, 2);
            Assert.Equal(GameStatus.InProgress, move6.Status);

            var move7 = await service.MakeMoveAsync(gameId, 2, 1);
            Assert.Equal(GameStatus.InProgress, move7.Status);

            var move8 = await service.MakeMoveAsync(gameId, 2, 0);
            Assert.Equal(GameStatus.InProgress, move8.Status);

            var result = await service.MakeMoveAsync(gameId, 2, 2);

            Assert.Equal(GameStatus.Draw, result.Status);
            Assert.All(result.Board, row => Assert.All(row, cell => Assert.NotNull(cell)));
        }


        [Fact]
        public async Task MakeMoveAsync_ThirdMove_Should_Sometimes_Place_Opponent_Symbol()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ThirdMoveSpecialRuleTest")
                .Options;

            using var context = new AppDbContext(options);

            var service = new GameService(context);
            var game = await service.CreateGameAsync(3, 3);

            var result1 = await service.MakeMoveAsync(game.Id, 0, 0);
            Assert.Equal("X", result1.Board[0][0]);

            var result2 = await service.MakeMoveAsync(game.Id, 1, 0);
            Assert.Equal("O", result2.Board[1][0]);

            var result3 = await service.MakeMoveAsync(game.Id, 0, 1);

            var symbols = new[] { result1.Board[0][0], result2.Board[1][0], result3.Board[0][1] };

            Assert.Contains("X", symbols);
            Assert.Contains("O", symbols);
            Assert.NotNull(result3.Board[0][1]);
        }
    }
}
