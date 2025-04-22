using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands;
using Questao5.Application.Commands.Requests;
using Questao5.Common.Exceptions;
using Questao5.Common.Responses;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountCurrentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountCurrentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountRequest request)
        {
            try
            {
                var requestAccount = new InsertAccountTransactionRequest(request.IdRequest, request.AccountCurrentId, request.AccountBalance, request.TransactionType);

                var command = new InsertAccountTransactionCommand(requestAccount);
                var response = await _mediator.Send(command);

                return Ok(response.IdTransaction);
            }
            catch (InvalidValueException ex)
            {
                var errorResponse = new ErrorResponse(ex.Message, "ValidationError");
                return BadRequest(errorResponse);
            }
            catch (InvalidAccountException ex)
            {
                var errorResponse = new ErrorResponse(ex.Message, "AccountError");
                return BadRequest(errorResponse);
            }
            catch (InactiveAccountException ex)
            {
                var errorResponse = new ErrorResponse(ex.Message, "AccountInactiveError");
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse("An unexpected error occurred.", "SystemError");
                return BadRequest(errorResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string accountCurrentId) 
        {
            try
            {
                var command = new GetAccountBalanceCommand(accountCurrentId);
                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse("An unexpected error occurred.", "SystemError");
                return BadRequest(errorResponse);
            }
        }
    }
}