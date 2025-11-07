using MockApiServer.Extensions;
using MockApiServer.Models.ViewModels;
using MockApiServer.Repositories;

namespace MockApiServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly string _saltKey;
        public UserService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _saltKey = config["ApplicationConfiguration:SaltKey"] ?? throw new Exception("SaltKey is missing.");
        }

        public Task<List<UserViewModel>> GetAllAsync() => _repo.GetAllAsync();

        public Task<UserViewModel?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(UserViewModel model) => _repo.AddAsync(model);

        public Task UpdateAsync(UserViewModel model) => _repo.UpdateAsync(model);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

        public Task<bool> LoginAsync(LoginViewModel model)
        {
            throw new NotImplementedException();
        }


        // Register with hashed password
        public async Task RegisterAsync(UserViewModel model)
        {
            model.Password = SaltEncryption.HashPassword(model.Password ?? "", _saltKey);
            model.CreatedAt = DateTime.Now;
            model.IsActive = true;
            await _repo.AddAsync(model);
        }

        // Authenticate user with salt verification
        public async Task<UserViewModel?> AuthenticateAsync(string username, string password)
        {
            var users = await _repo.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;

            var isValid = SaltEncryption.VerifyPassword(password, user.Password ?? "", _saltKey);
            return isValid ? user : null;
        }
    }
}
