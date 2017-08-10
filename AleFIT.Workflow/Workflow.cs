using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Configuration;
using AleFIT.Workflow.Executors;

namespace AleFIT.Workflow
{
    public class Workflow<T> : IWorkflow<T>
    {
        private readonly IWorkflowConfiguration _configuration;
        private readonly IExecutionProcessor<T> _executionProcessor;
        private readonly IReadOnlyList<IExecutable<T>> _workflowNodes;

        internal Workflow(
            IExecutionProcessor<T> executionProcessor, 
            IWorkflowConfiguration configuration,
            IEnumerable<IExecutable<T>> workflowNodes)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _executionProcessor = executionProcessor ?? throw new ArgumentNullException(nameof(executionProcessor));
            _workflowNodes = workflowNodes?.ToList() ?? throw new ArgumentNullException(nameof(workflowNodes));
        }

        public async Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            return await _executionProcessor.ProcessAsync(context, _workflowNodes).ConfigureAwait(false);
        }

        public async Task<ExecutionContext<T>> ExecuteAsync(T data)
        {
            var context = new ExecutionContext<T>(data, _configuration);

            return await _executionProcessor.ProcessAsync(context, _workflowNodes).ConfigureAwait(false);
        }

        public async Task<ExecutionContext<T>> ContinueAsync(ExecutionContext<T> context)
        {
            if (context.State == ExecutionState.Paused && context.PersistedExecutionIndexes.Count > 0)
            {
                return await _executionProcessor.ProcessAsync(context, _workflowNodes).ConfigureAwait(false);
            }

            throw new InvalidOperationException("Workflow is not in Paused state or there are no actions Pause actions in queue.");
        }
    }
}
