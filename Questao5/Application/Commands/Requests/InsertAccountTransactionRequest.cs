using Questao5.Common.Exceptions;
using Questao5.Domain.Enumerators;

namespace Questao5.Application.Commands.Requests
{
    public class InsertAccountTransactionRequest
    {
        public string IdRequest { get; set; }
        public string AccountCurrentId { get; set; }
        public decimal AccountBalance { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        public InsertAccountTransactionRequest(string idRequest, string accountCurrentId, decimal accountBalance, string transactionType)
        {
            if (string.IsNullOrWhiteSpace(idRequest))
                throw new InvalidValueException("IdRequest is required and must be a non-empty string.");

            if (string.IsNullOrWhiteSpace(accountCurrentId))
                throw new InvalidValueException("AccountCurrentId is required and must be a non-empty string.");

            if (accountBalance < decimal.One)
                throw new InvalidValueException("AccountBalance is required and must be greater than zero.");

            IdRequest = idRequest;
            AccountCurrentId = accountCurrentId;
            AccountBalance = accountBalance;
            TransactionType = ConvertToEnum(transactionType);
        }

        private TransactionTypeEnum ConvertToEnum(string typeString)
        {
            if (typeString.Length != 1)
            {
                throw new InvalidValueException($"Type transaction account '{typeString}' is invalid.");
            }

            char typeChar = typeString[0];
            switch (typeChar)
            {
                case 'C':
                    return TransactionTypeEnum.Credit;
                case 'D':
                    return TransactionTypeEnum.Debit;
                default:
                    throw new InvalidValueException($"Type transaction account '{typeString}' is invalid.");
            }
        }
    }
}
