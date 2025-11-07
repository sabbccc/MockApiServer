using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public interface IApplicationRepository
    {
        Task<List<ApplicationViewModel>> GetAllAsync();
        Task<ApplicationViewModel?> GetByIdAsync(int id);
        Task AddAsync(ApplicationViewModel model);
        Task UpdateAsync(ApplicationViewModel model);
        Task DeleteAsync(int id);
    }
}
