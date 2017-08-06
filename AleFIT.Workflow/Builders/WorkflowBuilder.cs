using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders.Interfaces;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Configuration;
using AleFIT.Workflow.Executors;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    public class WorkflowBuilder<T> : IWorkflowBuilder<T>, IConditionalWorkflowBuilder<T>
    {
        private readonly Queue<IExecutable<T>> _nodes = new Queue<IExecutable<T>>();
        private readonly IExecutionProcessor<T> _executionProcessor = new SequentialExecutionProcessor<T>();
        private readonly IWorkflowConfiguration _configuration;

        private IWithIfNodeBuilder<T> _ifElseNodeBuilder;

        private WorkflowBuilder(IWorkflowConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IWorkflowBuilder<T> Create() => new WorkflowBuilder<T>(WorkflowConfiguration.CreateDefault());

        public static IWorkflowBuilder<T> Create(Action<IMutableWorkflowConfiguration> configureAction)
        {
            var configuration = WorkflowConfiguration.CreateDefault();

            configureAction(configuration);

            return new WorkflowBuilder<T>(configuration);
        }

        public IWorkflowBuilder<T> Do(IExecutable<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _nodes.Enqueue(action);
            return this;
        }

        public IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return Do(new ExecutableNode<T>(action));
        }

        public IConditionalWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            _ifElseNodeBuilder = IfElseNodeBuilder<T>.CreateUsingProcessor(_executionProcessor)
                .WithIf(condition, Enumerable.Repeat(actionIfTrue, 1));

            return this;
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return If(new ConditionalNode<T>(condition), actionIfTrue, actionIfFalse);
        }

        public IWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            _nodes.Enqueue(IfElseNodeBuilder<T>
                .CreateUsingProcessor(_executionProcessor)
                .WithIf(condition, Enumerable.Repeat(actionIfTrue, 1))
                .Else(Enumerable.Repeat(actionIfFalse, 1)));

            return this;
        }

        public IConditionalWorkflowBuilder<T> If(IConditional<T> condition, Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            return If(condition, new ExecutableNode<T>(actionIfTrue));
        }

        public IConditionalWorkflowBuilder<T> If(
            Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return If(new ConditionalNode<T>(condition), new ExecutableNode<T>(actionIfTrue));
        }

        public IConditionalWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return If(new ConditionalNode<T>(condition), actionIfTrue);
        }

        public IConditionalWorkflowBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            _ifElseNodeBuilder.ElseIf(condition, actionIfTrue);

            return this;
        }

        public IConditionalWorkflowBuilder<T> ElseIf(IConditional<T> condition, Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return ElseIf(condition, new ExecutableNode<T>(actionIfTrue));
        }

        public IConditionalWorkflowBuilder<T> ElseIf(
            Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return ElseIf(new ConditionalNode<T>(condition), new ExecutableNode<T>(actionIfTrue));
        }

        public IWorkflowBuilder<T> Else(IExecutable<T> elseAction)
        {
            if (elseAction == null) throw new ArgumentNullException(nameof(elseAction));

            return Else(Enumerable.Repeat(elseAction, 1));
        }

        public IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> elseAction)
        {
            if (elseAction == null) throw new ArgumentNullException(nameof(elseAction));

            return Else(new ExecutableNode<T>(elseAction));
        }

        public IWorkflowBuilder<T> Else(IEnumerable<IExecutable<T>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            _nodes.Enqueue(_ifElseNodeBuilder.Else(elseActions));

            return this;
        }

        public IWorkflowBuilder<T> Else(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            return Else(elseActions.Select(action => new ExecutableNode<T>(action)));
        }

        public IWorkflow<T> Build()
        {
            return new Workflow<T>(_executionProcessor, _configuration, _nodes);
        }
    }
}
