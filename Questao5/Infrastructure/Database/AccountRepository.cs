using Dapper;
using Polly;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using Microsoft.Data.Sqlite;
using Polly.Retry;

namespace Questao5.Infrastructure.Database
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _db;
        private static readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<SqliteException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        public AccountRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> GetNumberAccountAsync(string accountCurrentId)
        {
            var numberAccount = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<int?>(
                    "SELECT numero FROM contacorrente WHERE idcontacorrente = @Id",
                    new { Id = accountCurrentId }
                );
            });

            return numberAccount ?? 0;
        }

        public async Task<int> GetNumberAccountActiveAsync(string accountCurrentId)
        {
            var numberAccount = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<int?>(
                    "SELECT numero FROM contacorrente WHERE idcontacorrente = @Id and ativo = 1",
                    new { Id = accountCurrentId }
                );
            });

            return numberAccount ?? 0;
        }

        public async Task<GetInfoAccountBalanceReponse> GeInfoAccountAsync(string accountCurrentId)
        {
            var infoAccount = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.QueryFirstOrDefaultAsync<GetInfoAccountBalanceReponse>(
                    "SELECT numero, nome FROM contacorrente WHERE idcontacorrente = @Id",
                    new { Id = accountCurrentId }
                );
            });

            return infoAccount;
        }
    }

}
