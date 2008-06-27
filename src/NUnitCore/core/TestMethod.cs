// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org/?p=license&r=2.4.
// ****************************************************************

namespace NUnit.Core
{
	using System;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Reflection;

	/// <summary>
	/// The TestMethod class represents a Test implemented as a method.
	/// 
	/// Because of how exceptions are handled internally, this class
	/// must incorporate processing of expected exceptions. A change to
	/// the Test interface might make it easier to process exceptions
	/// in an object that aggregates a TestMethod in the future.
	/// </summary>
	public abstract class TestMethod : Test
	{
		#region Fields
		/// <summary>
		/// The test method
		/// </summary>
		private MethodInfo method;

		/// <summary>
		/// The SetUp method.
		/// </summary>
		protected MethodInfo setUpMethod;

		/// <summary>
		/// The teardown method
		/// </summary>
		protected MethodInfo tearDownMethod;

        /// <summary>
        /// The ExpectedExceptionProcessor for this test, if any
        /// </summary>
        internal ExpectedExceptionProcessor exceptionProcessor;

        /// <summary>
        /// Arguments to be used in invoking the method
        /// </summary>
	    internal object[] arguments;

        /// <summary>
        /// The expected result of the method return value
        /// </summary>
	    internal object expectedResult;

		#endregion

		#region Constructors
		public TestMethod( MethodInfo method ) 
			: base( method ) 
		{
			this.method = method;
		}
		#endregion

		#region Properties
		public MethodInfo Method
		{
			get { return method; }
		}

        public ExpectedExceptionProcessor ExceptionProcessor
        {
            get { return exceptionProcessor; }
            set { exceptionProcessor = value; }
        }

		public bool ExceptionExpected
		{
            get { return exceptionProcessor != null; }
		}

		#endregion

		#region Run Methods
        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            using (new TestContext())
            {
                TestResult testResult = new TestResult(this);

                listener.TestStarted(this.TestName);
                long startTime = DateTime.Now.Ticks;

                switch (this.RunState)
                {
                    case RunState.Runnable:
                    case RunState.Explicit:
                        Run(testResult);
                        break;
                    case RunState.Skipped:
                    default:
                        testResult.Skip(IgnoreReason);
                        break;
                    case RunState.NotRunnable:
                        testResult.Invalid(IgnoreReason);
                        break;
                    case RunState.Ignored:
                        testResult.Ignore(IgnoreReason);
                        break;
                }

                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                testResult.Time = time;

                listener.TestFinished(testResult);
                return testResult;
            }
        }
        
        public virtual void Run(TestResult testResult)
		{
            try
            {
                if (this.Parent != null)
                    Fixture = this.Parent.Fixture;

                // Temporary... to allow for tests that directly execute a test case
                if (Fixture == null && !method.IsStatic)
                    Fixture = Reflect.Construct(this.FixtureType);

                if (this.Properties["_SETCULTURE"] != null)
                    TestContext.CurrentCulture =
                        new System.Globalization.CultureInfo((string)Properties["_SETCULTURE"]);

                doRun(testResult);
            }
            catch (Exception ex)
            {
                if (ex is NUnitException)
                    ex = ex.InnerException;

                RecordException(ex, testResult);
            }
            finally
            {
                Fixture = null;
            }
		}

		/// <summary>
		/// The doRun method is used to run a test internally.
		/// It assumes that the caller is taking care of any 
		/// TestFixtureSetUp and TestFixtureTearDown needed.
		/// </summary>
		/// <param name="testResult">The result in which to record success or failure</param>
		public virtual void doRun( TestResult testResult )
		{
			DateTime start = DateTime.Now;

			try 
			{
				if ( setUpMethod != null )
					Reflect.InvokeMethod( setUpMethod, setUpMethod.IsStatic ? null : this.Fixture );

				doTestCase( testResult );
			}
			catch(Exception ex)
			{
				if ( ex is NUnitException )
					ex = ex.InnerException;

				RecordException( ex, testResult );
			}
			finally 
			{
				doTearDown( testResult );

				DateTime stop = DateTime.Now;
				TimeSpan span = stop.Subtract(start);
				testResult.Time = (double)span.Ticks / (double)TimeSpan.TicksPerSecond;
			}
		}
		#endregion

		#region Invoke Methods by Reflection, Recording Errors

		private void doTearDown( TestResult testResult )
		{
			try
			{
				if ( tearDownMethod != null )
					tearDownMethod.Invoke( this.Fixture, new object[0] );
			}
			catch(Exception ex)
			{
				if ( ex is NUnitException )
					ex = ex.InnerException;
				// TODO: What about ignore exceptions in teardown?
				testResult.Error( ex,FailureSite.TearDown );
			}
		}

		private void doTestCase( TestResult testResult )
		{
			try
			{
				RunTestMethod(testResult);
                if ( testResult.IsSuccess && exceptionProcessor != null)
				    exceptionProcessor.ProcessNoException(testResult);
			}
			catch( Exception ex )
			{
                if (exceptionProcessor == null)
                    RecordException(ex, testResult);
                else
                    exceptionProcessor.ProcessException(ex, testResult);
			}
		}

		public virtual void RunTestMethod(TestResult testResult)
		{
		    object fixture = this.method.IsStatic ? null : this.Fixture;
			object result = Reflect.InvokeMethod( this.method, fixture, this.arguments );

            if (this.expectedResult != null)
                NUnitFramework.Assert.AreEqual(expectedResult, result);

            testResult.Success();
        }

		#endregion

		#region Record Info About An Exception
		protected void RecordException( Exception exception, TestResult testResult )
		{
            if (exception is NUnitException)
                exception = exception.InnerException;

            testResult.SetResult(NUnitFramework.GetResultState(exception), exception);
		}
		#endregion
    }
}
