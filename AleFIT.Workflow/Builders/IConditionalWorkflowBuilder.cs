using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Builders
{
    public interface IConditionalWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<ExecutionContext<T>> node);
        IConditionalWorkflowBuilder<T> ElseIf(Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task> actionIfTrue);
        IWorkflowBuilder<T> Else(IExecutable<ExecutionContext<T>> node);
        IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task> actionToExecute);
    }
}
