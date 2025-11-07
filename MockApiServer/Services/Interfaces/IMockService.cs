using MockApiServer.Models.ViewModels;

namespace MockApiServer.Services
{
    public interface IMockService
    {
        Task<List<MockViewModel>> GetByApplicationIdAsync(int appId);
        Task<List<MockViewModel>> GetAllAsync();
        Task<MockViewModel?> GetByIdAsync(int id);
        Task AddAsync(MockViewModel model);
        Task UpdateAsync(MockViewModel model);
        Task DeleteAsync(int id);
    }
}
