using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

using NUnit.Core;

using PNUnit.Framework;

using log4net;
using log4net.Config;


namespace PNUnit.Launcher
{
    class Launcher
    {
        private static readonly ILog log = LogManager.GetLogger("launcher");

        static string mTestPath = string.Empty;

        private static int MAX_TEST_RETRY = 3;

        [STAThread]
        static void Main(string[] args)
        {
            // Load the test configuration file
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: launcher configfile");
                return;
            }

            string configfile = args[0];

            mTestPath = Path.GetDirectoryName(configfile);
            TestGroup group = TestConfLoader.LoadFromFile(configfile);

            if ((group == null) || (group.ParallelTests.Length == 0))
            {
                Console.WriteLine("No tests to run");
                return;
            }

            ConfigureLogging();

            ConfigureRemoting();

            ArrayList failedGroups = new ArrayList();


            // Each parallel test is launched sequencially...
            Runner[] runners = new Runner[group.ParallelTests.Length];
            int i = 0;
            DateTime beginTimestamp = DateTime.Now;

            foreach (ParallelTest test in group.ParallelTests)
            {

                int retryCount = 0;

                bool bRetry = true;

                while (bRetry && retryCount < MAX_TEST_RETRY)
                {
                    bRetry = false;

                    log.InfoFormat("Test {0} of {1}", i + 1, group.ParallelTests.Length);

                    Runner runner = new Runner(test);
                    runner.Run();

                    runners[i] = runner;
                    // Wait to finish
                    runner.Join();

                    TestResult[] runnerResults = runner.GetTestResults();

                    if (RetryTest(runnerResults))
                    {
                        bRetry = true;
                        ++retryCount;
                        log.Info("Test failed with retry option, trying again");
                        continue;
                    }

                    if (FailedTest(runnerResults))
                    {
                        WriteFailed(runnerResults);
                        failedGroups.Add(test);
                        WriteFailedGroup(failedGroups);
                    }
                }

                ++i;
            }

            DateTime endTimestamp = DateTime.Now;

            // Print the results
            double TotalBiggerTime = 0;
            int TotalTests = 0;
            int TotalExecutedTests = 0;
            int TotalFailedTests = 0;
            int TotalSuccessTests = 0;

            IList failedTests = new ArrayList();

            foreach (Runner runner in runners)
            {
                int ExecutedTests = 0;
                int FailedTests = 0;
                int SuccessTests = 0;
                double BiggerTime = 0;
                TestResult[] results = runner.GetTestResults();
                log.InfoFormat("==== Tests Results for Parallel TestGroup {0} ===", runner.TestGroupName);
                i = 0;
                foreach (TestResult res in results)
                {
                    if (res.Executed)
                        ++ExecutedTests;
                    if (res.IsFailure)
                        ++FailedTests;
                    if (res.IsSuccess)
                        ++SuccessTests;

                    PrintResult(++i, res);
                    if (res.Time > BiggerTime)
                        BiggerTime = res.Time;

                    if (res.IsFailure)
                        failedTests.Add(res);
                }

                log.InfoFormat("Summary:");
                log.InfoFormat("\tTotal: {0}\n\tExecuted: {1}\n\tFailed: {2}\n\tSuccess: {3}\n\t% Success: {4}\n\tBiggest Execution Time: {5} s\n",
                    results.Length, ExecutedTests, FailedTests, SuccessTests,
                    results.Length > 0 ? 100 * SuccessTests / results.Length : 0,
                    BiggerTime);

                TotalTests += results.Length;
                TotalExecutedTests += ExecutedTests;
                TotalFailedTests += FailedTests;
                TotalSuccessTests += SuccessTests;
                TotalBiggerTime += BiggerTime;
            }

            // print all failed tests together
            if (failedTests.Count > 0)
            {
                log.InfoFormat("==== Failed tests ===");
                for (i = 0; i < failedTests.Count; ++i)
                    PrintResult(i, failedTests[i] as TestResult);
            }

            if (runners.Length > 1)
            {

                log.InfoFormat("Summary for all the parallel tests:");
                log.InfoFormat("\tTotal: {0}\n\tExecuted: {1}\n\tFailed: {2}\n\tSuccess: {3}\n\t% Success: {4}\n\tBiggest Execution Time: {5} s\n",
                    TotalTests, TotalExecutedTests, TotalFailedTests, TotalSuccessTests,
                    TotalTests > 0 ? 100 * TotalSuccessTests / TotalTests : 0,
                    TotalBiggerTime);
            }

            TimeSpan elapsedTime = endTimestamp.Subtract(beginTimestamp);
            log.InfoFormat("Launcher execution time: {0} seconds", elapsedTime.TotalSeconds);
        }

        private static bool FailedTest(TestResult[] results)
        {
            foreach (TestResult res in results)
                if (!res.IsSuccess)
                    return true;
            return false;
        }

        private static bool RetryTest(TestResult[] results)
        {
            foreach (TestResult res in results)
                if (res.IsFailure)
                    return false;
            return false;
        }

        private static void ConfigureRemoting()
        {
            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider =
                new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel =
                System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = 0;
            props["typeFilterLevel"] = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(
                props, clientProvider, serverProvider);

            ChannelServices.RegisterChannel(chan);
        }


        private static void PrintResult(int testNumber, TestResult res)
        {
            string[] messages = GetErrorMessages(res);
            log.InfoFormat("({0}) {1}", testNumber, messages[0]);
            if (!res.IsSuccess)
                log.InfoFormat(messages[1]);
        }

        private static string[] GetErrorMessages(TestResult res)
        {
            string[] result = new string[2];

            result[0] = string.Format(
                "Name: {0}\n  Result: {1,-12} Assert Count: {2,-2} Time: {3,5}",
                res.Name,
                res.IsSuccess ? "SUCCESS" : (res.IsFailure ? "FAILURE" : (!res.Executed ? "NOT EXECUTED" : "UNKNOWN")),
                res.AssertCount,
                res.Time);

            if (!res.IsSuccess)
                result[1] = string.Format(
                    "\nMessage: {0}\nStack Trace:\n{1}\n\n",
                        res.Message, res.StackTrace);

            return result;
        }

        private static void WriteFailed(TestResult[] results)
        {
            FileStream fs = new FileStream(Path.Combine(mTestPath, "smoke-errors.log"), FileMode.OpenOrCreate, FileAccess.ReadWrite);


            StreamWriter writer = new StreamWriter(fs);

            try
            {
                foreach (TestResult result in results)
                {
                    if (result.IsSuccess)
                        continue;

                    string[] messages = GetErrorMessages(result);

                    writer.WriteLine(messages[0]);
                    writer.WriteLine(messages[1]);
                }
            }
            finally
            {
                writer.Flush();
                writer.Close();
                fs.Close();
            }
        }

        private static void WriteFailedGroup(ArrayList failedTests)
        {
            TestGroup group = new TestGroup();
            group.ParallelTests = (ParallelTest[])failedTests.ToArray(typeof(ParallelTest));
            TestConfLoader.WriteToFile(group, Path.Combine(mTestPath, "smokefailed.conf"));
        }

        private static void ConfigureLogging()
        {
            string log4netpath = "launcher.log.conf";
            if (!File.Exists(log4netpath))
                log4netpath = Path.Combine(mTestPath, log4netpath);

            XmlConfigurator.Configure(new FileInfo(log4netpath));
        }


    }
}
