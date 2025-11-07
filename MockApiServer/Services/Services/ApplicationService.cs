using MockApiServer.Models.ViewModels;
using MockApiServer.Repositories;

namespace MockApiServer.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repo;

        public ApplicationService(IApplicationRepository repo)
        {
            _repo = repo;
        }

        public Task<List<ApplicationViewModel>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ApplicationViewModel?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(ApplicationViewModel model) => _repo.AddAsync(model);
        public Task UpdateAsync(ApplicationViewModel model) => _repo.UpdateAsync(model);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
