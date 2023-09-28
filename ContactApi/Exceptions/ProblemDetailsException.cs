using Microsoft.AspNetCore.Mvc;

namespace ContactApi.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetailsException(ProblemDetails problemDetails)
        {
            Value = problemDetails;
        }

        public ProblemDetails Value { get; }
    }
}
