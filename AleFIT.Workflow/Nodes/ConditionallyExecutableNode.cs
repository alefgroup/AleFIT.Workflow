using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Executors;

namespace AleFIT.Workflow.Nodes
{
    /// <summary>
    /// This class just holds the condition and an executable that will
    /// be executed using a specific processor.
    /// </summary>
    internal class ConditionallyExecutableNode<T> : IConditionallyExecutable<T>
    {
        private readonly IConditional<T> _condition;
        private readonly IEnumerable<IExecutable<T>> _actionsTrue;
        private readonly IExecutionProcessor<T> _executionProcessor;

        internal ConditionallyExecutableNode(
            IConditional<T> condition, 
            IEnumerable<IExecutable<T>> actionsTrue,
            IExecutionProcessor<T> executionProcessor)
        {
            _condition = condition;
            _actionsTrue = actionsTrue;
            _executionProcessor = executionProcessor;
        }


        public async Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context) => 
            await _executionProcessor.ProcessAsync(context, _actionsTrue.ToList()).ConfigureAwait(false);

        public async Task<bool> EvaluateAsync(ExecutionContext<T> context) => 
            await _condition.EvaluateAsync(context).ConfigureAwait(false);
    }
}
