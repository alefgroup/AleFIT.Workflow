using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IConditionalWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue);

        IConditionalWorkflowBuilder<T> ElseIf(
            IConditional<T> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IConditionalWorkflowBuilder<T> ElseIf(
            Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IConditionalWorkflowBuilder<T> ElseIf(
            Func<ExecutionContext<T>, Task<bool>> condition,
            IExecutable<T> actionIfTrue);

        IWorkflowBuilder<T> Else(IExecutable<T> elseAction);

        IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> elseAction);

        IWorkflowBuilder<T> Else(IEnumerable<IExecutable<T>> elseActions);

        IWorkflowBuilder<T> Else(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions);
    }
}
