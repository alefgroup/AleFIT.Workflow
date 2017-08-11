using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    /// <summary>
    /// Defines an object that is able to process a collection of <see cref="IExecutable{T}"/> nodes in specific way.
    /// </summary>
    internal interface IExecutionProcessor<T>
    {
        /// <summary>
        /// Processes <paramref name="executables"/> from the beginning.
        /// </summary>
        /// <param name="context">Context used during execution.</param>
        /// <param name="executables">Actions to be processed.</param>
        /// <returns>Processed context.</returns>
        Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables);

        /// <summary>
        /// Processes <paramref name="executables"/> starting at <paramref name="executionIndex"/>.
        /// </summary>
        /// <param name="context">Context used during execution.</param>
        /// <param name="executables">Actions to be processed.</param>
        /// <param name="executionIndex">Index at which execution will start.</param>
        /// <returns>Processed context.</returns>
        Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables, int executionIndex);
    }
}
