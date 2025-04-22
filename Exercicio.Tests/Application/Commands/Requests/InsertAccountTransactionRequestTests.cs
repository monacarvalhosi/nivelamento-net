using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Questao5.Application.Commands.Requests;
using Questao5.Common.Exceptions;

namespace Exercicio.Tests.Application.Commands.Requests
{
    public class InsertAccountTransactionRequestTests
    {
        [Fact]
        public void InsertAccountTransactionRequest_WhenCompleteProperties_ShouldReturnSuccess()
        {
            // Arrange
            var idRequest = "12345";
            var accountCurrentId = "A1B2C3D4E5F6";
            var accountBalance = 1000;
            var transactionType = "C";

            // Act
            var accountRequest = new AccountRequest
            {
                IdRequest = idRequest,
                AccountCurrentId = accountCurrentId,
                AccountBalance = accountBalance,
                TransactionType = transactionType
            };

            // Assert
            Assert.Equal(idRequest, accountRequest.IdRequest);
            Assert.Equal(accountCurrentId, accountRequest.AccountCurrentId);
            Assert.Equal(accountBalance, accountRequest.AccountBalance);
            Assert.Equal(transactionType, accountRequest.TransactionType);
        }

        [Fact]
        public void InsertAccountTransactionRequest_WhenIdRequestNullorEmpty_ShouldReturnException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest(null, "AccountId", 100, "C"));
            Assert.Equal("IdRequest is required and must be a non-empty string.", exception.Message);

            exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest(string.Empty, "AccountId", 100, "C"));
            Assert.Equal("IdRequest is required and must be a non-empty string.", exception.Message);
        }

        [Fact]
        public void InsertAccountTransactionRequest_WhenAccountCurrentIdNullorEmpty_ShouldReturnException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("RequestId", null, 100, "C"));
            Assert.Equal("AccountCurrentId is required and must be a non-empty string.", exception.Message);

            exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("RequestId", string.Empty, 100, "C"));
            Assert.Equal("AccountCurrentId is required and must be a non-empty string.", exception.Message);
        }

        [Fact]
        public void InsertAccountTransactionRequest_WhenAccountBalanceThen1_ShouldReturnException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("RequestId", "AccountId", 0, "C"));
            Assert.Equal("AccountBalance is required and must be greater than zero.", exception.Message);

            exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("RequestId", "AccountId", -1, "C"));
            Assert.Equal("AccountBalance is required and must be greater than zero.", exception.Message);
        }

        [Fact]
        public void InsertAccountTransactionRequest_WhenTypeAccountDistinctCOrD_ShouldReturnException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("ValidRequestId", "ValidAccountId", 100.00m, "Invalid"));
            Assert.Equal("Type transaction account 'Invalid' is invalid.", exception.Message);

            exception = Assert.Throws<InvalidValueException>(() => new InsertAccountTransactionRequest("ValidRequestId", "ValidAccountId", 100.00m, "B"));
            Assert.Equal("Type transaction account 'B' is invalid.", exception.Message);
        }
    }
}
