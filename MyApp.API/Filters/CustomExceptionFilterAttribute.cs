using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Common;
using Newtonsoft.Json;
using System;
using System.Net;

namespace MyApp.API.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception as CustomException !=null)
            {
                HttpStatusCode statusCode = HttpStatusCode.BadRequest;
                //HttpResponse response = context.HttpContext.Response;
                //response.StatusCode = (int)statusCode;
                //response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        isError = true,
                        errorMessage = context.Exception.Message,
                        errorCode = statusCode
                    });
                //response.ContentLength = result.Length;
                //response.Body.WriteAsync(result);
                context.Result = new BadRequestObjectResult(result);
            }
            else
            {
                base.OnException(context);
            }
            //var oresult = new ObjectResult(new
            //{
            //    context.Exception.Message, // Or a different generic message
            //    context.Exception.Source,
            //    ExceptionType = context.Exception.GetType().FullName,
            //});
            //context.Result = oresult;
            //base.OnException(context);
        }
        private HttpStatusCode getErrorCode(Type exceptionType)
        {
            enmExceptions tryParseResult;
            if (Enum.TryParse(exceptionType.Name, out tryParseResult))
            {
                switch (tryParseResult)
                {
                    case enmExceptions.CustomException: 
                        return HttpStatusCode.BadRequest;
                    //case enmExceptions.NullReferenceException:
                    //    return HttpStatusCode.LengthRequired;

                    //case enmExceptions.FileNotFoundException:
                    //    return HttpStatusCode.NotFound;

                    //case enmExceptions.OverflowException:
                    //    return HttpStatusCode.RequestedRangeNotSatisfiable;

                    //case enmExceptions.OutOfMemoryException:
                    //    return HttpStatusCode.ExpectationFailed;

                    //case enmExceptions.InvalidCastException:
                    //    return HttpStatusCode.PreconditionFailed;

                    //case enmExceptions.ObjectDisposedException:
                    //    return HttpStatusCode.Gone;

                    //case enmExceptions.UnauthorizedAccessException:
                    //    return HttpStatusCode.Unauthorized;

                    //case enmExceptions.NotImplementedException:
                    //    return HttpStatusCode.NotImplemented;

                    //case enmExceptions.NotSupportedException:
                    //    return HttpStatusCode.NotAcceptable;

                    //case enmExceptions.InvalidOperationException:
                    //    return HttpStatusCode.MethodNotAllowed;

                    //case enmExceptions.TimeoutException:
                    //    return HttpStatusCode.RequestTimeout;

                    //case enmExceptions.ArgumentException:
                    //    return HttpStatusCode.BadRequest;

                    //case enmExceptions.StackOverflowException:
                    //    return HttpStatusCode.RequestedRangeNotSatisfiable;

                    //case enmExceptions.FormatException:
                    //    return HttpStatusCode.UnsupportedMediaType;

                    //case enmExceptions.IOException:
                    //    return HttpStatusCode.NotFound;

                    //case enmExceptions.IndexOutOfRangeException:
                    //    return HttpStatusCode.ExpectationFailed;

                    default:
                        return HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }
        public enum enmExceptions
        {
            CustomException = 0,
            NullReferenceException = 1,
            FileNotFoundException = 2,
            OverflowException = 3,
            OutOfMemoryException = 4,
            InvalidCastException = 5,
            ObjectDisposedException = 6,
            UnauthorizedAccessException = 7,
            NotImplementedException = 8,
            NotSupportedException = 9,
            InvalidOperationException = 10,
            TimeoutException = 11,
            ArgumentException = 12,
            FormatException = 13,
            StackOverflowException = 14,
            SqlException = 15,
            IndexOutOfRangeException = 16,
            IOException = 17
        }
    }
}