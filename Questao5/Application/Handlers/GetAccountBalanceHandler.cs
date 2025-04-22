using Questao5.Application.Commands.Responses;
using Questao5.Application.Commands;
using Questao5.Infrastructure.Database;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Common.Exceptions;

namespace Questao5.Application.Handlers
{
    public class GetAccountBalanceHandler : IRequestHandler<GetAccountBalanceCommand, GetAccountBalanceResponse>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAccountBalanceHandler> _logger;

        public GetAccountBalanceHandler(ILogger<GetAccountBalanceHandler> logger,
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<GetAccountBalanceResponse> Handle(GetAccountBalanceCommand command, CancellationToken cancellationToken)
        {
            var accountCurrentId = command.AccountCurrentId;
            ValidateAccountBalance(accountCurrentId);

            try
            {            
                var responseAccount = await _accountRepository.GeInfoAccountAsync(accountCurrentId);

                var responseTransaction = await _transactionRepository.GetTotalBalanceAccountAsync(accountCurrentId);

                return new GetAccountBalanceResponse(responseTransaction, responseAccount.numero, responseAccount.nome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting ballance account.");
                throw new ApplicationException("Error processing the getting ballance account.");
            }
        }

        private void ValidateAccountBalance(string accountCurrentId)
        {
            var numberAccount = _accountRepository.GetNumberAccountAsync(accountCurrentId).Result;
            if (numberAccount < decimal.One)
                throw new InvalidAccountException($"No account exists for this account id {accountCurrentId}.");

            var numberAccountActive = _accountRepository.GetNumberAccountActiveAsync(accountCurrentId).Result;
            if (numberAccountActive < decimal.One)
                throw new InactiveAccountException($"No active account exists for this account id {accountCurrentId}.");
        }
    }
}
