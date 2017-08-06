namespace AleFIT.Workflow.Core
{
    public interface IConditionallyExecutable<T> : IExecutable<T>, IConditional<T>
    {
    }
}
