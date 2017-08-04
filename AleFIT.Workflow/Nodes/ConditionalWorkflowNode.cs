using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Nodes
{
    internal class ConditionalWorkflowNode<T> : IConditionalWorkflowNode<T>
    {
        private readonly IEnumerable<IConditionallyExecutable<T>> _conditionalExecutions;
        private readonly Func<T, Task<T>> _elseAction;

        public ConditionalWorkflowNode(IEnumerable<IConditionallyExecutable<T>> conditionalExecutions, Func<T, Task<T>> elseAction)
        {
            _conditionalExecutions = conditionalExecutions ?? throw new ArgumentNullException(nameof(conditionalExecutions));
            _elseAction = elseAction ?? throw new ArgumentNullException(nameof(elseAction));
        }

        public async Task<T> ExecuteAsync(T data)
        {
            foreach (var conditionalExecution in _conditionalExecutions)
            {
                if (await conditionalExecution.ShouldExecuteAsync(data))
                {
                    return await conditionalExecution.ExecuteAsync(data);
                }
            }
            if (_elseAction != null)
            {
                return await _elseAction(data);
            }
            return data;
        }
    }
}
