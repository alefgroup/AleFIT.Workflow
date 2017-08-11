using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders.Interfaces;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Core.Helpers
{
    public static class WorkflowBuilderExtensions
    {
        public static IWorkflowBuilder<T> Do<T>(this IWorkflowBuilder<T> builder, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return builder.Do(new ExecutableNode<T>(action));
        }

        public static IWorkflowBuilder<T> DoWithRetry<T>(this IWorkflowBuilder<T> builder,
            int maxRetries, Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            return builder.DoWithRetry(maxRetries, new ExecutableNode<T>(action));
        }

        public static IConditionalWorkflowBuilder<T> If<T>(
            this IWorkflowBuilder<T> builder, 
            IConditional<T> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            return builder.If(condition, new ExecutableNode<T>(actionIfTrue));
        }

        public static IWorkflowBuilder<T> If<T>(
            this IWorkflowBuilder<T> builder,
            Func<ExecutionContext<T>, Task<bool>> condition, 
            IExecutable<T> actionIfTrue, 
            IExecutable<T> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return builder.If(new ConditionalNode<T>(condition), actionIfTrue, actionIfFalse);
        }

        public static IConditionalWorkflowBuilder<T> If<T>(
            this IWorkflowBuilder<T> builder,
            Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return builder.If(new ConditionalNode<T>(condition), new ExecutableNode<T>(actionIfTrue));
        }

        public static IConditionalWorkflowBuilder<T> If<T>(
            this IWorkflowBuilder<T> builder,
            Func<ExecutionContext<T>, 
            Task<bool>> condition, IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return builder.If(new ConditionalNode<T>(condition), actionIfTrue);
        }

        public static IWorkflowBuilder<T> DoInParallel<T>(this IWorkflowBuilder<T> builder, params IExecutable<T>[] actions)
        {
            if (actions == null) throw new ArgumentNullException(nameof(actions));

            return builder.DoInParallel((IEnumerable<IExecutable<T>>)actions);
        }

        public static IConditionalWorkflowBuilder<T> ElseIf<T>(
            this IConditionalWorkflowBuilder<T> builder,
            IConditional<T> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return builder.ElseIf(condition, new ExecutableNode<T>(actionIfTrue));
        }

        public static IConditionalWorkflowBuilder<T> ElseIf<T>(
            this IConditionalWorkflowBuilder<T> builder,
            Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return builder.ElseIf(new ConditionalNode<T>(condition), new ExecutableNode<T>(actionIfTrue));
        }

        public static IConditionalWorkflowBuilder<T> ElseIf<T>(
            this IConditionalWorkflowBuilder<T> builder, 
            Func<ExecutionContext<T>, Task<bool>> condition, 
            IExecutable<T> actionIfTrue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));

            return builder.ElseIf(condition, actionIfTrue.ExecuteAsync);
        }

        public static IWorkflowBuilder<T> Else<T>(this IConditionalWorkflowBuilder<T> builder, IExecutable<T> elseAction)
        {
            if (elseAction == null) throw new ArgumentNullException(nameof(elseAction));

            return builder.Else(Enumerable.Repeat(elseAction, 1));
        }

        public static IWorkflowBuilder<T> Else<T>(
            this IConditionalWorkflowBuilder<T> builder, 
            IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions)
        {
            if (elseActions == null) throw new ArgumentNullException(nameof(elseActions));

            return builder.Else(elseActions.Select(action => new ExecutableNode<T>(action)));
        }

        public static IWorkflowBuilder<T> Else<T>(
            this IConditionalWorkflowBuilder<T> builder, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> elseAction)
        {
            if (elseAction == null) throw new ArgumentNullException(nameof(elseAction));

            return builder.Else(new ExecutableNode<T>(elseAction));
        }
    }
}
