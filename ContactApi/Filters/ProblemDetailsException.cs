using Microsoft.AspNetCore.Mvc;

namespace ContactApi.Filters
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
