﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Slipstream.CommonDotNet.Commands.Results;

namespace Slipstream.CommonDotNet.Commands
{
    public class ResultRegisterProcessor<TCommand, TSuccessResult, TReturn>
        where TCommand : IAsyncCommand
        where TSuccessResult : IResult
    {

        private readonly ISuccessResult<TCommand, TSuccessResult> command;

        private readonly IReadOnlyDictionary<Type, Func<IResult, TReturn>> defualtResultParsers;
        private readonly Dictionary<Type, Func<IResult, TReturn>> resultParsers = new Dictionary<Type, Func<IResult, TReturn>>();

        private readonly ILifetimeScopeService lifetimeScopeService;

        public ResultRegisterProcessor(ISuccessResult<TCommand, TSuccessResult> command, IReadOnlyDictionary<Type, Func<IResult, TReturn>> defualtResultParsers, ILifetimeScopeService lifetimeScopeService)
        {
            Contract.Requires(command != null);
            Contract.Requires(defualtResultParsers != null);
            Contract.Requires(lifetimeScopeService != null);

            this.command = command;
            this.defualtResultParsers = defualtResultParsers;
            this.lifetimeScopeService = lifetimeScopeService;
        }


        public ResultParser<TCommand, TSuccessResult, TReturn, TWhen> When<TWhen>(Func<TCommand, TWhen> action)
            where TWhen : IResult
        {
            return new ResultParser<TCommand, TSuccessResult, TReturn, TWhen>(this, func =>
            {
                resultParsers.Add(typeof(TWhen), func);
            });
        }

        // TODO: could add in Catch<TException>.Return(e => e...)
        //public ResultParser<TCommand, TSuccessResult, TReturn, TE> Catch<TWhen>(Func<TCommand, TWhen> action)
        //    where TWhen : Exception
        //{
        //    return new ResultParser<TCommand, TSuccessResult, TReturn, TWhen>(this, func =>
        //    {
        //        resultParsers.Add(typeof(TWhen), func);
        //    });
        //}

        // TODO: this should be done in another class
        public async Task<TReturn> ExecuteAsync()
        {
            using (var processor = new CommandProcessor(lifetimeScopeService))
            {
                var result = await processor.ProcessAsync(command);

                if (resultParsers.ContainsKey(result.GetType()))
                {
                    return resultParsers[result.GetType()](result);
                }
                else if (defualtResultParsers.ContainsKey(result.GetType()))
                {
                    return defualtResultParsers[result.GetType()](result);
                }
                else
                {
                    // TODO: not registered exception
                    throw new ResultNotRegisteredException(command.GetType(), result.GetType());
                }
            }
        }

        public Task<TSuccessResult> ExecuteSuccessAsync()
        {
            throw new NotImplementedException();
        }
    }
}
