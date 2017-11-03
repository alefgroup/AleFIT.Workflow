using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IIfWithoutThenWorkflowBuilder<T>
    {
        /// <summary>
        /// Adds an action that will be executed when previously setup condition is true.
        /// </summary>
        /// <param name="actionIfTrue">The action to be executed.</param>
        IConditionalWorkflowBuilder<T> Then(IExecutable<T> actionIfTrue);
    }
}
