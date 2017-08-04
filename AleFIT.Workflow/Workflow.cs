using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow
{
    public class Workflow<T> : IWorkflow<T>
    {
        private readonly IList<Func<ExecutionContext<T>, Task>> _workflowNodes;

        internal Workflow(IEnumerable<Func<ExecutionContext<T>, Task>> workflowNodes)
        {
            _workflowNodes = new List<Func<ExecutionContext<T>, Task>>(workflowNodes);
        }
        
        async Task IExecutable<T>.ExecuteAsync(T data)
        {
            var context = new ExecutionContext<T>(data);
            foreach (var workflowNode in _workflowNodes)
            {
                await workflowNode(context).ConfigureAwait(false);
            }
        }
    }
}
