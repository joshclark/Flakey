using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flakey.StressTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cpuCount = Environment.ProcessorCount;
            Console.WriteLine($"Running stress tests for {cpuCount} processors...");

            int iterationsPerProc = 20 * 1000000;
            var gen = new IdGenerator(1);

            Console.WriteLine($"Generating {iterationsPerProc} ids per processor.");

            var allTasks = new List<Task<LinkedList<long>>>();
            foreach (int cpu in Enumerable.Range(0, cpuCount))
            {
                allTasks.Add(GetIds(gen, iterationsPerProc));
            }

            Task.WhenAll(allTasks).Wait();

            Console.WriteLine($"Verifying generated ids...");

            foreach (var task in allTasks)
            {
                VerifyIds(task.Result);
            }

            Console.WriteLine("All done.");
        }

        private static Task<LinkedList<long>> GetIds(IdGenerator gen, int count)
        {
            return Task.Run(() => new LinkedList<long>(gen.Take(count)));
        }

        private static void VerifyIds(LinkedList<long> result)
        {
            int index = 0;
            var current = result.First.Next;
            while (current != null)
            {
                if (current.Previous != null) 
                {
                    if (current.Value <= current.Previous.Value)
                    {
                        Console.WriteLine($"Results out of order: Result[{index - 1}]: {current.Previous.Value}  Result[{index}]: {current.Value}");
                    }
                }
                current = current.Next;
                index++;
            }
        }


    }
}
