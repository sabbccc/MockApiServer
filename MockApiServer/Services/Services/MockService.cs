using MockApiServer.Models.ViewModels;
using MockApiServer.Repositories;

namespace MockApiServer.Services
{
    public class MockService : IMockService
    {
        private readonly IMockRepository _repo;

        public MockService(IMockRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MockViewModel>> GetByApplicationIdAsync(int appId) => _repo.GetByApplicationIdAsync(appId);
        public Task<List<MockViewModel>> GetAllAsync() => _repo.GetAllAsync();
        public Task<MockViewModel?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(MockViewModel model) => _repo.AddAsync(model);
        public Task UpdateAsync(MockViewModel model) => _repo.UpdateAsync(model);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
