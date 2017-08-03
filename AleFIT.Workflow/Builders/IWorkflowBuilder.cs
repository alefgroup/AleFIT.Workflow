using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    public interface IWorkflowBuilder<T>
    {
        IWorkflowBuilder<T> Do(IExecutable<T> node);
        IWorkflowBuilder<T> Do(Func<T, Task> actionToExecute);


        IConditionalWorkflowBuilder<T> If(Func<T, Task<bool>> condition);
        IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<T> node);
        IConditionalWorkflowBuilder<T> If(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue);
        IWorkflowBuilder<T> If(IConditionallyExecutable<T> condition, Func<T, Task> actionIfFalse);
        IWorkflowBuilder<T> If(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue, Func<T, Task> actionIfFalse);
        IWorkflowBuilder<T> If(Func<T, Task<bool>> condition, IWorkflow<T> workflowIfTrue, IWorkflow<T> workflowIfFalse);
        
        IWorkflow<T> Build();
    }
}