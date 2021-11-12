using System;
using System.Net;
using ETLib.Models.Constants;

namespace ETLib.Models.QueryResponse
{
    public class ResponseBaseModel<T> where T : class
    {
        public T Model { get; set; }
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        protected ResponseBaseModel(HttpStatusCode httpStatusCode, bool success, string message, in T t)
        {
            Model = t;
            Success = success;
            Message = message;
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Create a error response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="httpStatusCode"></param>
        /// <returns>Response</returns>
        protected ResponseBaseModel(string message, HttpStatusCode httpStatusCode) : this(httpStatusCode,false, message, null)
        {
        }

        public static ResponseBaseModel<T> GetSuccessResponse(T t)
        {
            return new ResponseBaseModel<T>(HttpStatusCode.OK,true, String.Empty, t);
        }

        /// <summary>
        /// create a notfound response
        /// </summary>
        /// <returns></returns>
        public static ResponseBaseModel<T> GetNotFoundResponse()
        {
            return new ResponseBaseModel<T>(HttpStatusCode.NotFound,false,$"{Types.ResponseMessages.NotFound}: {typeof(T)}", null);
        }

        /// <summary>
        /// create a notfound response with a give type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> GetNotFoundResponse(Type type)
        {
            return new ResponseBaseModel<T>(HttpStatusCode.NotFound,false, $"{Types.ResponseMessages.NotFound}: {typeof(T)}", null);
        }

        public static ResponseBaseModel<T> GetDbSaveFailedResponse()
        {
            return new ResponseBaseModel<T>(HttpStatusCode.BadRequest,false, $"{Types.ResponseMessages.DbSaveFailed}: {typeof(T)}",null);
        }

        public static ResponseBaseModel<T> GetNotAuthorizedResponse()
        {
            return new ResponseBaseModel<T>(HttpStatusCode.Forbidden,false, Types.ResponseMessages.NotAuthorized, null);
        }

        public static ResponseBaseModel<T> GetUnexpectedErrorResponse(Exception e)
        {
            return new ResponseBaseModel<T>(HttpStatusCode.BadRequest,false, $"{Types.ResponseMessages.UnexpectedError}: {e.Message}", null);
        }

        
    }
}