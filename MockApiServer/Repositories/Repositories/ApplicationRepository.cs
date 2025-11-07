using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Data.Entities;
using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly MockApiServerDbContext _context;

        public ApplicationRepository(MockApiServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationViewModel>> GetAllAsync()
        {
            return await _context.Applications
                .Select(a => new ApplicationViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive
                }).ToListAsync();
        }

        public async Task<ApplicationViewModel?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Where(a => a.Id == id)
                .Select(a => new ApplicationViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(ApplicationViewModel model)
        {
            var entity = new Application
            {
                Name = model.Name ?? "",
                IsActive = (bool)model.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "system"
            };
            _context.Applications.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationViewModel model)
        {
            var entity = await _context.Applications.FindAsync(model.Id);
            if (entity == null) return;

            entity.Name = model.Name ?? "";
            entity.IsActive = (bool)model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Applications.FindAsync(id);
            if (entity != null)
            {
                _context.Applications.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
