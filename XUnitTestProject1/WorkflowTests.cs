using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Test.Mocks;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class WorkflowTests
    {
        [Fact]
        public async Task ContinueAsync_NullContext_ShouldThrowAnException()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().Build();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await workflow.ContinueAsync(null));
        }

        [Fact]
        public async Task ContinueAsync_NotPausedContext_ShouldThrowAnException()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.ContinueAsync(context));
        }

        [Fact]
        public async Task Execute_Pause_Continue_ShouldCompleteNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .Do(new IncrementNode())
                .Pause()
                .Do(new IncrementNode())
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Paused, context.State);
            Assert.Equal(2, context.Data.SampleData);

            context = await workflow.ContinueAsync(context);

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(3, context.Data.SampleData);
        }
    }
}
