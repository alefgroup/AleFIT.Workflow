using System.Collections.Generic;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IConditionalWorkflowBuilder<T>
    {
        /// <summary>
        /// Adds another condition branch to the IF action.
        /// ElseIf actions will be evaluated in the same order in which they were defined.
        /// </summary>
        /// <param name="condition">Condition to be evaluated.</param>
        /// <param name="actionIfTrue">Action to be executed if <paramref name="condition"/> is true.</param>
        IConditionalWorkflowBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue);

        /// <summary>
        /// Adds another condition branch to the IF action.
        /// ElseIf actions will be evaluated in the same order in which they were defined.
        /// </summary>
        /// <param name="condition">Condition to be evaluated.</param>
        IIfWithoutThenWorkflowBuilder<T> ElseIf(IConditional<T> condition);

        /// <summary>
        /// Completes IF action definition by adding an Else node that will be executed if all previous conditions return false.
        /// </summary>
        /// <param name="elseActions">Actions to executed.</param>
        IWorkflowBuilder<T> Else(IEnumerable<IExecutable<T>> elseActions);
    }
}
