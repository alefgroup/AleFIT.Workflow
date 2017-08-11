using System;

namespace AleFIT.Workflow.Core.Configuration
{
    internal class WorkflowConfiguration : IInternalWorkflowConfiguration, IMutableWorkflowConfiguration
    {
        public WorkflowConfiguration(bool continueOnError, int degreeOfParallelism)
        {
            ContinueOnError = continueOnError;
            DegreeOfParallelism = degreeOfParallelism;
        }

        public bool ContinueOnError { get; set; }

        public int DegreeOfParallelism { get; set; }

        public bool IsDefault { get; set; }

        public int MaxRetryCount { get; set; }
        
        internal static WorkflowConfiguration CreateDefault()
        {
            return new WorkflowConfiguration(false, 4) { IsDefault = true };
        }
    }
}
