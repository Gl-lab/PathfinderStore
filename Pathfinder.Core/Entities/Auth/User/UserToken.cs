using System;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Core.Entities.Auth.Users
{
    public class UserToken : IdentityUserToken<Guid>
    {
    }
}