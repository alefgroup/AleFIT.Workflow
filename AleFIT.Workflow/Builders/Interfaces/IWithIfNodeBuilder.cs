using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IWithIfNodeBuilder<T>
    {
        IWithIfThenNodeBuilder<T> Then(IExecutable<T> actionIfTrue);
    }
}
