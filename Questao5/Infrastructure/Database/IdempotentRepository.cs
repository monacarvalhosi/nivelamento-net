using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Polly.Retry;
using Polly;

namespace Questao5.Infrastructure.Database
{
    public class IdempotentRepository : IIdempotentRepository
    {
        private readonly IDbConnection _db;

        private static readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<SqliteException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        public IdempotentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<string> GetKeyAsync(string idRequest)
        {
            var key = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.ExecuteScalarAsync<string>(
                    "SELECT chave_idempotencia FROM idempotencia WHERE requisicao = @Request AND resultado = 'success'",
                    new { Request = idRequest }
                );
            });

            return key;
        }
    }
}
