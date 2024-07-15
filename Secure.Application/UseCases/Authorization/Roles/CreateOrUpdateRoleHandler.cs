using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Utils.UnitOfWork;
using Secure.Application.Services.Authentication;

namespace Secure.Application.UseCases.Authorization.Roles
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
                await _unitOfWork.Commit();
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }
            return result;
        }
    }
}