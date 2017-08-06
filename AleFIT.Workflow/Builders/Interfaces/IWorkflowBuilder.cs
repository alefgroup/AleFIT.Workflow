﻿using System;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IWorkflowBuilder<T>
    {
        IWorkflowBuilder<T> Do(IExecutable<T> action);

        IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action);

        IConditionalWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue);

        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse);

        IWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse);

        IConditionalWorkflowBuilder<T> If(
            IConditional<T> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IConditionalWorkflowBuilder<T> If(
            Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IConditionalWorkflowBuilder<T> If(
            Func<ExecutionContext<T>, Task<bool>> condition,
            IExecutable<T> actionIfTrue);

        IWorkflow<T> Build();
    }
}
