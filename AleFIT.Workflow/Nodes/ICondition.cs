using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Model;

namespace AleFIT.Workflow.Nodes
{
    public interface ICondition<in T>
    {
        Task<bool> Evaluate(T context);
    }
}
