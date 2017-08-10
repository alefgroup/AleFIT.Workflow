using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Test.Mocks;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class ParallelNodeTests
    {
        [Fact]
        public async Task SingleParallelNode_ContinueOnErrorFalse_ThrowAnError_ShouldFail()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create(
                    configuration => { configuration.ContinueOnError = false; })
                .DoInParallel(
                    new DoNothingNode<GenericContext<int>>(),
                    new IncrementNode(),
                    new DecrementNode(),
                    new ThrowExceptionNode<GenericContext<int>>())
                .Do(new IncrementNode())
                .Build();

            var stopwatch = Stopwatch.StartNew();
            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));
            stopwatch.Stop();

            Assert.Equal(ExecutionState.Failed, result.State);
        }

        [Fact]
        public async Task SingleParallelNode_ContinueOnErrorTrue_ThrowAnError_ShouldComplete()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create(
                    configuration => { configuration.ContinueOnError = true; })
                .DoInParallel(
                    new DoNothingNode<GenericContext<int>>(),
                    new IncrementNode(),
                    new IncrementNode(),
                    new DecrementNode(),
                    new ThrowExceptionNode<GenericContext<int>>())
                .Do(new IncrementNode())
                .Build();

            var stopwatch = Stopwatch.StartNew();
            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));
            stopwatch.Stop();

            Assert.Equal(ExecutionState.Completed, result.State);
            Assert.Equal(7, result.ProcessedActions);
            Assert.Equal(2, result.Data.SampleData);
        }

        [Fact]
        public async Task SingleParallelNode_ShouldExecuteInParallel()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoInParallel(
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)),
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)),
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)))
                .Build();

            var stopwatch = Stopwatch.StartNew();
            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));
            stopwatch.Stop();

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(ExecutionState.Completed, result.State);
            Assert.Equal(4, result.ProcessedActions); // execution counts itself as node too
            Assert.InRange(stopwatch.Elapsed, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task MultipleParallelNodes_ShouldExecuteInParallel()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoInParallel(
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)),
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)))
                .DoInParallel(
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)),
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)))
                .DoInParallel(
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)),
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)))
                .Build();

            var stopwatch = Stopwatch.StartNew();
            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));
            stopwatch.Stop();

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(ExecutionState.Completed, result.State);
            Assert.Equal(9, result.ProcessedActions);
            Assert.InRange(stopwatch.Elapsed, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(4));
        }

        [Fact]
        public async Task NestedParallelNodes_ShouldExecuteInParallel()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(new DoNothingNode<GenericContext<int>>()) // 1
                .DoInParallel( // 2
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)), // 3
                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1))) // 4
                .If(new FixedConditionalNode<GenericContext<int>>(false), // 5
                    new DoNothingNode<GenericContext<int>>(),
                    WorkflowBuilder<GenericContext<int>>.Create() // 6
                        .Do(new DoNothingNode<GenericContext<int>>()) // 7
                        .DoInParallel( // 8
                            new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)), // 9
                            new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1))) // 10
                        .If(new FixedConditionalNode<GenericContext<int>>(true), // 11
                            WorkflowBuilder<GenericContext<int>>.Create() // 12
                                .Do(new DoNothingNode<GenericContext<int>>()) // 13
                                .DoInParallel( // 14
                                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1)), // 15
                                    new WaitNode<GenericContext<int>>(TimeSpan.FromSeconds(1))) // 16
                            .Build(),
                            new DoNothingNode<GenericContext<int>>())
                    .Build())
                .Build();

            var stopwatch = Stopwatch.StartNew();
            var result = await workflow.ExecuteAsync(new GenericContext<int>(0));
            stopwatch.Stop();

            Assert.Equal(0, result.Data.SampleData);
            Assert.Equal(ExecutionState.Completed, result.State);
            Assert.Equal(16, result.ProcessedActions);
            Assert.InRange(stopwatch.Elapsed, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(4));
        }
    }
}
