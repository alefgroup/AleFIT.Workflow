using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;
using AleFIT.Workflow.Test.TestData;

namespace AleFIT.Workflow.Test.TestNodes
{
    public class DoNothingNode<T> : IExecutable<ExecutionContext<T>>
    {
        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> data)
        {
            return Task.FromResult(data);
        }
    }
}