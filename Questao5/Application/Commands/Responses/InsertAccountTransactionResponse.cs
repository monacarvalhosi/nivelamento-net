namespace Questao5.Application.Commands.Responses
{
    public class InsertAccountTransactionResponse
    {
        public string IdTransaction { get; set; }

        public InsertAccountTransactionResponse(string idTransaction)
        {
            IdTransaction = idTransaction;
        }
    }
}
