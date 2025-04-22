namespace Questao5.Common.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string ErrorType { get; set; }

        public ErrorResponse(string message, string errorType)
        {
            Message = message;
            ErrorType = errorType;
        }
    }
}
