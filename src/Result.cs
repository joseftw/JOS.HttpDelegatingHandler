using System;
using System.Collections.Generic;
using System.Linq;

namespace JOS.HttpDelegatingHandler
{
    public class Result
    {
        protected Result(bool success, string message = default, Exception exception = default, IEnumerable<Error> errors = default)
        {
            Exception = exception;
            Success = success;
            Message = message;
            Errors = errors ?? Enumerable.Empty<Error>();
        }

        public bool Success { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public IEnumerable<Error> Errors { get; }
        public static Result Ok(string message = default) => new Result(true, message);
        public static Result Fail(string message = default, Exception exception = default, IEnumerable<Error> errors = default) => new Result(false, message, exception, errors);
    }

    public class Result<T> : Result
    {
        protected Result(bool success, T data, string message, Exception exception, IEnumerable<Error> errors) : base(success, message, exception, errors)
        {
            Data = data;
        }

        public T Data { get; }

        public static Result<T> Ok(T data, string message = default)
        {
            return new Result<T>(true, data, message, default, default);
        }

        public new static Result<T> Fail(string message, Exception exception = default, IEnumerable<Error> errors = default)
        {
            return new Result<T>(false, default(T), message, exception, errors);
        }
    }
}

public class Error
{
    public Error(string message, ErrorType type) : this(null, message, type)
    {

    }

    public Error(string errorCode, string message, ErrorType errorType)
    {
        ErrorCode = errorCode;
        Message = message;
        Type = errorType;
    }
    public string ErrorCode { get; }
    public string Message { get; }
    public ErrorType Type { get; }
}

public enum ErrorType
{
    Undefined,
    Register,
    NotFound,
    Unathorized
}
