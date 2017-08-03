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
        private readonly IList<Func<T, Task>> _workflowNodes;

        internal Workflow(IEnumerable<Func<T, Task>> workflowNodes)
        {
            _workflowNodes = new List<Func<T, Task>>(workflowNodes);
        }
        
        async Task IExecutable<T>.ExecuteAsync(T data)
        {
            foreach (var workflowNode in _workflowNodes)
            {
                await workflowNode(data).ConfigureAwait(false);
            }
        }
    }
}
