using MockApiServer.Models.ViewModels;
using MockApiServer.Repositories;

namespace MockApiServer.Services
{
    public class MockScenarioService : IMockScenarioService
    {
        private readonly IMockScenarioRepository _repo;

        public MockScenarioService(IMockScenarioRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MockScenarioViewModel>> GetAllAsync() => _repo.GetAllAsync();
        public Task<List<MockScenarioViewModel>> GetByMockIdAsync(int mockId) => _repo.GetByMockIdAsync(mockId);
        public Task<MockScenarioViewModel?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(MockScenarioViewModel model) => _repo.AddAsync(model);
        public Task UpdateAsync(MockScenarioViewModel model) => _repo.UpdateAsync(model);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
