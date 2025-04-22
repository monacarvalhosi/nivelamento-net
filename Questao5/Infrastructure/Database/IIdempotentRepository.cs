namespace Questao5.Infrastructure.Database
{
    public interface IIdempotentRepository
    {
        Task<string> GetKeyAsync(string idRequest);
    }
}
