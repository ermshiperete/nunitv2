using System;
using NUnit.Framework;
using NUnit.TestData;
using NUnit.TestUtilities;
using System.Collections;

namespace NUnit.Core.Tests
{
    [TestFixture]
    public class TestCaseAttributeTests
    {
        [TestCase(12, 3, 4)]
        [TestCase(12, 2, 6)]
        [TestCase(12, 4, 3)]
        [TestCase(12, 0, 0, ExpectedException = typeof(System.DivideByZeroException))]
        [TestCase(12, 0, 0, ExpectedExceptionName = "System.DivideByZeroException")]
        public void IntegerDivisionWithResultPassedToTest(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCase(12, 3, Result = 4)]
        [TestCase(12, 2, Result = 6)]
        [TestCase(12, 4, Result = 3)]
        [TestCase(12, 0, ExpectedException = typeof(System.DivideByZeroException))]
        [TestCase(12, 0, ExpectedExceptionName = "System.DivideByZeroException",
            TestName = "DivisionByZeroThrowsException")]
        public int IntegerDivisionWithResultCheckedByNUnit(int n, int d)
        {
            return n / d;
        }

        [TestCase(2, 2, Result=4)]
        public double CanConvertIntToDouble(double x, double y)
        {
            return x + y;
        }

        [TestCase("2.2", "3.3", Result = 5.5)]
        public decimal CanConvertStringToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [TestCase(2.2, 3.3, Result = 5.5)]
        public decimal CanConvertDoubleToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [Test]
		public void ConversionOverflowGivesError()
		{
			Test test = (Test)TestBuilder.MakeTestCase(
				typeof(TestCaseAttributeFixture), "MethodCausesConversionOverflow").Tests[0];
			Assert.AreEqual(RunState.Runnable, test.RunState);
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Error, result.ResultState);
		}

        [TestCase("12-October-1942")]
        public void CanConvertStringToDateTime(DateTime dt)
        {
            Assert.AreEqual(1942, dt.Year);
        }

        [TestCase(42, ExpectedException = typeof(System.Exception),
                   ExpectedMessage = "Test Exception")]
        public void CanSpecifyExceptionMessage(int a)
        {
        	throw new System.Exception("Test Exception");
        }
         
        [TestCase(null)]
        public void CanPassNullAsFirstArgument(object a)
        {
        	Assert.IsNull(a);
        }

        [TestCase(new object[] { 1, "two", 3.0 })]
        [TestCase(new object[] { "zip" })]
        public void CanPassObjectArrayAsFirstArgument(object[] a)
        {
        }
  
        [TestCase(new object[] { "a", "b" })]
        public void CanPassArrayAsArgument(object[] array)
        {
            Assert.AreEqual("a", array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase("a", "b")]
        public void ArgumentsAreCoalescedInObjectArray(object[] array)
        {
            Assert.AreEqual("a", array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase(1, "b")]
        public void ArgumentsOfDifferentTypeAreCoalescedInObjectArray(object[] array)
        {
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [Test]
        public void CanSpecifyDescription()
        {
			Test test = (Test)TestBuilder.MakeTestCase(
				typeof(TestCaseAttributeFixture), "MethodHasDescriptionSpecified").Tests[0];
			Assert.AreEqual("My Description", test.Description);
		}

        [Test]
        public void CanSpecifyTestName()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodHasTestNameSpecified").Tests[0];
            Assert.AreEqual("XYZ", test.TestName.Name);
            Assert.AreEqual("NUnit.TestData.TestCaseAttributeFixture.XYZ", test.TestName.FullName);
        }

        [Test]
        public void CanSpecifyExpectedException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsExpectedException").Tests[0];
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Success, result.ResultState);
        }

        [Test]
        public void CanSpecifyExpectedException_WrongException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsWrongException").Tests[0];
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            StringAssert.StartsWith("An unexpected exception type was thrown", result.Message);
        }

        [Test]
        public void CanSpecifyExpectedException_WrongMessage()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsExpectedExceptionWithWrongMessage").Tests[0];
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            StringAssert.StartsWith("The exception message text was incorrect", result.Message);
        }

        [Test]
        public void CanSpecifyExpectedException_NoneThrown()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsNoException").Tests[0];
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.AreEqual("System.ArgumentNullException was expected", result.Message);
        }

        [Test]
        public void IgnoreTakesPrecedenceOverExpectedException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodCallsIgnore").Tests[0];
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Ignored, result.ResultState);
            Assert.AreEqual("Ignore this", result.Message);
        }
    }
}
