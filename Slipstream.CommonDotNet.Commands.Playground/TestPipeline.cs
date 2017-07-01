﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slipstream.CommonDotNet.Commands.Pipelines;

namespace Slipstream.CommonDotNet.Commands.Playground
{
    public class TestPipeline : Pipeline
    {
        private readonly BloggingContext context;

        public TestPipeline(BloggingContext context)
        {
            Console.WriteLine("TestPipeline was created");
            this.context = context;
        }

        public override Task ExecutingAsync(IAsyncCommand command)
        {
            Console.WriteLine("Incoming with context: " + context.Id);
            return base.ExecutingAsync(command);
        }

        public override Task ExecutedAsync(IAsyncCommand command, object result)
        {
            Console.WriteLine("Outgoing with context: " + context.Id);
            return base.ExecutedAsync(command, result);
        }

    }

    public class SecondTestPipeline : Pipeline
    {
        private readonly BloggingContext context;

        public SecondTestPipeline(BloggingContext context)
        {
            Console.WriteLine("SecondTestPipeline was created");
            this.context = context;
        }

        public override Task ExecutingAsync(IAsyncCommand command)
        {
            Console.WriteLine("Incoming (Second) with command: " + command.GetType().Name);
            return base.ExecutingAsync(command);
        }

        public override Task ExecutedAsync(IAsyncCommand command, object result)
        {
            Console.WriteLine("Outgoing (Second) with command: " + command.GetType().Name + " and result: " + result.GetType().Name);
            return base.ExecutedAsync(command, result);
        }

        public override Task ExceptionAsync(IAsyncCommand command, Exception exception)
        {
            Console.WriteLine("Exception (Second) with command: " + command.GetType().Name + " and exception: " + exception.GetType().Name);
            return base.ExceptionAsync(command, exception);
        }
    }
}
