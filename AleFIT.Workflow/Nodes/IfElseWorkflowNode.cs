using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Executors;

namespace AleFIT.Workflow.Nodes
{
    internal class IfElseWorkflowNode<T> : IExecutable<T>
    {
        private readonly IReadOnlyList<IConditionallyExecutable<T>> _conditionalActions;
        private readonly IReadOnlyList<IExecutable<T>> _elseActions;
        private readonly IExecutionProcessor<T> _executionProcessor;

        public IfElseWorkflowNode(
            IEnumerable<IConditionallyExecutable<T>> conditionalActions, 
            IEnumerable<IExecutable<T>> elseActions,
            IExecutionProcessor<T> executionProcessor)
        {
            _conditionalActions = conditionalActions.ToList();
            _elseActions = elseActions.ToList();
            _executionProcessor = executionProcessor;
        }

        public async Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            foreach (var conditionalAction in _conditionalActions)
            {
                if (await conditionalAction.EvaluateAsync(context).ConfigureAwait(false))
                {
                    return await conditionalAction.ExecuteAsync(context).ConfigureAwait(false);
                }
            }

            return await _executionProcessor.ProcessAsync(context, _elseActions).ConfigureAwait(false);
        }
    }
}
