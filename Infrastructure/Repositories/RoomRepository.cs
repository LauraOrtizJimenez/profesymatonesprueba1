using Microsoft.EntityFrameworkCore;
using Proyecto1.Infrastructure.Data;
using Proyecto1.Infrastructure.Repositories.Interfaces;
using Proyecto1.Models;
using Proyecto1.Models.Enums;

namespace Proyecto1.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<Room?> GetByIdWithPlayersAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Players)
                .ThenInclude(p => p.User)
                .Include(r => r.Game)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Room>> GetAvailableRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Players)
                .ThenInclude(p => p.User)
                .Include(r => r.Game)
                .Where(r => r.Status == RoomStatus.Open || r.Status == RoomStatus.Full)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Room> CreateAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }
    }
}