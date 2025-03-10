﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.Extentions
{
    public class Result
    {
        public Result()
        {
        }

        public List<string> Messages { get; set; } = new List<string>();

        public bool Succeeded { get; set; }

        public static Result Fail()
        {
            return new Result { Succeeded = false };
        }

        public static Result Fail(Exception ex,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = null)
        {
            return new Result { Succeeded = false, Messages = new List<string> { ex.Message } };
        }

        public static Result Fail(string message)
        {
            return new Result { Succeeded = false, Messages = new List<string> { message } };
        }

        public static Result Fail(List<string> messages)
        {
            return new Result { Succeeded = false, Messages = messages };
        }

        public static Task<Result> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<Result> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<Result> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public static Result Success()
        {
            return new Result { Succeeded = true };
        }

        public static Result Success(string message)
        {
            return new Result { Succeeded = true, Messages = new List<string> { message } };
        }

        public static Task<Result> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<Result> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
    }

    public class Result<T> : Result
    {
        public Result()
        {
        }

        public T Data { get; set; }

        public new static Result<T> Fail()
        {
            return new Result<T> { Succeeded = false };
        }

        public new static Result<T> Fail(Exception ex,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = null)
        {
            return new Result<T> { Succeeded = false, Messages = new List<string> { ex.Message } };
        }

        public new static Result<T> Fail(string message)
        {
            return new Result<T> { Succeeded = false, Messages = new List<string> { message } };
        }

        public new static Result<T> Fail(List<string> messages)
        {
            return new Result<T> { Succeeded = false, Messages = messages };
        }

        public new static Task<Result<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static Task<Result<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<Result<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public new static Result<T> Success(DTOs.ReturnOrderDto? returnOrderDto)
        {
            return new Result<T> { Succeeded = true };
        }

        public new static Result<T> Success(string message)
        {
            return new Result<T> { Succeeded = true, Messages = new List<string> { message } };
        }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
        }

        public static Result<T> Success(T data, List<string> messages)
        {
            return new Result<T> { Succeeded = true, Data = data, Messages = messages };
        }

        public new static Task<Result<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<Result<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<Result<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }
    }
}