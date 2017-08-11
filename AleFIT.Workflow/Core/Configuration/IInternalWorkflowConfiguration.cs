using AleFIT.Workflow.Builders.Interfaces;

namespace AleFIT.Workflow.Core.Configuration
{
    /// <summary>
    /// Internally available workflow configuration which extends the normal one.
    /// </summary>
    internal interface IInternalWorkflowConfiguration : IWorkflowConfiguration
    {
        /// <summary>
        /// Defines whether this configuration is default or user has reconfigured it.
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Defines a maximum number of retries used in <see cref="IWorkflowBuilder{T}.DoWithRetry"/>
        /// This property is set from the value of <see cref="IWorkflowBuilder{T}.DoWithRetry"/> method.
        /// </summary>
        int MaxRetryCount { get; set; }
    }
}
