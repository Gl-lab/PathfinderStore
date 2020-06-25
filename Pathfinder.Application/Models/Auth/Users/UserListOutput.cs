using System;
using Pathfinder.Application.Models.Auth.Base;

namespace Pathfinder.Application.Models.Auth.Users
{
    public class UserListOutput: BaseAuthModel
    { 
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}