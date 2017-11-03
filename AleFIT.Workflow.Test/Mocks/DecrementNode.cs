using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Test.TestData;

namespace AleFIT.Workflow.Test.Mocks
{
    public class DecrementNode : IExecutable<GenericContext<int>>
    {
        public Task<ExecutionContext<GenericContext<int>>> ExecuteAsync(ExecutionContext<GenericContext<int>> context)
        {
            context.Data.SampleData--;
            return Task.FromResult(context);
        }
    }
}
