using Authorization.Authentication.User;

namespace Secure.Application.Repositories;

public interface IUserRepository
{
    public Task<User?> GetUserByNameOrEmail( string userNameOrEmail );
}