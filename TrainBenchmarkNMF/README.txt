Welcome TTC'2015 Participant to the NMF solution image clone for the TrainBenchmark Case and the Java Refactoring Case
=====================================================================================================

This image contains the source code of both the solutions and the frameworks required to run them.
The sources are compiled on Mono, whereas they were originally developed for and tested using the
.NET framework. But given that this is not available on a Linux machine, you can test the solutions
here using Mono.

Please do not take the performance figures from this run in Mono. According to our experience, it is
slower and less reliable than executing the same code on a Windows machine on .NET. In particular, a
row of features are still missing in Mono, which is why for example the memory measurement does not
work in the TrainBenchmark case.

If you want to review the source code, please run monodevelop (e.g. through the command line) and open
the TTC2015 solution. MonoDevelop will open with an exception, which you have to ignore. This should be
a bug in MonoDevelop and is not related to our solution. You will see most of the NMF libraries to run
the solutions and the solutions themselves, each as an own project. 

Overview of the TrainBenchmark solution
---------------------------------------

The solution to the TrainBenchmark case consists of the following parts:
- The metamodel folder contains the generated code for the Railway metamodel The model is also embedded as XMI
  through the railway.nmf file which contains the metamodel transformed to NMeta, which is the meta-metamodel
  that runs NMF.
- Configuration.cs contains a specification of the command line arguments
- Random.cs contains a random number generator that creates the exact same sequence as the Java RNG does
- TrainRepair.cs contains the patterns, the repair transformations and the implementations, both incremental
  and batch mode
- Program.cs contains the main method that coordinates the benchmark run

Running the TrainBenchmark solution
-----------------------------------

We modified the scripts to run the TrainBenchmark case to take into account that our solution is not
based on Java. To call this script, go to ~/Desktop/TTC2015/TrainBenchmark/scripts and run python run.py.
The only effect of this is that it will run both versions of the NMF solution (incremental and batch mode),
regardless of the configuration specified. Consequently, the JVM-args are ignored since also there is no JVM.

The solution already struggles on input model size 16. We assume that this is also caused by the Mono runtime.
On a Windows 8.1 machine running .NET 4.0 on a machine with 4 cores, 12GB RAM and a lot of opened applications,
the solution finishes for size 1024 within the specified timeout.

To run the solution directly, run 

mono TrainBenchmarkNMF.exe

This will tell you how to specify the correct parameters.

Building the solution manually
------------------------------

In linux, the most comfortable option to build the solution is of course using monodevelop. On the ArchLinux SHARE
demo however, you have to do about four or five attempts, as there are some problems with the build order.

If you want to compile from the command line, you better use xbuild, which is the Mono implementation of MSBuild. 
To do that, you have to install it first.

sudo apt-get install mono-xbuild mono-mcs

After that, you can build the solution using xbuild. To improve the performance, you should run in it in Release mode.

xbuild TrainBenchmarkNMF.sln /p:Configuration=Release

