using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Commands;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Common.Exceptions;
using Questao5.Infrastructure.Database;

namespace Exercicio.Tests.Application.Handlers
{
    public class InsertAccountTransactionCommandHandlerTests
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IIdempotentRepository _idempotentRepository;
        private readonly ILogger<InsertAccountTransactionCommandHandler> _logger;
        private readonly InsertAccountTransactionCommandHandler _handler;

        public InsertAccountTransactionCommandHandlerTests()
        {
            _transactionRepository = Substitute.For<ITransactionRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _idempotentRepository = Substitute.For<IIdempotentRepository>();
            _logger = Substitute.For<ILogger<InsertAccountTransactionCommandHandler>>();

            _handler = new InsertAccountTransactionCommandHandler(_transactionRepository, _logger, _accountRepository, _idempotentRepository);

        }

        [Fact]
        public async Task Handle_WhenIsValid_ShouldReturnResponse()
        {
            // Arrange
            var request = new InsertAccountTransactionRequest("req_123", "131615", decimal.One, "C");
            var command = new InsertAccountTransactionCommand(request);

            _idempotentRepository.GetKeyAsync(command.Request.IdRequest).Returns(Task.FromResult("some-key"));
            _accountRepository.GetNumberAccountAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(20));
            _accountRepository.GetNumberAccountActiveAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(20));
            _transactionRepository.InsertTransaction(command.Request).Returns(Task.FromResult(new InsertAccountTransactionResponse("1")));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.IdTransaction);
        }

        [Fact]
        public async Task Handle_WhenIdempotentIsInvalid_ShouldReturnException()
        {
            // Arrange
            var request = new InsertAccountTransactionRequest("req_123", "131615", decimal.One, "C");
            var command = new InsertAccountTransactionCommand(request);

            _idempotentRepository.GetKeyAsync(command.Request.IdRequest).Returns(Task.FromResult<string>(string.Empty));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("It was not possible to proceed with the account transaction because the request identifier req_123 was not found.", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenAccountIsInvalid_ShouldReturnInvalidAccountException()
        {
            // Arrange
            var request = new InsertAccountTransactionRequest("req_123", "131615", decimal.One, "C");
            var command = new InsertAccountTransactionCommand(request);

            _idempotentRepository.GetKeyAsync(command.Request.IdRequest).Returns(Task.FromResult("some-key"));
            _accountRepository.GetNumberAccountAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(0));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidAccountException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"No account exists for this account id {command.Request.AccountCurrentId}.", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenAccountIsInvalid_ShouldReturnInactiveAccountException()
        {
            // Arrange
            var request = new InsertAccountTransactionRequest("req_123", "131615", decimal.One, "D");
            var command = new InsertAccountTransactionCommand(request);

            _idempotentRepository.GetKeyAsync(command.Request.IdRequest).Returns(Task.FromResult("some-key"));
            _accountRepository.GetNumberAccountAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(0));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidAccountException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"No account exists for this account id {command.Request.AccountCurrentId}.", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenDatabaseNotConect_ShouldReturnException()
        {
            // Arrange
            var request = new InsertAccountTransactionRequest("req_123", "131615", decimal.One, "C");
            var command = new InsertAccountTransactionCommand(request);

            _idempotentRepository.GetKeyAsync(command.Request.IdRequest).Returns(Task.FromResult("some-key"));
            _accountRepository.GetNumberAccountAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(20));
            _accountRepository.GetNumberAccountActiveAsync(command.Request.AccountCurrentId).Returns(Task.FromResult(20));
            _transactionRepository.InsertTransaction(command.Request).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
