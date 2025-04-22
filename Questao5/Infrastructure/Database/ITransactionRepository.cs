using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Infrastructure.Database
{
    public interface ITransactionRepository
    {
        Task<InsertAccountTransactionResponse> InsertTransaction(InsertAccountTransactionRequest request);
        Task<decimal> GetTotalBalanceAccountAsync(string accountCurrentId);
    }
}
