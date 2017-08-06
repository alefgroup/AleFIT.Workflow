using System;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class WorkflowBuilderBuild
    {
        [Fact]
        public async Task EmptyWorkflow_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(0, result.ProcessedActions);
        }

        [Fact]
        public async Task SingleDoNode_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(new DoNothingNode<GenericContext<int>>())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(1, result.ProcessedActions);
        }


        [Fact]
        public async Task MultipleDoNodes_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(new DoNothingNode<GenericContext<int>>())
                .Do(new DoNothingNode<GenericContext<int>>())
                .Do(new DoNothingNode<GenericContext<int>>())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(3, result.ProcessedActions);
        }

        [Fact]
        public async Task SingleIfNode_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(context => Task.FromResult(true), 
                    new DoNothingNode<GenericContext<int>>(),
                    new DoNothingNode<GenericContext<int>>())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(2, result.ProcessedActions);
        }
    }
}
