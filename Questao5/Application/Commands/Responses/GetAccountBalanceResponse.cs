namespace Questao5.Application.Commands.Responses
{
    public class GetAccountBalanceResponse
    {
        public int? AccountNumber { get; set; }
        public string AccountTitularity { get; set; }

        public DateTime AccountDate => DateTime.Now;
        public decimal AccountBalance { get; set; }

        public GetAccountBalanceResponse(decimal accountBalance, int? accountNumber, string accountTitularity)
        {
            AccountBalance = accountBalance;
            AccountNumber = accountNumber;
            AccountTitularity = accountTitularity;
        }
    }
}
