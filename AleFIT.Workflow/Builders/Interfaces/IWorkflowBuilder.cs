using System.Collections.Generic;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IWorkflowBuilder<T>
    {
        /// <summary>
        /// Adds a sequential action that will be executed in the workflow.
        /// </summary>
        /// <param name="action">The action that is going to be executed.</param>
        IWorkflowBuilder<T> Do(IExecutable<T> action);

        /// <summary>
        /// Adds a sequential action that will be executed in the workflow.
        /// If this action fails, it will be executed again with a maximum of <paramref name="maxRetries"/> retries.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries used if action fails.</param>
        /// <param name="action">The action that is going to be executed.</param>
        IWorkflowBuilder<T> DoWithRetry(int maxRetries, IExecutable<T> action);

        /// <summary>
        /// Adds a conditional IF action to the workflow.
        /// </summary>
        /// <param name="condition">The condition that will be evaluated.</param>
        /// <param name="actionIfTrue">Action that will be executed if <paramref name="condition"/> is true.</param>
        IConditionalWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue);

        /// <summary>
        /// Adds a complete IF action to the workflow.
        /// </summary>
        /// <param name="condition">The condition that will be evaluated.</param>
        /// <param name="actionIfTrue">Action that will be executed if <paramref name="condition"/> is true.</param>
        /// <param name="actionIfFalse">Action that will be executed if <paramref name="condition"/> is false</param>
        IWorkflowBuilder<T> If(IConditional<T> condition, IExecutable<T> actionIfTrue, IExecutable<T> actionIfFalse);

        /// <summary>
        /// Adds a conditional IF action to the workflow. Rest of the IF action has to be specified next in the builder.
        /// </summary>
        /// <param name="condition">The condition that will be evaluated.</param>
        IIfWithoutThenWorkflowBuilder<T> If(IConditional<T> condition);

        /// <summary>
        /// Adds a pause node to the workflow. 
        /// When an execution reaches this action it will return a paused context that can be continued later.
        /// </summary>
        IWorkflowBuilder<T> Pause();

        /// <summary>
        /// Adds a collection of actions that will be executed in parallel.
        /// Execution continues when all actions in <paramref name="actions"/> finishes.
        /// </summary>
        /// <remarks>
        /// You CANNOT use pause node with this action, because it can lead to inconsistencies.
        /// </remarks>
        /// <param name="actions">Collection of actions that will be executed in parallel.</param>
        IWorkflowBuilder<T> DoInParallel(IEnumerable<IExecutable<T>> actions);

        /// <summary>
        /// Creates the final workflow using previously configured actions.
        /// </summary>
        /// <returns>Fully configured workflow</returns>
        IWorkflow<T> Build();
    }
}
