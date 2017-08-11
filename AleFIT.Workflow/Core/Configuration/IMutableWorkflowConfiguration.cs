namespace AleFIT.Workflow.Core.Configuration
{
    public interface IMutableWorkflowConfiguration
    {
        bool ContinueOnError { get; set; }

        int DegreeOfParallelism { get; set; }
    }
}
