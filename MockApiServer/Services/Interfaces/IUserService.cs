using MockApiServer.Models.ViewModels;

namespace MockApiServer.Services
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllAsync();
        Task<UserViewModel?> GetByIdAsync(int id);
        Task AddAsync(UserViewModel model);
        Task UpdateAsync(UserViewModel model);
        Task DeleteAsync(int id);
        Task<bool> LoginAsync(LoginViewModel model);

        // 🔐 Auth-specific
        Task<UserViewModel?> AuthenticateAsync(string username, string password);
        Task RegisterAsync(UserViewModel model);
    }
}
