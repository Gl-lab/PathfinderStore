using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class CreateOrUpdateRoleHandler: IRequestHandler<CreateOrUpdateRoleCommand, IdentityResult>
    {
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrUpdateRoleHandler(IRoleService roleService, IUnitOfWork unitOfWork)
        {
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IdentityResult> Handle(CreateOrUpdateRoleCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                result = await _roleService.AddRoleAsync(request);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            return result;
        }
    }
}