using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Helpers;
using AleFIT.Workflow.Test.Mocks;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class DoWorkflowNodeTests
    {
        [Theory]
        [InlineData(1), InlineData(-10), InlineData(20)]
        public async Task DoMutateContext_ShouldReturnMutatedContext(int data)
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().Do(
                c =>
                    {
                        c.Data.SampleData++;
                        return Task.FromResult(c);
                    }).Do(
                c =>
                    {
                        c.Data.SampleData++;
                        return Task.FromResult(c);
                    })
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(data));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(data + 2, context.Data.SampleData);
        }

        [Fact]
        public async Task NullContextData_ShouldProcessNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().Do(
                c =>
                    {
                        Debug.Write(c.State);
                        return Task.FromResult(c);
                    }).Build();

            var context = await workflow.ExecuteAsync(null);

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Null(context.Data);
        }
    }
}
