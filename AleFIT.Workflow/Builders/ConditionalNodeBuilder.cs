using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    internal class ConditionalNodeBuilder<T> : IFullConditionalNodeBuilder<T>
    {
        private readonly Queue<IConditionallyExecutable<T>> _conditionalExecutions = new Queue<IConditionallyExecutable<T>>();
        private Func<T, Task<bool>> _condition;
        
        public ConditionalNodeBuilder(Func<T, Task<bool>> condition)
        {
            _condition = condition;
        }

        private ConditionalNodeBuilder(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            _conditionalExecutions.Enqueue(new ConditionallyExecutable<T>(condition, actionIfTrue));
        }

        public static IFullConditionalNodeBuilder<T> Create(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            return new ConditionalNodeBuilder<T>(condition, actionIfTrue);
        }

        public static IEmptyConditionalNodeBuilder<T> Create(Func<T, Task<bool>> condition)
        {
            return new ConditionalNodeBuilder<T>(condition);
        }
        
        public IFullConditionalNodeBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            _conditionalExecutions.Enqueue(new ConditionallyExecutable<T>(condition, actionIfTrue));
            return this;
        }

        IConditionalWorkflowNode<T> IFullConditionalNodeBuilder<T>.Else(Func<T, Task> actionToExecute)
        {
            return new ConditionalWorkflowNode<T>(_conditionalExecutions, actionToExecute);
        }
    }
}
