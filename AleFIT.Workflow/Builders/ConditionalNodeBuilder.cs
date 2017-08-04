using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders.Interfaces;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    internal class ConditionalNodeBuilder<T> : IFullConditionalNodeBuilder<T>
    {
        private readonly Queue<IConditionallyExecutable<T>> _conditionalExecutions = new Queue<IConditionallyExecutable<T>>();

        private ConditionalNodeBuilder(Func<T, Task<bool>> condition, Func<T, Task<T>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            _conditionalExecutions.Enqueue(new ConditionallyExecutable<T>(condition, actionIfTrue));
        }

        public static IFullConditionalNodeBuilder<T> Create(Func<T, Task<bool>> condition, Func<T, Task<T>> actionIfTrue)
        {
            return new ConditionalNodeBuilder<T>(condition, actionIfTrue);
        }

        public IFullConditionalNodeBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task<T>> actionIfTrue)
        {
            _conditionalExecutions.Enqueue(new ConditionallyExecutable<T>(condition, actionIfTrue));
            return this;
        }

        IConditionalWorkflowNode<T> IFullConditionalNodeBuilder<T>.Else(Func<T, Task<T>> actionToExecute)
        {
            return new ConditionalWorkflowNode<T>(_conditionalExecutions, actionToExecute);
        }
    }
}
