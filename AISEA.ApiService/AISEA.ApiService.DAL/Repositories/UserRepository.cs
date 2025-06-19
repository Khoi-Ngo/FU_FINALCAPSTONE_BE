using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AiseaContext context) : base(context)
        {
        }
        //get user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User> GetUserByEmailOrUsernameAsync(string email, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == username);
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Status == EUserStatus.ACTIVE)
                .Include(u => u.Role)
                .ToListAsync();
        }
        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users.Include(u => u.Role);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetActiveUsersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.Status == EUserStatus.ACTIVE)
                .Include(u => u.Role);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

    }
}