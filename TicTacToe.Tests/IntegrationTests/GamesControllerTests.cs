using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TicTacToe.Api;
using Xunit;

namespace TicTacToe.Tests.IntegrationTests
{
    public class GamesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GamesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateGame_Should_Return_Created_Game()
        {

            var client = _factory.CreateClient();
            var request = new
            {
                boardSize = 3,
                winCondition = 3
            };

            var response = await client.PostAsJsonAsync("/api/games", request);

            response.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Health_Endpoint_Should_Return_OK()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/health");

            response.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Invalid_Json_Should_Return_BadRequest()
        {
            var client = _factory.CreateClient();
            var invalidJson = "{ invalid json }";
            var content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/games", content);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
