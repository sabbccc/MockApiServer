using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Data.Entities;
using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MockApiServerDbContext _context;

        public UserRepository(MockApiServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserViewModel>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                    Name = u.Name,
                    MobileNo = u.MobileNo,
                    LastLoginTime = u.LastLoginTime,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy,
                    IsActive = u.IsActive,
                    Remarks = u.Remarks
                }).ToListAsync();
        }

        public async Task<UserViewModel?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                    Name = u.Name,
                    MobileNo = u.MobileNo,
                    LastLoginTime = u.LastLoginTime,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy,
                    IsActive = u.IsActive,
                    Remarks = u.Remarks
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(UserViewModel model)
        {
            var entity = new User
            {
                Username = model.Username ?? string.Empty,
                Password = model.Password ?? string.Empty,
                Name = model.Name ?? string.Empty,
                MobileNo = model.MobileNo ?? string.Empty,
                LastLoginTime = model.LastLoginTime,
                CreatedAt = DateTime.Now,
                CreatedBy = "system",
                IsActive = model.IsActive ?? true,
                Remarks = model.Remarks
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserViewModel model)
        {
            var entity = await _context.Users.FindAsync(model.Id);
            if (entity == null) return;

            entity.Username = model.Username ?? entity.Username;
            entity.Password = model.Password ?? entity.Password;
            entity.Name = model.Name ?? entity.Name;
            entity.MobileNo = model.MobileNo ?? entity.MobileNo;
            entity.LastLoginTime = model.LastLoginTime;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = "system";
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.Remarks = model.Remarks ?? entity.Remarks;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
