using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Model
{
    public class WorkflowConfiguration
    {
        private WorkflowConfiguration() { }

        internal static WorkflowConfiguration CreateDefault()
        {
            return new WorkflowConfiguration();
        }

        public bool ContinueOnError { get; set; } = false;

        internal WorkflowConfiguration Clone()
        {
            return new WorkflowConfiguration
            {
                ContinueOnError = ContinueOnError
            };
        }
    }
}
