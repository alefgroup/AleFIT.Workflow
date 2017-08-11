using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Core.Configuration
{
    internal interface IInternalWorkflowConfiguration : IWorkflowConfiguration
    {
        bool IsDefault { get; }

        int MaxRetryCount { get; set; }
    }
}
