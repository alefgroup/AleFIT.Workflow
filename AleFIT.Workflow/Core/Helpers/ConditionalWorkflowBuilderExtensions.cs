using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders.Interfaces;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Core.Helpers
{
    public static class ConditionalWorkflowBuilderExtensions
    {
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
    }
}
