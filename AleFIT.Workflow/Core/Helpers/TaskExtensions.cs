using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Core.Helpers
{
    internal static class TaskExtensions
    {
        internal static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                }));
        }
    }
}
