using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    internal interface IEmptyConditionalNodeBuilder<T>
    {
        IFullConditionalNodeBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue);
    }
}