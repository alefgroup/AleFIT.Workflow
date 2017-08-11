using AleFIT.Workflow.Builders.Interfaces;

namespace AleFIT.Workflow.Core.Configuration
{
    /// <summary>
    /// Defines a mutable workflow configuration.
    /// </summary>
    public interface IMutableWorkflowConfiguration
    {
        /// <summary>
        /// Sets whether a workflow execution should continue when an error occurs.
        /// <remarks>Default is false</remarks>
        /// </summary>
        bool ContinueOnError { get; set; }

        /// <summary>
        /// Sets a maximum number of parallel execution used in <see cref="IWorkflowBuilder{T}.DoInParallel"/> action.
        /// <remarks>Default is 4</remarks>
        /// </summary>
        int DegreeOfParallelism { get; set; }
    }
}
