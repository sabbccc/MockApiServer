using MockApiServer.Data.Entities;
using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public interface IMockRepository
    {
        Task<Mock?> GetMockWithScenariosAsync(string path, string method);
        Task<List<MockViewModel>> GetByApplicationIdAsync(int appId);
        Task<List<MockViewModel>> GetAllAsync();
        Task<MockViewModel?> GetByIdAsync(int id);
        Task AddAsync(MockViewModel model);
        Task UpdateAsync(MockViewModel model);
        Task DeleteAsync(int id);
    }
}
