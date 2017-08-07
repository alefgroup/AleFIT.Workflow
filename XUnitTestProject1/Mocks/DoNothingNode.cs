using System.Threading.Tasks;
using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.Mocks
{
    public class DoNothingNode<T> : IExecutable<T>
    {
        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context) => Task.FromResult(context);
    }
}
