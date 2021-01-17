using System;
using System.Collections.Generic;

namespace Pathfinder.Application.Models.Auth.Users
{
    public class CreateOrUpdateUserInput
    {
        public UserModel User { get; set; } = new UserModel();

        public List<int> GrantedRoleIds { get; set; } = new List<int>();
    }
}
