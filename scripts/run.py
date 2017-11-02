#!/usr/bin/env python3
"""
@author: Zsolt Kovari

"""
import argparse
import logger
import logging
import os
import shutil
import subprocess
import sys
import util
from loader import Loader
from subprocess import CalledProcessError

def flatten(lst):
    return sum(([x] if not isinstance(x, list) else flatten(x) for x in lst), [])

def build(skip_tests):
    """Builds the project.
    """
    util.set_working_directory("..\\")
    if skip_tests:
        subprocess.check_call("mvn clean install -DskipTests", shell=True)
    else:
        subprocess.check_call("mvn clean install", shell=True)
    util.set_working_directory()


def generate(conf):
    """Generates the models.
    """
    target = util.get_generator_jar()
    for size in conf.sizes:
        subprocess.check_call(flatten(["java", conf.vmargs, "-jar", target, "-size", str(size)]))


def benchmark(conf):
    """Runs measurements.
    """
    header = "../output/header.tsv"
    result_file = "../output/output.tsv"
    if os.path.exists(result_file):
        os.remove(result_file)
    shutil.copy(header, result_file)
    for change_set in conf.change_sets:
        for size in conf.sizes:
            for query in conf.queries:
                for tool in conf.tools:
                    target = util.get_tool_jar(tool)
                    print("Running benchmark: tool = " + tool + ", change set = " + change_set +
                        ", query = " + query + ", size = " + str(size))
                    try:
                        output = subprocess.check_output(flatten(
                        ["java", conf.vmargs,
                         "-jar", target,
                         "-runs", str(conf.runs),
                         "-size", str(size),
                         "-query", query,
                         "-changeSet", change_set,
                         "-iterationCount", str(conf.iterations)]))
                        with open(result_file, "ab") as file:
                            file.write(output)
                    except CalledProcessError as e:
                        print("Program exited with error")

                print("Running benchmark: tool = NMF(Incremental), change set = " + change_set +
                      ", query = " + query + ", size = " + str(size))
                try:
                    output = subprocess.check_output(flatten(
                    ["..\\solution\\TrainBenchmarkNMF.exe",
					 "..\\models\\railway-{0}.railway".format(size),
                     "--runs", str(conf.runs),
                     "--tool", "NMF(Incremental)",
                     "--size", str(size),
                     "--query", query,
                     "--changeSet", change_set,
                     "--iterationCount", str(conf.iterations)]))
                    with open(result_file, "ab") as file:
                        file.write(output)
                except CalledProcessError as e:
                    print("Program exited with error")

                print("Running benchmark: tool = NMF(Parallel), change set = " + change_set +
                      ", query = " + query + ", size = " + str(size))
                try:
                    output = subprocess.check_output(flatten(
                    ["..\\solution_par\\TrainBenchmarkNMF.exe",
					 "..\\models\\railway-{0}.railway".format(size),
                     "--runs", str(conf.runs),
                     "--tool", "NMF(Parallel)",
                     "--size", str(size),
                     "--query", query,
                     "--changeSet", change_set,
                     "--iterationCount", str(conf.iterations)]))
                    with open(result_file, "ab") as file:
                        file.write(output)
                except CalledProcessError as e:
                    print("Program exited with error")

                print("Running benchmark: tool = NMF(Promotion), change set = " + change_set +
                      ", query = " + query + ", size = " + str(size))
                try:
                    output = subprocess.check_output(flatten(
                    ["..\\solution\\TrainBenchmarkNMF.exe",
					 "..\\models\\railway-{0}.railway".format(size),
                     "--runs", str(conf.runs),
                     "--tool", "NMF(Promotion)", "--promotion",
                     "--size", str(size),
                     "--query", query,
                     "--changeSet", change_set,
                     "--iterationCount", str(conf.iterations)]))
                    with open(result_file, "ab") as file:
                        file.write(output)
                except CalledProcessError as e:
                    print("Program exited with error")

                print("Running benchmark: tool = NMF(Batch), change set = " + change_set +
                      ", query = " + query + ", size = " + str(size))
                try:
                    output = subprocess.check_output(flatten(
                    ["..\\solution\\TrainBenchmarkNMF.exe",
					 "..\\models\\railway-{0}.railway".format(size),
					 "--batch",
                     "--tool", "NMF(Batch)",
                     "--runs", str(conf.runs),
                     "--size", str(size),
                     "--query", query,
                     "--changeSet", change_set,
                     "--iterationCount", str(conf.iterations)]))
                    with open(result_file, "ab") as file:
                        file.write(output)
                except CalledProcessError as e:
                    print("Program exited with error")


def clean_dir(dir):
    if os.path.exists(dir):
        shutil.rmtree(dir)
    os.mkdir(dir)


def visualize():
    """Visualizes the benchmark results
    """
    clean_dir("..\\diagrams")
    util.set_working_directory("..\\reporting")
    subprocess.call(["Rscript", "visualize.R", "..\\config\\reporting-1.json"])


def extract_results():
    """Extracts the benchmark results
    """
    clean_dir("..\\results")
    util.set_working_directory("..\\reporting")
    subprocess.call(["Rscript", "extract_results.R"])


def test():
    build(True)
    generate(config)
    build(False)
    benchmark(config)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-b", "--build",
                        help="build the project",
                        action="store_true")
    parser.add_argument("-g", "--generate",
                        help="generate models",
                        action="store_true")
    parser.add_argument("-m", "--measure",
                        help="run the benchmark",
                        action="store_true")
    parser.add_argument("-s", "--skip-tests",
                        help="skip JUNIT tests",
                        action="store_true")
    parser.add_argument("-v", "--visualize",
                        help="create visualizations",
                        action="store_true")
    parser.add_argument("-e", "--extract",
                        help="extract results",
                        action="store_true")
    parser.add_argument("-t", "--test",
                        help="run test",
                        action="store_true")
    args = parser.parse_args()


    util.set_working_directory()
    logger.init()
    loader = Loader()
    config = loader.load()

    if args.build:
        build(args.skip_tests)
    if args.generate:
        generate(config)
    if args.measure:
        benchmark(config)
    if args.test:
        test()
    if args.visualize:
        visualize()
    if args.extract:
        extract_results()

    # if there are no args, execute a full sequence
    # with the test and the visualization/reporting
    no_args = all(val==False for val in vars(args).values())
    if no_args:
        test()
        visualize()
        extract_results()
