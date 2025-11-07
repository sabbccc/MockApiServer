using MockApiServer.Models.ViewModels;

namespace MockApiServer.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserViewModel>> GetAllAsync();
        Task<UserViewModel?> GetByIdAsync(int id);
        Task AddAsync(UserViewModel model);
        Task UpdateAsync(UserViewModel model);
        Task DeleteAsync(int id);
    }
}
