﻿using System;

namespace DreamNucleus.Commands
{
    public class CommandProcessorSuccessResult<TSuccessResult>
    {
        public bool Success { get; }

        private readonly TSuccessResult _result;
        public TSuccessResult Result
        {
            get
            {
                if (Success)
                {
                    return _result;
                }
                else
                {
                    // TODO: what exception to use here?
                    throw new NotSupportedException();
                }
            }
        }

        private readonly Exception _exception;
        public Exception Exception
        {
            get
            {
                if (!Success)
                {
                    return _exception;
                }
                else
                {
                    // TODO: what exception to use here?
                    throw new NotSupportedException();
                }
            }
        }

        public CommandProcessorSuccessResult(TSuccessResult result)
        {
            _result = result;
            Success = true;
        }

        public CommandProcessorSuccessResult(Exception exception)
        {
            _exception = exception;
            Success = false;
        }
    }
}
