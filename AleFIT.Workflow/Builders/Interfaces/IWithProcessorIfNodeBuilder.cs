using System;
using System.Collections.Generic;
using System.Text;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IWithProcessorIfNodeBuilder<T>
    {
        IWithIfNodeBuilder<T> WithIf(IConditional<T> condition, IEnumerable<IExecutable<T>> actionsIfTrue);
    }
}
