using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Class to assert that the actual value is greater than the expected value.
    /// </summary>
    public class GreaterEqualAsserter : ComparisonAsserter
    {
        /// <summary>
        /// Constructs a GreaterAsserter for two objects implementing IComparable
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to issue on failure</param>
        /// <param name="args">Arguments to apply in formatting the message</param>
        public GreaterEqualAsserter(IComparable expected, IComparable actual, string message, params object[] args)
            : base(expected, actual, message, args) { }

        /// <summary>
        /// Test whether the actual is greater than the expected, building up
        /// the failure message for later use if they are not.
        /// </summary>
        /// <returns>True if actual is greater than expected</returns>
        public override bool Test()
        {
            if (((IComparable)actual).CompareTo(expected) >= 0) return true;

            DisplayDifferences();
            return false;
        }


        private void DisplayDifferences()
        {
            FailureMessage.WriteExpectedLine(string.Format("Value greater than or equal to {0}", expected));
            FailureMessage.WriteActualLine(actual.ToString());
        }
    }
}


