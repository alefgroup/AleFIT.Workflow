namespace AleFIT.Workflow.Core
{
    /// <summary>
    /// Defines an object that can be conditionally executed.
    /// </summary>
    /// <typeparam name="T">Data used in the context of this object.</typeparam>
    public interface IConditionallyExecutable<T> : IExecutable<T>, IConditional<T>
    {
    }
}
