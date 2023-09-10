using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactApi.Filters
{
    public class GlobalExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ProblemDetailsException exception)
            {
                context.Result = new ObjectResult(exception.Value)
                {
                    StatusCode = exception.Value.Status
                };

                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
