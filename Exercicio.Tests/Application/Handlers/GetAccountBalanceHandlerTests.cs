using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Commands;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Common.Exceptions;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Database.CommandStore.Responses;

namespace Exercicio.Tests.Application.Handlers
{
    public class GetAccountBalanceHandlerTests
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAccountBalanceHandler> _logger;
        private readonly GetAccountBalanceHandler _handler;

        public GetAccountBalanceHandlerTests()
        {
            _transactionRepository = Substitute.For<ITransactionRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _logger = Substitute.For<ILogger<GetAccountBalanceHandler>>();

            // Instanciando o handler
            _handler = new GetAccountBalanceHandler(_logger, _transactionRepository, _accountRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectBalance_WhenAccountIsValid()
        {
            var accountCurrentId = "151515";

            var command = new GetAccountBalanceCommand(accountCurrentId);

            _accountRepository.GetNumberAccountAsync(accountCurrentId).Returns(Task.FromResult(10));
            _accountRepository.GetNumberAccountActiveAsync(accountCurrentId).Returns(Task.FromResult(10));
            _accountRepository.GeInfoAccountAsync(accountCurrentId).Returns(Task.FromResult(new GetInfoAccountBalanceReponse
            {
                numero = 130625,
                nome = "Hommer"
            }));

            _transactionRepository.GetTotalBalanceAccountAsync(accountCurrentId).Returns(Task.FromResult(decimal.One));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(130625, result.AccountNumber);
            Assert.Equal("Hommer", result.AccountTitularity);
            Assert.Equal(decimal.One, result.AccountBalance);
        }

        [Fact]
        public async Task Handle__WhenAccountCurrentInvalid_ShouldReturnInvalidAccountException()
        {
            var accountCurrentId = "131313";
            var command = new GetAccountBalanceCommand(accountCurrentId);

            _accountRepository.GetNumberAccountAsync(accountCurrentId).Returns(Task.FromResult(0));

            var exception = await Assert.ThrowsAsync<InvalidAccountException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"No account exists for this account id 131313.", exception.Message);
        }

        [Fact]
        public async Task Handle__WhenAccountCurrentInvalid_ShouldReturnInactiveAccountException()
        {
            var accountCurrentId = "131313";
            var command = new GetAccountBalanceCommand(accountCurrentId);

            _accountRepository.GetNumberAccountAsync(accountCurrentId).Returns(Task.FromResult(10));
            _accountRepository.GetNumberAccountActiveAsync(accountCurrentId).Returns(Task.FromResult(0));

            var exception = await Assert.ThrowsAsync<InactiveAccountException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("No active account exists for this account id 131313.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
        {
            var accountCurrentId = "151515";
            var command = new GetAccountBalanceCommand(accountCurrentId);

            _accountRepository.GetNumberAccountAsync(accountCurrentId).Returns(Task.FromResult(10));
            _accountRepository.GetNumberAccountActiveAsync(accountCurrentId).Returns(Task.FromResult(10));
            _accountRepository.GeInfoAccountAsync(accountCurrentId).Throws(new Exception("Test exception"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        }

    }
}
