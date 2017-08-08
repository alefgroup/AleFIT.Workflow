using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Nodes
{
    internal class PauseWorkflowNode<T> : IExecutable<T>
    {
        private readonly TaskCompletionSource<ExecutionContext<T>> _completionSource =
            new TaskCompletionSource<ExecutionContext<T>>();

        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            context.SetPaused(_completionSource);
            return Task.FromResult(context);
        }
    }
}
