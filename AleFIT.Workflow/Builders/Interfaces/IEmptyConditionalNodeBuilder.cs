using System;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IEmptyConditionalNodeBuilder<T>
    {
        IFullConditionalNodeBuilder<T> ElseIf(Func<T, Task<bool>> condition, Func<T, Task<T>> actionIfTrue);
    }
}