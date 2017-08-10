using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.Mocks
{
    internal class WaitNode<T> : IExecutable<T>
    {
        private readonly TimeSpan _waitTime;

        public WaitNode(TimeSpan waitTime)
        {
            _waitTime = waitTime;
        }

        public async Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            await Task.Delay(_waitTime);
            return context;
        }
    }
}
