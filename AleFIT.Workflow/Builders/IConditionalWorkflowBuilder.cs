using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Builders
{
    public interface IConditionalWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<T> node);
        IConditionalWorkflowBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue);
        IWorkflowBuilder<T> Else(IExecutable<T> node);
        IWorkflowBuilder<T> Else(Func<T, Task> actionToExecute);
    }
}
