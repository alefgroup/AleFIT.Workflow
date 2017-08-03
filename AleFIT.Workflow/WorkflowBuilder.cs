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
        private readonly IList<Func<T, Task>> _workflowNodes = new List<Func<T, Task>>();
        private IFullConditionalNodeBuilder<T> _fullConditionalBuilder;
        private IEmptyConditionalNodeBuilder<T> _emptyConditionalBuilder;

        private WorkflowBuilder() { }

        public static IWorkflowBuilder<T> Create()
        {
            return new WorkflowBuilder<T>();
        }

        public IWorkflowBuilder<T> Do(IExecutable<T> node)
        {
            _workflowNodes.Add(node.ExecuteAsync);
            return this;
        }

        public IWorkflowBuilder<T> Do(Func<T, Task> actionToExecute)
        {
            _workflowNodes.Add(actionToExecute);
            return this;
        }

        public IConditionalWorkflowBuilder<T> If(Func<T, Task<bool>> condition)
        {
            _emptyConditionalBuilder = ConditionalNodeBuilder<T>.Create(condition);
            return this;
        }

        public IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<T> node)
        {
            _fullConditionalBuilder = ConditionalNodeBuilder<T>.Create(node.ShouldExecuteAsync, node.ExecuteAsync);
            return this;
        }

        public IConditionalWorkflowBuilder<T> If(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            _fullConditionalBuilder = ConditionalNodeBuilder<T>.Create(condition, actionIfTrue);
            return this;
        }

        public IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<T> node)
        {
            return ElseIf(node.ShouldExecuteAsync, node.ExecuteAsync);
        }

        public IConditionalWorkflowBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            _fullConditionalBuilder.ElseIf(condition, actionIfTrue);
            return this;
        }

        public IWorkflowBuilder<T> Else(IExecutable<T> node)
        {
            return Else(node.ExecuteAsync);
        }

        public IWorkflowBuilder<T> Else(Func<T, Task> actionToExecute)
        {
            _workflowNodes.Add(_fullConditionalBuilder.Else(actionToExecute).ExecuteAsync);
            return this;
        }

        public IWorkflowBuilder<T> If(IConditionallyExecutable<T> condition, Func<T, Task> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return If(condition.ShouldExecuteAsync, condition.ExecuteAsync, actionIfFalse);
        }

        public IWorkflowBuilder<T> If(Func<T, Task<bool>> condition, IWorkflow<T> workflowIfTrue, IWorkflow<T> workflowIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (workflowIfTrue == null) throw new ArgumentNullException(nameof(workflowIfTrue));
            if (workflowIfFalse == null) throw new ArgumentNullException(nameof(workflowIfFalse));

            return If(condition, workflowIfTrue.ExecuteAsync, workflowIfFalse.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue, Func<T, Task> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            _workflowNodes.Add(ConditionalNodeBuilder<T>.Create(condition, actionIfTrue).Else(actionIfFalse).ExecuteAsync);
            return this;
        }

        public IWorkflow<T> Build()
        {
            return new Workflow<T>(_workflowNodes);
        }
    }
}
