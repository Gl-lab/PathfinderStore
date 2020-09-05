using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Core.Entities;

namespace Pathfinder.Infrastructure.Data
{
    public static class IdentitySeedData
    {
        private const String adminUser = "Admin";
        private const String adminPassword = "Secret123$";
        public static async void  EnsurePopulated(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser user = await userManager.FindByNameAsync(adminUser).ConfigureAwait(false);
            if (user == null)
            {
                user = new ApplicationUser(adminUser);
                await userManager.CreateAsync(user, adminPassword).ConfigureAwait(false);
            }
        }
    }
}
