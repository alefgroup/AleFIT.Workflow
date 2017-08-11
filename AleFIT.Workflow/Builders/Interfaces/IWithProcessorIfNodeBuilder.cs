using System;
using System.Collections.Generic;
using System.Text;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Builders.Interfaces
{
    internal interface IWithProcessorIfNodeBuilder<T>
    {
        IWithIfThenNodeBuilder<T> WithIfThen(IConditional<T> condition, IEnumerable<IExecutable<T>> actionsIfTrue);

        IWithIfNodeBuilder<T> WithIf(IConditional<T> condition);
    }
}
