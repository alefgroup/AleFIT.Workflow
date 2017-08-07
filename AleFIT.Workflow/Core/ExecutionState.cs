using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Core
{
    public enum ExecutionState
    {
        Running = 1,
        Paused = 2,
        Completed = 3,
        Failed = 4
    }
}
