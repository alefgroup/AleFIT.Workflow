using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow
{
    public class WorkflowBuilder<T> : IWorkflowBuilder<T>, IConditionalWorkflowBuilder<T>
    {
        private readonly IList<Func<ExecutionContext<T>, Task>> _workflowNodes = new List<Func<ExecutionContext<T>, Task>>();
        private IFullConditionalNodeBuilder<ExecutionContext<T>> _fullConditionalBuilder;

        private WorkflowBuilder() { }

        public static IWorkflowBuilder<T> Create()
        {
            return new WorkflowBuilder<T>();
        }

        public IWorkflowBuilder<T> Do(IExecutable<ExecutionContext<T>> node)
        {
            return Do(node.ExecuteAsync);
        }

        public IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task> actionToExecute)
        {
            _workflowNodes.Add(actionToExecute);
            return this;
        }

        public IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> condition, Func<ExecutionContext<T>, Task> actionIfFalse)
        {
            return If(condition.ShouldExecuteAsync, condition.ExecuteAsync, actionIfFalse);
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, IWorkflow<ExecutionContext<T>> workflowIfTrue, IWorkflow<ExecutionContext<T>> workflowIfFalse)
        {
            return If(condition, workflowIfTrue.ExecuteAsync, workflowIfFalse.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, Func<ExecutionContext<T>, Task> actionIfTrue, Func<ExecutionContext<T>, Task> actionIfFalse)
        {
            var conditionalNode = ConditionalNodeBuilder<ExecutionContext<T>>
                .Create(condition, actionIfTrue)
                .Else(actionIfFalse);

            _workflowNodes.Add(conditionalNode.ExecuteAsync);
            return this;
        }

        public IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> node)
        {
            return If(node.ShouldExecuteAsync, node.ExecuteAsync);
        }

        public IConditionalWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, Func<ExecutionContext<T>, Task> actionIfTrue)
        {
            _fullConditionalBuilder = ConditionalNodeBuilder<ExecutionContext<T>>.Create(condition, actionIfTrue);
            return this;
        }

        public IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<ExecutionContext<T>> node)
        {
            return ElseIf(node.ShouldExecuteAsync, node.ExecuteAsync);
        }

        public IConditionalWorkflowBuilder<T> ElseIf(Func<ExecutionContext<T>, Task<bool>> condition, Func<ExecutionContext<T>, Task> actionIfTrue)
        {
            _fullConditionalBuilder.ElseIf(condition, actionIfTrue);
            return this;
        }

        public IWorkflowBuilder<T> Else(IExecutable<ExecutionContext<T>> node)
        {
            return Else(node.ExecuteAsync);
        }

        public IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task> actionToExecute)
        {
            _workflowNodes.Add(_fullConditionalBuilder.Else(actionToExecute).ExecuteAsync);
            return this;
        }

        public IWorkflow<T> Build()
        {
            return new Workflow<T>(_workflowNodes);
        }

    }
}
