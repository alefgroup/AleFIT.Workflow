using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IFullConditionalNodeBuilder<T> : IEmptyConditionalNodeBuilder<T>
    {
        IConditionalWorkflowNode<T> Else(Func<T, Task<T>> actionToExecute);
    }
}
