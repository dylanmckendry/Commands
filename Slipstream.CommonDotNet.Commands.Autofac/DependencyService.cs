﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Slipstream.CommonDotNet.Commands.Autofac
{
    public class DependencyService : ILifetimeScopeDependencyService
    {
        private readonly ILifetimeScope lifetimeScope;

        public DependencyService(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public bool IsRegistered<T>()
        {
            return lifetimeScope.IsRegistered<T>();
        }

        public bool IsRegistered(Type type)
        {
            return lifetimeScope.IsRegistered(type);
        }

        public T Resolve<T>()
        {
            return lifetimeScope.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return lifetimeScope.Resolve(type);
        }

        public void Dispose()
        {
            lifetimeScope.Dispose();
        }
    }

    public class LifetimeScopeService : ILifetimeScopeService
    {
        private readonly ILifetimeScope lifetimeScope;

        public LifetimeScopeService(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public ILifetimeScopeDependencyService BeginLifetimeScope()
        {
            return new DependencyService(lifetimeScope.BeginLifetimeScope());
        }

        public ILifetimeScopeDependencyService BeginLifetimeScope(ICommandProcessor commandProcessor)
        {
            return new DependencyService(lifetimeScope.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(commandProcessor).As<ICommandProcessor>().ExternallyOwned();
            }));
        }
    }
}
