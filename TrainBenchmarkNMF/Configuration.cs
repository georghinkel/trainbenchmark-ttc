using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTC2015.TrainBenchmark
{
    public class Configuration
    {
        [Option("runs", Required = true, HelpText = "The number of runs that the benchmark should run")]
        public int Runs { get; set; }

        [Option("size", Required = true, HelpText = "The size of the input model, needed for serialization")]
        public int Size { get; set; }

        [Option("query", Required = true, HelpText = "The query that should be evaluated")]
        public string Query { get; set; }

        [Option("changeSet", Required = true, HelpText = "A value indicating the change set")]
        public string ChangeSet { get; set; }

        [Option("iterationCount", Required=true, HelpText="The number of iterations")]
        public int IterationCount { get; set; }

        [Option("tool", Required = false, HelpText = "The text printed to the tool field")]
        public string Tool { get; set; }

        [ValueOption(0)]
        public string Target { get; set; }

        [Option("promotion", Required = false, DefaultValue = false, HelpText = "Determines whether the solution should be run in argument promotion")]
        public bool ArgumentPromotion { get; set; }

        [Option("batch", Required = false, HelpText = "Runs validation in batch mode")]
        public bool Batch { get; set; }

        [Option("inject", Required = false, HelpText = "Runs the benchmark in inject mode")]
        public bool Inject { get; set; }

        [Option("configuration", Required = false, HelpText = "The incrementalization configuration. Ignored if the benchmark is run in batch mode. If no incrementalization is provided, the default incrementalization is chosen.")]
        public string IncrementalizationConfig { get; set; }
    }
}
