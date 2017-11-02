using NMF.Expressions;
using IncConfig = NMF.Expressions.IncrementalizationConfiguration;
using NMF.Models;
using NMF.Models.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTC2015.TrainBenchmark.Railway;

namespace TTC2015.TrainBenchmark
{
    public class Program
    {
        public enum Mode
        {
            Fixed,
            Proportional,
            RandomChanges
        }

        private static readonly Stopwatch stopwatch = new Stopwatch();
        public static string tool;
        public static Configuration configuration;

        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
                configuration = new Configuration();
                if (!CommandLine.Parser.Default.ParseArguments(args, configuration))
                {
                    Console.Error.WriteLine("Wrong parameter arguments!");
                    Console.Error.WriteLine(CommandLine.Text.HelpText.AutoBuild(configuration));
                }

                IncConfig.Configuration incConfiguration = null;

                if (configuration.IncrementalizationConfig != null)
                {
                    var configurationRepo = new ModelRepository();
                    var configModel = configurationRepo.Resolve(configuration.IncrementalizationConfig);
                    incConfiguration = configModel.RootElements[0] as IncConfig.Configuration;
                }
                else if (configuration.ArgumentPromotion)
                {
                    NotifySystem.DefaultSystem = new PromotionNotifySystem();
                }

                var fixedChangeSet = string.Equals(configuration.ChangeSet, "fixed", StringComparison.InvariantCultureIgnoreCase) ? Mode.Fixed :
                                     string.Equals(configuration.ChangeSet, "proportional", StringComparison.InvariantCultureIgnoreCase) ? Mode.Proportional :
                                     Mode.RandomChanges;
                for (int i = 0; i < configuration.Runs; i++)
                {
                    var repo = new ModelRepository();
                    if (incConfiguration != null)
                    {
                        NotifySystem.DefaultSystem = new ConfiguredNotifySystem(repo, incConfiguration);
                    }
                    RunBenchmark(fixedChangeSet, i, repo);
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
#endif
        }

        public static void RunBenchmark(Mode operationMode, int i, IModelRepository repository)
        {
            stopwatch.Start();
            var train = repository.Resolve(new Uri(new FileInfo(configuration.Target).FullName));
            var railwayContainer = train.Model.RootElements.Single() as RailwayContainer;

            TrainRepair trainRepair = null;
            if (!configuration.Batch)
            {
                trainRepair = new IncrementalTrainRepair();
                tool = tool ?? "NMF(Incremental)";
            }
            else
            {
                trainRepair = new BatchTrainRepair();
                tool = tool ?? "NMF(Batch)";
            }
            if (!string.IsNullOrEmpty(configuration.Tool))
            {
                tool = configuration.Tool;
            }
            trainRepair.RepairTrains(railwayContainer, configuration.Query);
            stopwatch.Stop();
            Emit("read", i, 0, null);

            if (operationMode == Mode.RandomChanges)
            {
                trainRepair.PrepareRandomChanges(railwayContainer);
            }

            // Check
            stopwatch.Restart();
            var actions = trainRepair.Check();
            stopwatch.Stop();
            Emit("check", i, 0, actions.Count());

            if (!configuration.Inject)
            {
                var actionsSorted = (from pair in actions
                                     orderby pair.Item1
                                     select pair.Item2).ToList();

                for (int iter = 0; iter < configuration.IterationCount; iter++)
                {
                    // Repair
                    Repair(operationMode, trainRepair, actionsSorted);
                    Emit("repair", i, iter, null);

                    // ReCheck
                    stopwatch.Restart();
                    actions = trainRepair.Check();
                    stopwatch.Stop();
                    Emit("recheck", i, iter, actions.Count());

                    actionsSorted = (from pair in actions
                                     orderby pair.Item1
                                     select pair.Item2).ToList();
                }
            }
            else
            {
                var actionsSorted = (from pair in trainRepair.Inject()
                                     orderby pair.Item1
                                     select pair.Item2).ToList();

                for (int iter = 0; iter < configuration.IterationCount; iter++)
                {
                    // Repair
                    Repair(operationMode, trainRepair, actionsSorted);
                    Emit("inject", i, iter, null);

                    // ReCheck
                    stopwatch.Restart();
                    actions = trainRepair.Check();
                    stopwatch.Stop();
                    Emit("recheck", i, iter, actions.Count());

                    actionsSorted = (from pair in trainRepair.Inject()
                                     orderby pair.Item1
                                     select pair.Item2).ToList();
                }
            }
        }

        private static void Repair(Mode operationMode, TrainRepair trainRepair, List<Action> actionsSorted)
        {
            switch (operationMode)
            {
                case Mode.Fixed:
                    stopwatch.Restart();
                    ExecutionEngine.Current.BeginTransaction();
                    trainRepair.RepairFixed(10, actionsSorted);
                    ExecutionEngine.Current.CommitTransaction();
                    stopwatch.Stop();
                    break;
                case Mode.Proportional:
                    stopwatch.Restart();
                    ExecutionEngine.Current.BeginTransaction();
                    trainRepair.RepairProportional(10, actionsSorted);
                    ExecutionEngine.Current.CommitTransaction();
                    stopwatch.Stop();
                    break;
                case Mode.RandomChanges:
                    stopwatch.Restart();
                    ExecutionEngine.Current.BeginTransaction();
                    trainRepair.PerformRandomChanges(10, actionsSorted);
                    ExecutionEngine.Current.CommitTransaction();
                    stopwatch.Stop();
                    break;
                default:
                    break;
            }
        }

        private static void Emit(string phase, int runIdx, int iteration, int? elements)
        {
            const string format = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}";

			Console.Out.WriteLine(format, configuration.ChangeSet, runIdx, tool, configuration.Size, configuration.Query, phase, iteration, "time", stopwatch.ElapsedTicks * 100);
			Console.Out.WriteLine(format, configuration.ChangeSet, runIdx, tool, configuration.Size, configuration.Query, phase, iteration, "memory", Environment.WorkingSet);
            if (elements != null)
            {
				Console.Out.WriteLine(format, configuration.ChangeSet, runIdx, tool, configuration.Size, configuration.Query, phase, iteration, "rss", elements.Value);
            }
        }
    }
}
