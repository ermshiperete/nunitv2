#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig
/************************************************************************************
'
' Copyright � 2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' Copyright � 2000-2003 Philip A. Craig
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' Portions Copyright � 2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' or Copyright � 2000-2003 Philip A. Craig
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'***********************************************************************************/
#endregion

namespace NUnit.Core
{
	using System;
	using System.Collections;
	using System.Reflection;
	using NUnit.Core.Filters;

	/// <summary>
	/// Summary description for TestSuite.
	/// </summary>
	/// 
	[Serializable]
	public class TestSuite : Test
	{
		#region Fields
		/// <summary>
		/// Our collection of child tests
		/// </summary>
		private ArrayList tests = new ArrayList();

		/// <summary>
		/// The fixture setup method for this suite
		/// </summary>
		protected MethodInfo fixtureSetUp;

		/// <summary>
		/// The fixture teardown method for this suite
		/// </summary>
		protected MethodInfo fixtureTearDown;

		#endregion

		#region Constructors
		public TestSuite( string name ) 
			: base( name ) { }

		public TestSuite( string parentSuiteName, string name ) 
			: base( parentSuiteName, name ) { }

		public TestSuite( Type fixtureType )
			: base( fixtureType ) { }
		#endregion

		#region Public Methods
		public void Sort()
		{
			this.tests.Sort();

			foreach( Test test in Tests )
			{
				TestSuite suite = test as TestSuite;
				if ( suite != null )
					suite.Sort();
			}		
		}

		public void Sort(IComparer comparer)
		{
			this.tests.Sort(comparer);

			foreach( Test test in Tests )
			{
				TestSuite suite = test as TestSuite;
				if ( suite != null )
					suite.Sort(comparer);
			}
		}

		public void Add( Test test ) 
		{
//			if( test.RunState == RunState.Runnable )
//			{
//				test.RunState = this.RunState;
//				test.IgnoreReason = this.IgnoreReason;
//			}
			test.Parent = this;
			tests.Add(test);
		}

		public void Add( object fixture )
		{
			Test test = TestFixtureBuilder.BuildFrom( fixture );
			if ( test != null )
				Add( test );
		}
		#endregion

		#region Properties
		public override IList Tests 
		{
			get { return tests; }
		}

		public override bool IsSuite
		{
			get { return true; }
		}

		public override int TestCount
		{
			get
			{
				int count = 0;

				foreach(Test test in Tests)
				{
					count += test.TestCount;
				}
				return count;
			}
		}
		#endregion

		#region Test Overrides
		public override string TestType
		{
			get	{ return "Test Suite"; }
		}

		public override int CountTestCases(ITestFilter filter)
		{
			int count = 0;

			if(filter.Pass(this)) 
			{
				foreach(Test test in Tests)
				{
					count += test.CountTestCases(filter);
				}
			}
			return count;
		}

		public override TestResult Run(EventListener listener)
		{
			return Run( listener, TestFilter.Empty );
		}

		public override TestResult Run(EventListener listener, ITestFilter filter)
		{
			using( new TestContext() )
			{
				TestSuiteResult suiteResult = new TestSuiteResult( new TestInfo(this), TestName.Name);

				listener.SuiteStarted( this.TestName );
				long startTime = DateTime.Now.Ticks;

				switch (this.RunState)
				{
					case RunState.Runnable:
					case RunState.Explicit:
						suiteResult.RunState = RunState.Executed;
						DoOneTimeSetUp(suiteResult);
						if ( suiteResult.IsFailure )
							MarkTestsFailed(Tests, suiteResult, listener, filter);
						else
						{
							RunAllTests(suiteResult, listener, filter);
							DoOneTimeTearDown(suiteResult);
						}
						break;

					case RunState.Skipped:
						suiteResult.Skip(this.IgnoreReason);
						MarkTestsNotRun(Tests, RunState.Skipped, IgnoreReason, suiteResult, listener, filter);
						break;

					default:
					case RunState.Ignored:
					case RunState.NotRunnable:
						suiteResult.Ignore(this.IgnoreReason);
						MarkTestsNotRun(Tests, RunState.Ignored, IgnoreReason, suiteResult, listener, filter);
						break;
				}

				long stopTime = DateTime.Now.Ticks;
				double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
				suiteResult.Time = time;

				listener.SuiteFinished(suiteResult);
				return suiteResult;
			}
		}
		#endregion

		#region Virtual Methods
        protected virtual void DoOneTimeSetUp(TestResult suiteResult)
        {
            if (FixtureType != null)
            {
                try
                {
                    if (Fixture == null) // In case TestFixture was created with fixture object
						CreateUserFixture();

                    if (this.fixtureSetUp != null)
                        Reflect.InvokeMethod(fixtureSetUp, Fixture);

                    //setUpStatus = SetUpState.SetUpComplete;
                    //setUpFailed = false;
                }
                catch (Exception ex)
                {
                    //NunitException nex = ex as NunitException;
                    if (ex is NunitException || ex is System.Reflection.TargetInvocationException)
                        ex = ex.InnerException;

                    if (IsIgnoreException(ex))
                    {
                        this.RunState = RunState.Ignored;
                        suiteResult.Ignore(ex.Message);
                        suiteResult.StackTrace = ex.StackTrace;
                        this.IgnoreReason = ex.Message;
                    }
                    else 
                    {
                        if (IsAssertException(ex))
                            suiteResult.Failure(ex.Message, ex.StackTrace);
                        else
                            suiteResult.Error(ex);
                       
                        //setUpStatus = SetUpState.SetUpFailed;
                        //setUpFailed = true;
                    }
                }
            }
        }

