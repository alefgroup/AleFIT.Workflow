using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders.Interfaces;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Executors;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    internal class IfElseNodeBuilder<T> : IWithProcessorIfNodeBuilder<T>, IWithIfThenNodeBuilder<T>, IWithIfNodeBuilder<T>
    {
        private readonly IExecutionProcessor<T> _executionProcessor;
        private readonly Queue<IConditionallyExecutable<T>> _conditionalActions = new Queue<IConditionallyExecutable<T>>();

        private IConditional<T> _lastCondition;

        private IfElseNodeBuilder(IExecutionProcessor<T> executionProcessor)
        {
            _executionProcessor = executionProcessor;
        }

        public static IWithProcessorIfNodeBuilder<T> CreateUsingProcessor(IExecutionProcessor<T> processor)
        {
            if (processor == null) throw new ArgumentNullException(nameof(processor));
            
            return new IfElseNodeBuilder<T>(processor);
        }

        public IWithIfThenNodeBuilder<T> WithIfThen(IConditional<T> condition, IEnumerable<IExecutable<T>> actionsIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionsIfTrue == null) throw new ArgumentNullException(nameof(actionsIfTrue));
            
            _conditionalActions.Enqueue(new ConditionallyExecutableNode<T>(condition, actionsIfTrue, _executionProcessor));

            return this;
        }

        public IWithIfNodeBuilder<T> WithIf(IConditional<T> condition)
        {
            _lastCondition = condition ?? throw new ArgumentNullException(nameof(condition));
            return this;
        }


        public IWithIfThenNodeBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue)
        {
            _conditionalActions.Enqueue(
                new ConditionallyExecutableNode<T>(condition, Enumerable.Repeat(actionIfTrue, 1), _executionProcessor));

            return this;
        }

        public IWithIfThenNodeBuilder<T> ElseIf(
            IConditional<T> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            return ElseIf(condition, new ExecutableNode<T>(actionIfTrue));
        }

        public IWithIfThenNodeBuilder<T> ElseIf(
            Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            return ElseIf(new ConditionalNode<T>(condition), new ExecutableNode<T>(actionIfTrue));
        }

        public IWithIfNodeBuilder<T> ElseIf(IConditional<T> condition)
        {
            _lastCondition = condition ?? throw new ArgumentNullException(nameof(condition));
            return this;
        }

        public IWithIfThenNodeBuilder<T> Then(IExecutable<T> actionIfTrue)
        {
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return ElseIf(_lastCondition, actionIfTrue);
        }

        public IfElseWorkflowNode<T> Else(IEnumerable<IExecutable<T>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            return new IfElseWorkflowNode<T>(_conditionalActions, elseActions, _executionProcessor);
        }

        public IfElseWorkflowNode<T> Else(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            return Else(elseActions.Select(action => new ExecutableNode<T>(action)));
        }
    }
}
