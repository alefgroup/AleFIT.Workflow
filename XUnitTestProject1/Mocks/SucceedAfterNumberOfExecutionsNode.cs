using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.Mocks
{
    public class SucceedAfterNumberOfExecutionsNode<T> : IExecutable<T>
    {
        private readonly int _succeedAfterExecution;
        private int _executionCount;

        public SucceedAfterNumberOfExecutionsNode(int succeedAfter) => _succeedAfterExecution = succeedAfter;

        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            if (_executionCount < _succeedAfterExecution)
            {
                _executionCount++;
                throw new Exception();
            }

            _executionCount++;
            return Task.FromResult(context);
        }
    }
}
