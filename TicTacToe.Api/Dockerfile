FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TicTacToe.Api/TicTacToe.Api.csproj", "TicTacToe.Api/"]
COPY ["TicTacToe.Tests/TicTacToe.Tests.csproj", "TicTacToe.Tests/"]
RUN dotnet restore "TicTacToe.Api/TicTacToe.Api.csproj"
RUN dotnet restore "TicTacToe.Tests/TicTacToe.Tests.csproj"
COPY . .
WORKDIR "/src/TicTacToe.Api"
RUN dotnet build "TicTacToe.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicTacToe.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.Api.dll"]