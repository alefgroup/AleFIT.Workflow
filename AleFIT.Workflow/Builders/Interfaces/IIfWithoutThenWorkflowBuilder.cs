using System;
using System.Collections.Generic;
using System.Text;
using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    public interface IIfWithoutThenWorkflowBuilder<T>
    {
        IConditionalWorkflowBuilder<T> Then(IExecutable<T> actionIfTrue);
    }
}
