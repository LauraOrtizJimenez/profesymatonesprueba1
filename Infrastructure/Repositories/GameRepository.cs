using Microsoft.EntityFrameworkCore;
using Proyecto1.Infrastructure.Data;
using Proyecto1.Infrastructure.Repositories.Interfaces;
using Proyecto1.Models;

namespace Proyecto1.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Board)
                .ThenInclude(b => b.Snakes)
                .Include(g => g.Board)
                .ThenInclude(b => b.Ladders)
                .Include(g => g.Players)
                .ThenInclude(p => p.User)
                .Include(g => g.Moves)
                .Include(g => g.Room)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Game> UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<List<Game>> GetGamesByUserIdAsync(int userId)
        {
            return await _context.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.User)
                .Where(g => g.Players.Any(p => p.UserId == userId))
                .OrderByDescending(g => g.StartedAt)
                .ToListAsync();
        }

        public async Task<Game?> GetByRoomIdAsync(int roomId)
        {
            return await _context.Games
                .Include(g => g.Board)
                .ThenInclude(b => b.Snakes)
                .Include(g => g.Board)
                .ThenInclude(b => b.Ladders)
                .Include(g => g.Players)
                .ThenInclude(p => p.User)
                .Include(g => g.Moves)
                .Include(g => g.Room)
                .OrderByDescending(g => g.StartedAt)
                .FirstOrDefaultAsync(g => g.RoomId == roomId);
        }
    }
}
