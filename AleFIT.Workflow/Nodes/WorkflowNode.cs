using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Nodes
{
    internal class WorkflowNode<T> : IExecutable<T>
    {
        private readonly Func<T, Task> _executeAsync;

        public WorkflowNode(Func<T, Task> executeAsync)
        {
            _executeAsync = executeAsync;
        }

        public async Task ExecuteAsync(T data) => await _executeAsync(data).ConfigureAwait(false);
    }
}
