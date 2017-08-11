using AleFIT.Workflow.Builders.Interfaces;

namespace AleFIT.Workflow.Core.Configuration
{
    /// <summary>
    /// Defines an immutable workflow configuration.
    /// </summary>
    public interface IWorkflowConfiguration
    {
        /// <summary>
        /// Sets whether a workflow execution should continue when an error occurs.
        /// <remarks>Default is false</remarks>
        /// </summary>
        bool ContinueOnError { get; }

        /// <summary>
        /// Sets a maximum number of parallel execution used in <see cref="IWorkflowBuilder{T}.DoInParallel"/> action.
        /// <remarks>Default is 4</remarks>
        /// </summary>
        int DegreeOfParallelism { get; }        
    }
}
