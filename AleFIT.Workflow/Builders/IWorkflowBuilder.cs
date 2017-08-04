using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    public interface IWorkflowBuilder<T>
    {
        IWorkflowBuilder<T> Do(IExecutable<ExecutionContext<T>> node);

        IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionToExecute);
        
        IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> node);

        IConditionalWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfFalse);

        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfFalse);

        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition,
            IExecutable<ExecutionContext<T>> actionIfTrue,
            IExecutable<ExecutionContext<T>> actionIfFalse);

        IWorkflowBuilder<T> If(ICondition<ExecutionContext<T>> condition,
            IExecutable<ExecutionContext<T>> actionIfTrue,
            IExecutable<ExecutionContext<T>> actionIfFalse);

        IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> trueBranch,
            IExecutable<ExecutionContext<T>> elseBranch);

        IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            IWorkflow<T> workflowIfTrue,
            IWorkflow<T> workflowIfFalse);
        
        IWorkflow<T> Build();
    }
}