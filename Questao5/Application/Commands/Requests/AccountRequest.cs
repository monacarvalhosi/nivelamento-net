namespace Questao5.Application.Commands.Requests
{
    public class AccountRequest
    {
        public string IdRequest { get; set; }
        public string AccountCurrentId { get; set; }
        public decimal AccountBalance { get; set; }
        public string TransactionType { get; set; }
    }
}
