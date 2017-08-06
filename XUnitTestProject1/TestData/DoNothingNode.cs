using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.TestData
{
    public class DoNothingNode<T> : IExecutable<T>
    {
        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context) => Task.FromResult(context);
    }
}
