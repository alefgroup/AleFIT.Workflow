using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IConditionalWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue);

        IIfWithoutThenWorkflowBuilder<T> ElseIf(IConditional<T> condition);

        IWorkflowBuilder<T> Else(IEnumerable<IExecutable<T>> elseActions);
    }
}
