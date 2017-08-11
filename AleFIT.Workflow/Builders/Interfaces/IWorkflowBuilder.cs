using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IWorkflowBuilder<T>
    {
        IWorkflowBuilder<T> Do(IExecutable<T> action);

        IWorkflowBuilder<T> DoWithRetry(int maxRetries, IExecutable<T> action);

        IConditionalWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue);

        IWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse);
        
        IWorkflowBuilder<T> Pause();

        IWorkflowBuilder<T> DoInParallel(IEnumerable<IExecutable<T>> actions);

        IWorkflow<T> Build();
    }
}
