using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IWithIfNodeBuilder<T>
    {
        IWithIfNodeBuilder<T> ElseIf(IConditional<T> condition, IExecutable<T> actionIfTrue);

        IWithIfNodeBuilder<T> ElseIf(
            IConditional<T> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IWithIfNodeBuilder<T> ElseIf(
            Func<ExecutionContext<T>, Task<bool>> condition,
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue);

        IfElseWorkflowNode<T> Else(IEnumerable<IExecutable<T>> elseActions);

        IfElseWorkflowNode<T> Else(IEnumerable<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> elseActions);
    }
}
