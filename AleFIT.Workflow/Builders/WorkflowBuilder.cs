using System;
using System.Collections;
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
        private readonly List<IExecutable<T>> _nodes = new List<IExecutable<T>>();
        private readonly IExecutionProcessor<T> _sequentialExecutionProcessor = new SequentialExecutionProcessor<T>();
        private readonly IExecutionProcessor<T> _parallelExecutionProcessor = new ParallelExecutionProcessor<T>();
        private readonly IExecutionProcessor<T> _retryExecutionProcessor = new RetrySequentialExecutionProcessor<T>();
        private readonly IInternalWorkflowConfiguration _configuration;

        private IWithIfNodeBuilder<T> _ifElseNodeBuilder;

        private WorkflowBuilder(IInternalWorkflowConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IWorkflowBuilder<T> Create() => new WorkflowBuilder<T>(WorkflowConfiguration.CreateDefault());

        public static IWorkflowBuilder<T> Create(Action<IMutableWorkflowConfiguration> configureAction)
        {
            var configuration = WorkflowConfiguration.CreateDefault();

            configureAction(configuration);
            configuration.IsDefault = false;

            return new WorkflowBuilder<T>(configuration);
        }

        public IWorkflowBuilder<T> Do(IExecutable<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _nodes.Add(action);
            return this;
        }

        public IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return Do(new ExecutableNode<T>(action));
        }

        public IWorkflowBuilder<T> DoWithRetry(int maxRetries, IExecutable<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (maxRetries < 0)
            {
                throw new ArgumentException($"{nameof(maxRetries)} cannot be negative.");
            }

            var retryConfiguration = WorkflowConfiguration.CreateDefault();
            
            // retries would be ignored if we continued on error
            retryConfiguration.ContinueOnError = false;
            retryConfiguration.MaxRetryCount = maxRetries;
            retryConfiguration.IsDefault = false;

            var retryWorkflow = new Workflow<T>(_retryExecutionProcessor, retryConfiguration, Enumerable.Repeat(action, 1));

            _nodes.Add(retryWorkflow);
            return this;
        }

        public IWorkflowBuilder<T> DoWithRetry(int maxRetries, Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            return DoWithRetry(maxRetries, new ExecutableNode<T>(action));
        }

        public IConditionalWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            _ifElseNodeBuilder = IfElseNodeBuilder<T>.CreateUsingProcessor(_sequentialExecutionProcessor)
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

            _nodes.Add(IfElseNodeBuilder<T>
                .CreateUsingProcessor(_sequentialExecutionProcessor)
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

        public IWorkflowBuilder<T> Pause()
        {
            _nodes.Add(new PauseWorkflowNode<T>());
            return this;
        }

        public IWorkflowBuilder<T> DoInParallel(params IExecutable<T>[] actions)
        {
            if (actions == null) throw new ArgumentNullException(nameof(actions));

            return DoInParallel((IEnumerable<IExecutable<T>>)actions);
        }

        public IWorkflowBuilder<T> DoInParallel(IEnumerable<IExecutable<T>> actions)
        {
            if (actions == null) throw new ArgumentNullException(nameof(actions));
            
            _nodes.Add(new Workflow<T>(_parallelExecutionProcessor, (IInternalWorkflowConfiguration)_configuration, actions));
            return this;
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

        public IConditionalWorkflowBuilder<T> ElseIf(Func<ExecutionContext<T>, Task<bool>> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return ElseIf(condition, actionIfTrue.ExecuteAsync);
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

            _nodes.Add(_ifElseNodeBuilder.Else(elseActions));

            return this;
        }

        public IWorkflowBuilder<T> Else(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            return Else(elseActions.Select(action => new ExecutableNode<T>(action)));
        }

        public IWorkflow<T> Build()
        {
            return new Workflow<T>(_sequentialExecutionProcessor, _configuration, _nodes);
        }
    }
}
