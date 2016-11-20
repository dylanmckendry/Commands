using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slipstream.CommonDotNet.Commands.Results
{
    public interface IResult
    {
    }

    public class Result<TResult> : IResult
    {
        public TResult Value { get; }

        private Result(TResult value)
        {
            Value = value;
        }

        public static Result<TValue> From<TValue>(TValue value)
        {
            return new Result<TValue>(value);
        }

        public static Unit Empty => Unit.Value;
    }
}
