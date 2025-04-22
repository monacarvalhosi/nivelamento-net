using Questao5.Infrastructure.Database.CommandStore.Responses;

namespace Questao5.Infrastructure.Database
{
    public interface IAccountRepository
    {
        Task<int> GetNumberAccountAsync(string accountCurrentId);
        Task<int> GetNumberAccountActiveAsync(string accountCurrentId);
        Task<GetInfoAccountBalanceReponse> GeInfoAccountAsync(string accountCurrentId);
    }
}
