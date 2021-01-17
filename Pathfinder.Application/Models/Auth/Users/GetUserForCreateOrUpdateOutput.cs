using System;
using System.Collections.Generic;
using Pathfinder.Application.Models.Auth.Roles;

namespace Pathfinder.Application.Models.Auth.Users
{
    public class GetUserForCreateOrUpdateOutput
    {
        public UserModel User { get; set; } = new UserModel();

        public List<RoleModel> AllRoles { get; set; } = new List<RoleModel>();

        public List<int> GrantedRoleIds { get; set; } = new List<int>();
    }
}