		protected virtual void CreateUserFixture()
		{
			Fixture = Reflect.Construct(FixtureType);
		}

        protected virtual void DoOneTimeTearDown(TestResult suiteResult)
        {
            //setUpStatus = SetUpState.SetUpNeeded;
            //setUpFailed = false;
            
            if ( this.RunState == RunState.Runnable && this.Fixture != null)
            {
                try
                {
                    if (this.fixtureTearDown != null)
                        Reflect.InvokeMethod(fixtureTearDown, Fixture);
                }
                catch (Exception ex)
                {
                    // Error in TestFixtureTearDown causes the
                    // suite to be marked as a failure, even if
                    // all the contained tests passed.
                    NunitException nex = ex as NunitException;
                    if (nex != null)
                        ex = nex.InnerException;


                    suiteResult.Failure(ex.Message, ex.StackTrace);
                }
                finally
                {
                    IDisposable disposeable = Fixture as IDisposable;
                    if (disposeable != null)
                        disposeable.Dispose();
                    this.Fixture = null;
                }
            }
        }
        
        private void RunAllTests(
			TestSuiteResult suiteResult, EventListener listener, ITestFilter filter )
		{
            foreach (Test test in ArrayList.Synchronized(Tests))
            {
                if (filter.Pass(test))
                {
                    RunState saveRunState = test.RunState;

                    if (test.RunState == RunState.Runnable && this.RunState != RunState.Runnable && this.RunState != RunState.Explicit )
                    {
                        test.RunState = this.RunState;
                        test.IgnoreReason = this.IgnoreReason;
                    }

                    TestResult result = test.Run(listener, filter);

                    suiteResult.AddResult(result);

                    if (saveRunState != test.RunState)
                    {
                        test.RunState = saveRunState;
                        test.IgnoreReason = null;
                    }
                }
            }
		}

        private void MarkTestsNotRun(
            IList tests, RunState runState, string ignoreReason, TestSuiteResult suiteResult, EventListener listener, ITestFilter filter)
        {
            foreach (Test test in ArrayList.Synchronized(tests))
            {
                if (filter.Pass(test))
                    MarkTestNotRun(test, runState, ignoreReason, suiteResult, listener, filter);
            }
        }

        private void MarkTestNotRun(
            Test test, RunState runState, string ignoreReason, TestSuiteResult suiteResult, EventListener listener, ITestFilter filter)
        {
            if (test is TestSuite)
            {
                listener.SuiteStarted(test.TestName);
                TestSuiteResult result = new TestSuiteResult( new TestInfo(test), test.TestName.FullName);
				result.NotRun( runState, ignoreReason, null );
                MarkTestsNotRun(test.Tests, runState, ignoreReason, suiteResult, listener, filter);
                suiteResult.AddResult(result);
                listener.SuiteFinished(result);
            }
            else
            {
                listener.TestStarted(test.TestName);
                TestCaseResult result = new TestCaseResult( new TestInfo(test) );
                result.NotRun( runState, ignoreReason, null );
                suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
        }

        private void MarkTestsFailed(
            IList tests, TestSuiteResult suiteResult, EventListener listener, ITestFilter filter)
        {
            foreach (Test test in ArrayList.Synchronized(tests))
                if (filter.Pass(test))
                    MarkTestFailed(test, suiteResult, listener, filter);
        }

        private void MarkTestFailed(
            Test test, TestSuiteResult suiteResult, EventListener listener, ITestFilter filter)
        {
            if (test is TestSuite)
            {
                listener.SuiteStarted(test.TestName);
                TestSuiteResult result = new TestSuiteResult( new TestInfo(test), test.TestName.FullName);
                result.Failure("Parent SetUp Failed", null, FailureSite.Parent);
                MarkTestsFailed(test.Tests, suiteResult, listener, filter);
                suiteResult.AddResult(result);
                listener.SuiteFinished(result);
            }
            else
            {
                listener.TestStarted(test.TestName);
                TestCaseResult result = new TestCaseResult( new TestInfo(test) );
                result.Failure("TestFixtureSetUp Failed", null, FailureSite.Parent);
                suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
        }

        protected virtual bool IsAssertException(Exception ex)
		{
            return ex.GetType().FullName == NUnitFramework.AssertException;
		}

		protected virtual bool IsIgnoreException(Exception ex)
		{
            return ex.GetType().FullName == NUnitFramework.IgnoreException;
		}
		#endregion
	}
}
