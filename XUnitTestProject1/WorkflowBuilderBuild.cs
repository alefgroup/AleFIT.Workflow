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

        [Fact]
        public async Task NestedIfNodes_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(context => Task.FromResult(true), WorkflowBuilder<GenericContext<int>>.Create()
                    .If(context => Task.FromResult(true),
                        new DoNothingNode<GenericContext<int>>(),
                        new DoNothingNode<GenericContext<int>>())
                    .Build(),
                    new DoNothingNode<GenericContext<int>>())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(4, result.ProcessedActions);
        }

        [Fact]
        public async Task MultipleWorkflows_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(WorkflowBuilder<GenericContext<int>>.Create().Do(new DoNothingNode<GenericContext<int>>()).Build())
                .Do(WorkflowBuilder<GenericContext<int>>.Create().Do(new DoNothingNode<GenericContext<int>>()).Build())
                .Do(WorkflowBuilder<GenericContext<int>>.Create().Do(new DoNothingNode<GenericContext<int>>()).Build())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(6, result.ProcessedActions);
        }

        [Fact]
        public async Task NestedIfAndWorkflowNodes_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(new DoNothingNode<GenericContext<int>>()) // 1
                .Do(new DoNothingNode<GenericContext<int>>()) // 2
                .If(new FixedConditionalNode<GenericContext<int>>(false), // 3
                    new DoNothingNode<GenericContext<int>>(),
                    WorkflowBuilder<GenericContext<int>>.Create() // 4
                        .Do(new DoNothingNode<GenericContext<int>>()) // 5
                        .Do(new DoNothingNode<GenericContext<int>>()) // 6
                        .Do(new DoNothingNode<GenericContext<int>>()) // 7
                        .If(new FixedConditionalNode<GenericContext<int>>(true), // 8
                            WorkflowBuilder<GenericContext<int>>.Create() // 9
                                .Do(new DoNothingNode<GenericContext<int>>()) // 10
                                .Do(new DoNothingNode<GenericContext<int>>()) // 11
                            .Build(),
                            new DoNothingNode<GenericContext<int>>())
                    .Build())
                .Build();

            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(11, result.ProcessedActions);
        }
    }
}
