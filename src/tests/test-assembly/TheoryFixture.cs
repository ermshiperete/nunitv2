﻿using System;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class TheoryFixture
    {
        [Datapoint]
        private int i0 = 0;
        [Datapoint]
        static int i1 = 1;
        [Datapoint]
        public int i100 = 100;

        private void Dummy()
        {
            int x = i0; // Suppress Compiler Warnings
            int y = i1; //
        }

        [Theory]
        public void TheoryWithNoArguments()
        {
        }

        [Theory]
        public void TheoryWithArgumentsButNoDatapoints(decimal x, decimal y)
        {
        }

        [Theory]
        public void TheoryWithArgumentsAndDatapoints(int x, int y)
        {
        }

        [TestCase(5, 10)]
        [TestCase(3, 12)]
        public void TestWithArguments(int x, int y)
        {
        }
    }
}
