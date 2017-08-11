using System;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Nodes
{
    /// <summary>
    /// This executable node just hold the action that will be executed.
    /// </summary>
    internal class ExecutableNode<T> : IExecutable<T>
    {
        private readonly Func<ExecutionContext<T>, Task<ExecutionContext<T>>> _action;

        public ExecutableNode(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> action)
        {
            _action = action;
        }

        public async Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context) => await _action(context).ConfigureAwait(false);
    }
}
