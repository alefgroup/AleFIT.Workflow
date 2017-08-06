using System;

namespace AleFIT.Workflow.Core.Configuration
{
    internal class WorkflowConfiguration : IWorkflowConfiguration, IMutableWorkflowConfiguration
    {
        public WorkflowConfiguration(bool continueOnError)
        {
            ContinueOnError = continueOnError;
        }

        public bool ContinueOnError { get; set; }

        public IWorkflowConfiguration Clone()
        {
            return new WorkflowConfiguration(ContinueOnError);
        }

        internal static WorkflowConfiguration CreateDefault()
        {
            return new WorkflowConfiguration(false);
        }
    }
}
