using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTC2015.TrainBenchmark;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmark = new SwitchSetBenchmark();
            var results = benchmark.MeasureConfiguration(new NMF.Expressions.IncrementalizationConfiguration.Configuration());
            foreach (var metric in benchmark.Metrics)
            {
                Console.WriteLine("{0}={1}", metric, results[metric]);
            }
            Console.Read();
        }
    }
}
