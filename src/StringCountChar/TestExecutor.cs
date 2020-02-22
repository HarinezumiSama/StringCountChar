﻿

using System;
using System.Diagnostics;
using System.Numerics;
using StringCountChar.Properties;

namespace StringCountChar
{
    internal static class TestExecutor
    {
        public static readonly string TestTextData = Resources.TestTextData;
        public static readonly char SearchChar = 'i';

        public static void Run()
        {
            Console.WriteLine($@"{nameof(Vector)}.{nameof(Vector.IsHardwareAccelerated)}: {Vector.IsHardwareAccelerated}");
            Console.WriteLine($@"{nameof(TestTextData)}.{nameof(TestTextData.Length)}: {TestTextData.Length:N0}");
            Console.WriteLine($@"{nameof(SearchChar)}: '{SearchChar}'");

            Console.WriteLine();
            Console.WriteLine(@"* Warming up and validating results.");
            var valueOfCountUsingLinqAndLambda = TestTextData.CountUsingLinqAndLambda(SearchChar);
            Console.WriteLine($@"{nameof(StringHelper.CountUsingLinqAndLambda).PadRight(32)}: {valueOfCountUsingLinqAndLambda:N0}");

            var valueOfCountUsingLinqAndLocalFunction = TestTextData.CountUsingLinqAndLocalFunction(SearchChar);
            Console.WriteLine($@"{nameof(StringHelper.CountUsingLinqAndLocalFunction).PadRight(32)}: {valueOfCountUsingLinqAndLocalFunction:N0}");
            Trace.Assert(valueOfCountUsingLinqAndLocalFunction == valueOfCountUsingLinqAndLambda);

            var valueOfCountUsingForEach = TestTextData.CountUsingForEach(SearchChar);
            Console.WriteLine($@"{nameof(StringHelper.CountUsingForEach).PadRight(32)}: {valueOfCountUsingForEach:N0}");
            Trace.Assert(valueOfCountUsingForEach == valueOfCountUsingLinqAndLambda);

            var valueOfCountUsingForEachButNoBranching = TestTextData.CountUsingForEachButNoBranching(SearchChar);
            Console.WriteLine($@"{nameof(StringHelper.CountUsingForEachButNoBranching).PadRight(32)}: {valueOfCountUsingForEachButNoBranching:N0}");
            Trace.Assert(valueOfCountUsingForEachButNoBranching == valueOfCountUsingLinqAndLambda);

            var valueOfCountUsingSimd = TestTextData.CountUsingSimd(SearchChar);
            Console.WriteLine($@"{nameof(StringHelper.CountUsingSimd).PadRight(32)}: {valueOfCountUsingSimd:N0}");
            Trace.Assert(valueOfCountUsingSimd == valueOfCountUsingLinqAndLambda);

            //// Tests
            RunPerformanceTests(10);
            RunPerformanceTests(50);
            RunPerformanceTests(100);
            RunPerformanceTests(500);
            RunPerformanceTests(1000);
        }

        private static void RunPerformanceTests(int iterationCount)
        {
            Console.WriteLine();
            Console.WriteLine($@"* Running performance tests (iteration count: {iterationCount}).");

            TestCountUsingLinqAndLambda(iterationCount);
            TestCountUsingLinqAndLocalFunction(iterationCount);
            TestCountUsingForEach(iterationCount);
            TestCountUsingForEachButNoBranching(iterationCount);
            TestCountUsingSimd(iterationCount);
        }

        private static void TestCountUsingLinqAndLambda(int iterationCount)
        {
            var totalCount = 0;

            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < iterationCount; index++)
            {
                unchecked
                {
                    totalCount += TestTextData.CountUsingLinqAndLambda(SearchChar);
                }
            }
            stopwatch.Stop();

            Trace.Assert(totalCount != 0);  // Just using the variable

            Console.WriteLine(
                $@"{nameof(StringHelper.CountUsingLinqAndLambda).PadRight(32)}: {stopwatch.ElapsedMilliseconds:N0} ms ({stopwatch.Elapsed})");
        }

        private static void TestCountUsingLinqAndLocalFunction(int iterationCount)
        {
            var totalCount = 0;

            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < iterationCount; index++)
            {
                unchecked
                {
                    totalCount += TestTextData.CountUsingLinqAndLocalFunction(SearchChar);
                }
            }
            stopwatch.Stop();

            Trace.Assert(totalCount != 0);  // Just using the variable

            Console.WriteLine(
                $@"{nameof(StringHelper.CountUsingLinqAndLocalFunction).PadRight(32)}: {stopwatch.ElapsedMilliseconds:N0} ms ({stopwatch.Elapsed})");
        }

        private static void TestCountUsingForEach(int iterationCount)
        {
            var totalCount = 0;

            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < iterationCount; index++)
            {
                unchecked
                {
                    totalCount += TestTextData.CountUsingForEach(SearchChar);
                }
            }
            stopwatch.Stop();

            Trace.Assert(totalCount != 0);  // Just using the variable

            Console.WriteLine(
                $@"{nameof(StringHelper.CountUsingForEach).PadRight(32)}: {stopwatch.ElapsedMilliseconds:N0} ms ({stopwatch.Elapsed})");
        }

        private static void TestCountUsingForEachButNoBranching(int iterationCount)
        {
            var totalCount = 0;

            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < iterationCount; index++)
            {
                unchecked
                {
                    totalCount += TestTextData.CountUsingForEachButNoBranching(SearchChar);
                }
            }
            stopwatch.Stop();

            Trace.Assert(totalCount != 0);  // Just using the variable

            Console.WriteLine(
                $@"{nameof(StringHelper.CountUsingForEachButNoBranching).PadRight(32)}: {stopwatch.ElapsedMilliseconds:N0} ms ({stopwatch.Elapsed})");
        }

        private static void TestCountUsingSimd(int iterationCount)
        {
            var totalCount = 0;

            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < iterationCount; index++)
            {
                unchecked
                {
                    totalCount += TestTextData.CountUsingSimd(SearchChar);
                }
            }
            stopwatch.Stop();

            Trace.Assert(totalCount != 0);  // Just using the variable

            Console.WriteLine(
                $@"{nameof(StringHelper.CountUsingSimd).PadRight(32)}: {stopwatch.ElapsedMilliseconds:N0} ms ({stopwatch.Elapsed})");
        }
    }
}