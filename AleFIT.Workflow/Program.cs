using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow
{
    class Program
    {
        public async Task Process()
        {
            var workflow = WorkflowBuilder<Data>.Create()
                .Do(context =>
                {
                    context.Data.Id = 3;
                    return Task.FromResult(0);
                })
                .If(context => Task.FromResult(true), context => Task.FromResult(0), context => Task.FromResult(0))
                .Do(context => Task.FromResult(0))
                .Build();

            await workflow.ExecuteAsync(new Data { Id = 1, Name = "Franta", Ip = "10.0.0.1" });
        }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
    }
}
