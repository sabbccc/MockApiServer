using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Data.Entities;
using MockApiServer.Models.ViewModels;
using MockApiServer.Repositories;

namespace MockApiServer.Repositories
{
    public class MockRepository : IMockRepository
    {
        private readonly MockApiServerDbContext _dbContext;

        public MockRepository(MockApiServerDbContext db)
        {
            _dbContext = db;
        }

        public async Task<Mock?> GetMockWithScenariosAsync(string path, string method)
        {
            return await _dbContext.Mocks
                .Include(m => m.MockScenarios)
                .FirstOrDefaultAsync(m =>
                    m.Path == "/" + path &&
                    m.Method == method &&
                    m.IsActive == true);
        }

        public async Task<List<MockViewModel>> GetByApplicationIdAsync(int appId)
        {
            return await _dbContext.Mocks
                .Where(m => m.ApplicationId == appId)
                .Include(m => m.Application)
                .Select(m => new MockViewModel
                {
                    Id = m.Id,
                    ApplicationId = m.ApplicationId,
                    ApplicationName = m.Application.Name,
                    Name = m.Name,
                    Path = m.Path,
                    Method = m.Method,
                    IsActive = m.IsActive
                }).ToListAsync();
        }

        public async Task<List<MockViewModel>> GetAllAsync()
        {
            return await _dbContext.Mocks
                .Include(m => m.Application)
                .Select(m => new MockViewModel
                {
                    Id = m.Id,
                    ApplicationId = m.ApplicationId,
                    ApplicationName = m.Application.Name,
                    Name = m.Name,
                    Path = m.Path,
                    Method = m.Method,
                    IsActive = m.IsActive
                }).ToListAsync();
        }

        public async Task<MockViewModel?> GetByIdAsync(int id)
        {
            return await _dbContext.Mocks
                .Where(m => m.Id == id)
                .Include(m => m.Application)
                .Select(m => new MockViewModel
                {
                    Id = m.Id,
                    ApplicationId = m.ApplicationId,
                    ApplicationName = m.Application.Name,
                    Name = m.Name,
                    Path = m.Path,
                    Method = m.Method,
                    IsActive = m.IsActive
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(MockViewModel model)
        {
            var entity = new Mock
            {
                ApplicationId = (int)model.ApplicationId,
                Name = model.Name,
                Path = model.Path ?? "",
                Method = model.Method ?? "",
                IsActive = (bool)model.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "system"
            };
            _dbContext.Mocks.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(MockViewModel model)
        {
            var entity = await _dbContext.Mocks.FindAsync(model.Id);
            if (entity == null) return;

            entity.Name = model.Name;
            entity.Path = model.Path ?? "";
            entity.Method = model.Method ?? "";
            entity.IsActive = (bool)model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = "system";

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Mocks.FindAsync(id);
            if (entity != null)
            {
                _dbContext.Mocks.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
