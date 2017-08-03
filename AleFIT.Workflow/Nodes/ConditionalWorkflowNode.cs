using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Nodes
{
    internal class ConditionalWorkflowNode<T> : IConditionalWorkflowNode<T>
    {
        private readonly IEnumerable<(Func<T, Task<bool>> condition, Func<T, Task> executeIfTrue)> _conditionalExecutions;
        private readonly Func<T, Task> _elseAction;

        public ConditionalWorkflowNode(IEnumerable<(Func<T, Task<bool>> condition, Func<T, Task> executeIfTrue)> conditionalExecutions, Func<T, Task> elseAction)
        {
            _conditionalExecutions = conditionalExecutions ?? throw new ArgumentNullException(nameof(conditionalExecutions));
            _elseAction = elseAction ?? throw new ArgumentNullException(nameof(elseAction));
        }

        public async Task ExecuteAsync(T data)
        {
            foreach (var conditionalExecution in _conditionalExecutions)
            {
                if (await conditionalExecution.condition(data))
                {
                    await conditionalExecution.executeIfTrue(data);
                    return;
                }
            }
            if (_elseAction != null)
            {
                await _elseAction(data);
            }
        }
    }
}
