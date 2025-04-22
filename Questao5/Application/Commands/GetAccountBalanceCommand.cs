using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands
{
    public class GetAccountBalanceCommand  : IRequest<GetAccountBalanceResponse>
    {
        public string AccountCurrentId { get; }

        public GetAccountBalanceCommand(string accountCurrentId)
        {
            AccountCurrentId = accountCurrentId;
        }
    }
}
