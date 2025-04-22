using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands
{
    public class InsertAccountTransactionCommand : IRequest<InsertAccountTransactionResponse>
    {
        public InsertAccountTransactionRequest Request { get; }

        public InsertAccountTransactionCommand(InsertAccountTransactionRequest request)
        {
            Request = request;
        }
    }
}
