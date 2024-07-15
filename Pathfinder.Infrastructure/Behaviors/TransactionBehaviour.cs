using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly PathfinderDbContext _dbContext;

        public TransactionBehaviour(PathfinderDbContext dbContext,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(PathfinderDbContext));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken )
        {
            TResponse response = default;

            try
            {
                IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation($"Begin transaction {typeof(TRequest).Name}");

                    await _dbContext.BeginTransaction().ConfigureAwait(false);

                    response = await next().ConfigureAwait(false);

                    await _dbContext.Commit().ConfigureAwait(false);

                    _logger.LogInformation($"Committed transaction {typeof(TRequest).Name}");
                }).ConfigureAwait(false);

                return response;
            }
            catch (Exception)
            {
                _logger.LogInformation($"Rollback transaction executed {typeof(TRequest).Name}");

                await _dbContext.Rollback();
                throw;
            }
        }
    }
}
