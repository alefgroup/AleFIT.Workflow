using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    /// <summary>
    /// Defines an object that hold certain condition that can be evaluated.
    /// </summary>
    /// <typeparam name="T">Data used in execution context</typeparam>
    public interface IConditional<T>
    {
        /// <summary>
        /// Asynchronously evaluates the condition.
        /// </summary>
        /// <param name="context">Context used for this execution.</param>
        /// <returns>Result of the evaluation.</returns>
        Task<bool> EvaluateAsync(ExecutionContext<T> context);
    }
}
