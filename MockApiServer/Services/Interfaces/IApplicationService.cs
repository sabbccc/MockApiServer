using MockApiServer.Models.ViewModels;

namespace MockApiServer.Services
{
    public interface IApplicationService
    {
        Task<List<ApplicationViewModel>> GetAllAsync();
        Task<ApplicationViewModel?> GetByIdAsync(int id);
        Task AddAsync(ApplicationViewModel model);
        Task UpdateAsync(ApplicationViewModel model);
        Task DeleteAsync(int id);
    }
}
