namespace AleFIT.Workflow.Core.Configuration
{
    public interface IWorkflowConfiguration
    {
        bool ContinueOnError { get; }

        int DegreeOfParallelism { get; }        
    }
}
