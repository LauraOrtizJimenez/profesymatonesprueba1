using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Proyecto1.Services.Interfaces;

namespace Proyecto1.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IGameService gameService, ILogger<GameHub> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"User {userId} connected to GameHub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"User {userId} disconnected from GameHub");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGameGroup(int gameId)
        {
            var userId = GetUserId();
            var groupName = $"Game_{gameId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"User {userId} joined game group {groupName}");

            // Notificar a otros jugadores
            await Clients.OthersInGroup(groupName).SendAsync("PlayerJoined", userId);

            // Enviar estado actual del juego al jugador que se conecta
            try
            {
                var gameState = await _gameService.GetGameStateAsync(gameId);
                await Clients.Caller.SendAsync("GameStateUpdate", gameState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending game state to user {userId}");
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task LeaveGameGroup(int gameId)
        {
            var userId = GetUserId();
            var groupName = $"Game_{gameId}";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"User {userId} left game group {groupName}");

            await Clients.OthersInGroup(groupName).SendAsync("PlayerLeft", userId);
        }

        public async Task SendMove(int gameId)
        {
            var userId = GetUserId();
            var groupName = $"Game_{gameId}";

            try
            {
                var moveResult = await _gameService.RollDiceAndMoveAsync(gameId, int.Parse(userId));

                // Si requiere responder al profesor, enviar solo al jugador
                if (moveResult.RequiresProfesorAnswer && moveResult.ProfesorQuestion != null)
                {
                    await Clients.Caller.SendAsync("ReceiveProfesorQuestion", moveResult.ProfesorQuestion);
                }

                // Enviar resultado del movimiento a todos en el grupo
                await Clients.Group(groupName).SendAsync("MoveCompleted", new
                {
                    UserId = userId,
                    MoveResult = moveResult
                });

                // Enviar estado actualizado del juego
                var gameState = await _gameService.GetGameStateAsync(gameId);
                await Clients.Group(groupName).SendAsync("GameStateUpdate", gameState);

                // Si hay ganador, notificar
                if (moveResult.IsWinner)
                {
                    await Clients.Group(groupName).SendAsync("GameFinished", new
                    {
                        WinnerId = userId,
                        Message = moveResult.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing move for user {userId} in game {gameId}");
                await Clients.Caller.SendAsync("MoveError", ex.Message);
            }
        }

        public async Task SendSurrender(int gameId)
        {
            var userId = GetUserId();
            var groupName = $"Game_{gameId}";

            try
            {
                await _gameService.SurrenderAsync(gameId, int.Parse(userId));

                // Notificar rendición
                await Clients.Group(groupName).SendAsync("PlayerSurrendered", userId);

                // Enviar estado actualizado del juego
                var gameState = await _gameService.GetGameStateAsync(gameId);
                await Clients.Group(groupName).SendAsync("GameStateUpdate", gameState);

                // Verificar si el juego terminó
                if (gameState.Status == "Finished")
                {
                    await Clients.Group(groupName).SendAsync("GameFinished", new
                    {
                        WinnerId = gameState.WinnerPlayerId,
                        WinnerName = gameState.WinnerName,
                        Reason = "Other players surrendered"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing surrender for user {userId} in game {gameId}");
                await Clients.Caller.SendAsync("SurrenderError", ex.Message);
            }
        }

        public async Task RequestGameState(int gameId)
        {
            try
            {
                var gameState = await _gameService.GetGameStateAsync(gameId);
                await Clients.Caller.SendAsync("GameStateUpdate", gameState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting game state for game {gameId}");
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        private string GetUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new HubException("User not authenticated");
        }
    }
}

 