using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.Mocks
{
    public class ThrowExceptionNode<T> : IExecutable<T>
    {
        private readonly Exception _exception;

        public ThrowExceptionNode()
        {
            _exception = new Exception();
        }

        public ThrowExceptionNode(Exception exception)
        {
            _exception = exception;
        }
        
        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            throw _exception;
        }
    }
}
