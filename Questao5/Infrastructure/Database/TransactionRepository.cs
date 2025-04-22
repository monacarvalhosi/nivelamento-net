using Polly;
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Enumerators;
using Microsoft.Data.Sqlite;
using Polly.Retry;

namespace Questao5.Infrastructure.Database
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _db;

        private static readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<SqliteException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        public TransactionRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<InsertAccountTransactionResponse> InsertTransaction(InsertAccountTransactionRequest request)
        {
            var insertTransaction = @"
                INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);
                SELECT last_insert_rowid();";

            var transactionId = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<string>(insertTransaction, new
                {
                    IdMovimento = Guid.NewGuid().ToString(),
                    IdContaCorrente = request.AccountCurrentId.ToString(),
                    DataMovimento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TipoMovimento = request.TransactionType == TransactionTypeEnum.Debit ? "D" : "C",
                    Valor = request.AccountBalance
                });
            });

            return new InsertAccountTransactionResponse(transactionId);
        }

        public async Task<decimal> GetTotalBalanceAccountAsync(string accountCurrentId)
        {
            var balanceDebit = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<decimal?>(
                    "SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @Id AND tipoMovimento = 'D'",
                    new { Id = accountCurrentId }
                ) ?? 0;
            });

            var balanceCredit = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<decimal?>(
                    "SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @Id AND tipoMovimento = 'C'",
                    new { Id = accountCurrentId }
                ) ?? 0;
            });

            return (balanceCredit - balanceDebit);
        }
    }
}
