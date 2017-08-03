using System;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Nodes
{
    internal interface IConditionalWorkflowNode<in T> : IExecutable<T>
    {

    }
}