using System;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow
{
    public interface IWorkflow<T> : IExecutable<T>
    {
        /// <summary>
        /// Asynchronously executes this workflow with given <paramref name="data"/>
        /// </summary>
        /// <remarks>This method is intended for internal use. Use <see cref="ExecuteAsync"/> instead.</remarks>
        /// <param name="data">The data that will be passed to context.</param>
        /// <returns>Final context that went through the execution</returns>
        Task<ExecutionContext<T>> ExecuteAsync(T data);

        /// <summary>
        /// Asynchronously continues the execution with given <paramref name="c"/>.
        /// </summary>
        /// <remarks>
        /// This method throws <see cref="InvalidOperationException"/> if used on non-paused context.
        /// </remarks>
        /// <param name="context">The context that will be used for continued execution.</param>
        /// <returns>Final context that went through the execution</returns>
        Task<ExecutionContext<T>> ContinueAsync(ExecutionContext<T> context);
    }
}
