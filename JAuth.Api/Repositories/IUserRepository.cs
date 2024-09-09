using JAuth.Api.Data;
using JAuth.UserEntityModels;

namespace JAuth.Api.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> CreateUserAsync(RegisterRequest registerRequest);
        Task<ApplicationUser?> UpdateUserAsync(UpdateAccountRequest updateRequest, ApplicationUser user);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> DeleteUserAsync(ApplicationUser user);
    }

}
