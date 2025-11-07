using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public interface IMockScenarioRepository
    {
        Task<List<MockScenarioViewModel>> GetAllAsync();
        Task<List<MockScenarioViewModel>> GetByMockIdAsync(int mockId);
        Task<MockScenarioViewModel?> GetByIdAsync(int id);
        Task AddAsync(MockScenarioViewModel model);
        Task UpdateAsync(MockScenarioViewModel model);
        Task DeleteAsync(int id);
    }
}
