using MediatR;
using Questao5.Application.Commands;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Common.Exceptions;
using Questao5.Infrastructure.Database;

namespace Questao5.Application.Handlers
{
    public class InsertAccountTransactionCommandHandler : IRequestHandler<InsertAccountTransactionCommand, InsertAccountTransactionResponse>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IIdempotentRepository _idempotentRepository;
        private readonly ILogger<InsertAccountTransactionCommandHandler> _logger;

        public InsertAccountTransactionCommandHandler(ITransactionRepository transactionRepository, 
            ILogger<InsertAccountTransactionCommandHandler> logger, 
            IAccountRepository accountRepository,
            IIdempotentRepository idempotentRepository)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
            _accountRepository = accountRepository;
            _idempotentRepository = idempotentRepository;
        }

        public async Task<InsertAccountTransactionResponse> Handle(InsertAccountTransactionCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;
            ValidateInsertAccountTransaction(req);

            try
            {
                var response = await _transactionRepository.InsertTransaction(req);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while inserting transaction.");
                throw new ApplicationException("Error processing the account transaction.");
            }
        }

        private void ValidateInsertAccountTransaction(InsertAccountTransactionRequest req)
        {
            var key = _idempotentRepository.GetKeyAsync(req.IdRequest).Result;
            if (string.IsNullOrEmpty(key))
                throw new Exception($"It was not possible to proceed with the account transaction because the request identifier {req.IdRequest} was not found.");

            var numberAccount = _accountRepository.GetNumberAccountAsync(req.AccountCurrentId).Result;
            if (numberAccount < decimal.One)
                throw new InvalidAccountException($"No account exists for this account id {req.AccountCurrentId}.");

            var numberAccountActive = _accountRepository.GetNumberAccountActiveAsync(req.AccountCurrentId).Result;
            if (numberAccountActive < decimal.One)
                throw new InvalidAccountException($"No active account exists for this account id {req.AccountCurrentId}.");
        }
    }
}
