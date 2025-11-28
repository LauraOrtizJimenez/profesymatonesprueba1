using Microsoft.EntityFrameworkCore; 
using Proyecto1.Infrastructure.Data;  
using Proyecto1.Models;

namespace Proyecto1.Infrastructure.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<Game?> GetByIdWithDetailsAsync(int id);
        Task<Game> CreateAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task<List<Game>> GetGamesByUserIdAsync(int userId);
        Task<Game?> GetByRoomIdAsync(int roomId);
    }
}