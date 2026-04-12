using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByNameOrEmail( string userNameOrEmail );
}