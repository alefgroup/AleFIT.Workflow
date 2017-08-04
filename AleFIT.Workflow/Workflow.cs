using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;

namespace AleFIT.Workflow
{
    public class Workflow<T> : IWorkflow<T>
    {
        private readonly WorkflowConfiguration _configuration;
        private readonly IList<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> _workflowNodes;

        internal Workflow(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> workflowNodes, WorkflowConfiguration configuration)
        {
            if (workflowNodes == null) throw new ArgumentNullException(nameof(workflowNodes));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _workflowNodes = new List<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>>(workflowNodes);
        }
        
        public async Task<ExecutionContext<T>> ExecuteAsync(T data)
        {
            var context = new ExecutionContext<T>(data);
            return await ExecuteInternalAsync(context);
        }

        async Task<ExecutionContext<T>> IExecutable<ExecutionContext<T>>.ExecuteAsync(ExecutionContext<T> data)
        {
            return await ExecuteInternalAsync(data);
        }

        private async Task<ExecutionContext<T>> ExecuteInternalAsync(ExecutionContext<T> data)
        {
            ExecutionContext<T> context = data;
            foreach (var workflowNode in _workflowNodes)
            {
                try
                {
                    context = await workflowNode(context).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (_configuration.ContinueOnError)
                    {
                        context.Exception = exception;
                    }
                    else
                    {
                        context.SetFaulted(exception);
                        break;
                    }
                }
                finally
                {
                    context.ProcessedNodes++;
                }
            }
            return context;
        }
    }
}
