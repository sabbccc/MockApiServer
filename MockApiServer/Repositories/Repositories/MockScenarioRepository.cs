using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Data.Entities;
using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public class MockScenarioRepository : IMockScenarioRepository
    {
        private readonly MockApiServerDbContext _context;

        public MockScenarioRepository(MockApiServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<MockScenarioViewModel>> GetAllAsync()
        {
            return await _context.MockScenarios
                .Include(s => s.Mock)
                .Select(s => new MockScenarioViewModel
                {
                    Id = s.Id,
                    MockId = s.MockId,
                    MockName = s.Mock.Name,
                    ScenarioKey = s.ScenarioKey,
                    StatusCode = s.StatusCode,
                    ResponseJson = s.ResponseJson,
                    HeadersJson = s.HeadersJson,
                    IsActive = s.IsActive
                }).ToListAsync();
        }

        public async Task<List<MockScenarioViewModel>> GetByMockIdAsync(int mockId)
        {
            return await _context.MockScenarios
                .Where(s => s.MockId == mockId)
                .Include(s => s.Mock)
                .Select(s => new MockScenarioViewModel
                {
                    Id = s.Id,
                    MockId = s.MockId,
                    MockName = s.Mock.Name,
                    ScenarioKey = s.ScenarioKey,
                    StatusCode = s.StatusCode,
                    ResponseJson = s.ResponseJson,
                    HeadersJson = s.HeadersJson,
                    IsActive = s.IsActive
                }).ToListAsync();
        }

        public async Task<MockScenarioViewModel?> GetByIdAsync(int id)
        {
            return await _context.MockScenarios
                .Where(s => s.Id == id)
                .Include(s => s.Mock)
                .Select(s => new MockScenarioViewModel
                {
                    Id = s.Id,
                    MockId = s.MockId,
                    MockName = s.Mock.Name,
                    ScenarioKey = s.ScenarioKey,
                    StatusCode = s.StatusCode,
                    ResponseJson = s.ResponseJson,
                    HeadersJson = s.HeadersJson,
                    IsActive = s.IsActive
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(MockScenarioViewModel model)
        {
            var entity = new MockScenario
            {
                MockId = (int)model.MockId,
                ScenarioKey = model.ScenarioKey ?? "",
                StatusCode = (int)model.StatusCode,
                ResponseJson = model.ResponseJson ?? "",
                HeadersJson = model.HeadersJson,
                IsActive = (bool)model.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "system"
            };
            _context.MockScenarios.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MockScenarioViewModel model)
        {
            var entity = await _context.MockScenarios.FindAsync(model.Id);
            if (entity == null) return;

            entity.ScenarioKey = model.ScenarioKey ?? "";
            entity.StatusCode = (int)model.StatusCode;
            entity.ResponseJson = model.ResponseJson ?? "";
            entity.HeadersJson = model.HeadersJson;
            entity.IsActive = (bool)model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.MockScenarios.FindAsync(id);
            if (entity != null)
            {
                _context.MockScenarios.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
