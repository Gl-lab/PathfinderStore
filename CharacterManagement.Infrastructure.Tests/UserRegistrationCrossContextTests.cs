using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure;
using Pathfinder.CharacterManagement.Infrastructure.Consumers;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Secure.Application;
using Pathfinder.Secure.Application.DTO.Authentication.Account;
using Pathfinder.Secure.Application.UseCases.Authorization.Account;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Infrastructure;
using Pathfinder.Secure.Infrastructure.Data;
using Pathfinder.Utils.UnitOfWork;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class UserRegistrationCrossContextTests
{
    [Fact]
    public async Task RegisterUser_CreatesAuthorizedUserAndCharacterManagementAccount()
    {
        ServiceProvider serviceProvider = CreateServiceProvider();
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        SecureDbContext secureDbContext = scope.ServiceProvider.GetRequiredService<SecureDbContext>();
        CharacterManagementDbContext characterManagementDbContext = scope.ServiceProvider.GetRequiredService<CharacterManagementDbContext>();
        secureDbContext.Database.EnsureCreated();
        characterManagementDbContext.Database.EnsureCreated();

        IBusControl bus = scope.ServiceProvider.GetRequiredService<IBusControl>();
        await bus.StartAsync();

        try
        {
            IRequestHandler<RegisterUserCommand, RegisterUserOutput> handler = scope.ServiceProvider
                .GetRequiredService<IRequestHandler<RegisterUserCommand, RegisterUserOutput>>();
            RegisterUserCommand command = new RegisterUserCommand(
                "newmember",
                "newmember@mail.com",
                "123qwe",
                "New",
                "Member" );

            RegisterUserOutput result = await handler.Handle( command, CancellationToken.None );

            Assert.True( result.IdentityResult.Succeeded );
            Assert.NotNull( result.UserId );

            Account account = await WaitForAccount( serviceProvider, result.UserId.Value );
            User user = await secureDbContext.Users
                .AsNoTracking()
                .SingleAsync( candidate => candidate.Id == result.UserId.Value );
            UserRole userRole = await secureDbContext.UserRoles
                .AsNoTracking()
                .SingleAsync( candidate => candidate.UserId == user.Id );
            bool hasMemberPermission = await secureDbContext.RolePermission
                .AsNoTracking()
                .AnyAsync( candidate =>
                    candidate.RoleId == userRole.RoleId
                    && candidate.PermissionId == DefaultPermissions.MemberAccess.Id );

            Assert.Equal( command.UserName, user.UserName );
            Assert.Equal( command.Email, user.Email );
            Assert.True( user.EmailConfirmed );
            Assert.False( String.IsNullOrWhiteSpace( user.PasswordHash ) );
            Assert.Equal( DefaultRoles.Member.Id, userRole.RoleId );
            Assert.True( hasMemberPermission );
            Assert.Equal( user.Id, account.UserId );
            Assert.Equal( command.Name, account.Name );
            Assert.Equal( command.Surname, account.Surname );
        }
        finally
        {
            await bus.StopAsync();
            await serviceProvider.DisposeAsync();
        }
    }

    private static ServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        string secureDatabaseName = $"secure-{Guid.NewGuid():N}";
        string characterManagementDatabaseName = $"character-management-{Guid.NewGuid():N}";

        services.AddLogging();
        services.AddDbContext<SecureDbContext>( options => options.UseInMemoryDatabase( secureDatabaseName ) );
        services.AddDbContext<CharacterManagementDbContext>( options => options.UseInMemoryDatabase( characterManagementDatabaseName ) );
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<SecureDbContext>();
        services.Configure<IdentityOptions>( options =>
        {
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 2;
        } );
        services.AddSecureInfrastructureServices();
        services.AddSecureApplicationServices();
        services.AddCharacterManagementInfrastructureServices();
        services.AddCharacterManagementApplicationServices();
        services.AddScoped<IUnitOfWork, CrossContextUnitOfWork>();
        services.AddMassTransit( configurator =>
        {
            configurator.AddConsumer<UserRegisteredEventConsumer>();
            configurator.UsingInMemory( ( context, busConfigurator ) =>
            {
                busConfigurator.ConfigureEndpoints( context );
            } );
        } );

        return services.BuildServiceProvider();
    }

    private static async Task<Account> WaitForAccount( ServiceProvider serviceProvider, int userId )
    {
        for ( int attempt = 0; attempt < 50; attempt++ )
        {
            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
            CharacterManagementDbContext dbContext = scope.ServiceProvider.GetRequiredService<CharacterManagementDbContext>();
            Account? account = await dbContext.Account
                .AsNoTracking()
                .SingleOrDefaultAsync( candidate => candidate.UserId == userId );
            if ( account is not null )
            {
                return account;
            }

            await Task.Delay( 20 );
        }

        throw new TimeoutException( $"Account for user {userId} was not created." );
    }

    private sealed class CrossContextUnitOfWork : IUnitOfWork
    {
        private readonly CharacterManagementDbContext _characterManagementDbContext;
        private readonly SecureDbContext _secureDbContext;

        public CrossContextUnitOfWork(
            SecureDbContext secureDbContext,
            CharacterManagementDbContext characterManagementDbContext )
        {
            _secureDbContext = secureDbContext;
            _characterManagementDbContext = characterManagementDbContext;
        }

        public async Task Commit()
        {
            await _secureDbContext.SaveChangesAsync();
            await _characterManagementDbContext.SaveChangesAsync();
        }

        public Task Rollback() => Task.CompletedTask;

        public Task BeginTransaction() => Task.CompletedTask;
    }
}
