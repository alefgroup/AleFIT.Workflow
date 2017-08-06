namespace AleFIT.Workflow.Core.Configuration
{
    public interface IWorkflowConfiguration
    {
        bool ContinueOnError { get; }

        IWorkflowConfiguration Clone();
    }
}
