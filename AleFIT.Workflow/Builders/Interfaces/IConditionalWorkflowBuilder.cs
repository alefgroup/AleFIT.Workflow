using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IConditionalWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<ExecutionContext<T>> node);

        IConditionalWorkflowBuilder<T> ElseIf(Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IWorkflowBuilder<T> Else(IExecutable<ExecutionContext<T>> node);

        IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionToExecute);
    }
}
