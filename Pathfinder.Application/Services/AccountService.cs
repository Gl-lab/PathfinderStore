using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Mapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Interfaces;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Paging;
using AutoMapper;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using System.Linq;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Services
{
    public class AccountService: IAccountService
    {
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepository;
        private readonly IUserService userService;
        public AccountService(IAccountRepository accountRepository, 
            IMapper mapper, 
            IUserService userService)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
                
        }

        public async Task<AccountDto> Get()
        {
            var user = userService.GetCurrentUser();
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            return mapper.Map<AccountDto>(account);
        }

        public async Task<AccountDto> Update(AccountDto newAccount)
        {
            var user = userService.GetCurrentUser() ?? throw new ArgumentNullException("userService.GetCurrentUser()");
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            if (account == null) throw new ArgumentNullException("Account for user not exists");
            account.Name = newAccount.Name;
            account.Surname = newAccount.Surname;
            var result = await accountRepository.SaveAsync(account).ConfigureAwait(false);
            return mapper.Map<AccountDto>(result);
        }
    }
}
