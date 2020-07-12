using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
    public static class Async
    {
        public static Task ParallelForAsync4<T, TArg>(this T[] source, TArg arg, Func<T, int, int, TArg, Task> funcBody)
        {
            int lg = source.Length;

            const int div0 = 0;
            var div1 = lg / 4;
            var div2 = lg / 2;
            var div3 = lg * 3 / 4;
            var div4 = lg;

            async Task AwaitPartition(T[] source, int partition, int i0, int i1, TArg arg)
            {
                for (int i = i0; i < i1; ++i)
                {
                    await funcBody(source[i], partition, i, arg);
                }
            }

            var tasks = new Task[4];

            tasks[0] = Task.Run(() => AwaitPartition(source, 0, div0, div1, arg));
            tasks[1] = Task.Run(() => AwaitPartition(source, 1, div1, div2, arg));
            tasks[2] = Task.Run(() => AwaitPartition(source, 2, div2, div3, arg));
            tasks[3] = Task.Run(() => AwaitPartition(source, 3, div3, div4, arg));


            return Task.WhenAll(tasks);
        }

        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await funcBody(partition.Current); }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }


        public static Task ParallelForEachAsync<T1, T2>(this IEnumerable<T1> source, Func<T1, T2, Task> funcBody, T2 inputClass, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T1> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await funcBody(partition.Current, inputClass); }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }

        public static Task ParallelForEachAsync<T1, T2, T3>(this IEnumerable<T1> source, Func<T1, T2, T3, Task> funcBody, T2 inputClass, T3 secondInputClass, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T1> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await funcBody(partition.Current, inputClass, secondInputClass); }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }

        public static Task ParallelForEachAsync<T1, T2, T3, T4>(this IEnumerable<T1> source, Func<T1, T2, T3, T4, Task> funcBody, T2 inputClass, T3 secondInputClass, T4 thirdInputClass, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T1> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await funcBody(partition.Current, inputClass, secondInputClass, thirdInputClass); }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }
    }
}
