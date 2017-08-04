using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    public interface IWorkflowBuilder<T>
    {
        IWorkflowBuilder<T> Do(IExecutable<ExecutionContext<T>> node);
        IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task> actionToExecute);


        IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> node);
        IConditionalWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, Func<ExecutionContext<T>, Task> actionIfTrue);
        IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> condition, Func<ExecutionContext<T>, Task> actionIfFalse);
        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task> actionIfTrue, 
            Func<ExecutionContext<T>, Task> actionIfFalse);
        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            IWorkflow<ExecutionContext<T>> workflowIfTrue, 
            IWorkflow<ExecutionContext<T>> workflowIfFalse);
        
        IWorkflow<T> Build();
    }
}