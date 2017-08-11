using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    /// <summary>
    /// Defines an object that can be executed.
    /// </summary>
    /// <typeparam name="T">Context data used in the execution.</typeparam>
    public interface IExecutable<T>
    {
        /// <summary>
        /// Asynchronously executes this object with given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that will be used for this execution.</param>
        /// <returns>Resulting context that went through the execution</returns>
        Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context);
    }
}
