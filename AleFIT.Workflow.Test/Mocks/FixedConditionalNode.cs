using System.Threading.Tasks;
using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Test.Mocks
{
    public class FixedConditionalNode<T> : IConditional<T>
    {
        private readonly bool _result;

        public FixedConditionalNode(bool result)
        {
            _result = result;
        }

        public Task<bool> EvaluateAsync(ExecutionContext<T> context) => Task.FromResult(_result);
    }
}
