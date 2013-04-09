﻿using System;
using System.Collections.Generic;

namespace Enflow.Base
{
    public interface IWorkflowFactory
    {
        IWorkflow<T> Get<T>(string name) where T : IModel<T>;
    }

    public abstract class WorkflowFactory : IWorkflowFactory
    {
        private readonly Dictionary<string, Func<object>> _registrations = new Dictionary<string, Func<object>>();

        protected void Register<T>(string name, Func<IWorkflow<T>> resolver) where T : IModel<T>
        {
            Func<object> wrapper = resolver; // Required for support of some PCL targets.
            _registrations.Add(name, wrapper);
        }

        public IWorkflow<T> Get<T>(string name) where T : IModel<T>
        {
            try
            {
                return (IWorkflow<T>) _registrations[name].Invoke();
            }
            catch (KeyNotFoundException) { throw new WorkflowFactoryException("Unable to resolve workflow with name: " + name); }
            catch (InvalidCastException) { throw new WorkflowFactoryException("Wrong generic argument supplied for workflow with name: " + name); }
        }
    }
}
