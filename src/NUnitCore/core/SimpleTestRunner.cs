using System;
using System.IO;
using System.Threading;
using System.Collections;
using NUnit.Core.Filters;
using System.Reflection;

namespace NUnit.Core
{
	/// <summary>
	/// SimpleTestRunner is the simplest direct-running TestRunner. It
	/// passes the event listener interface that is provided on to the tests
	/// to use directly and does nothing to redirect text output. Both
	/// Run and BeginRun are actually synchronous, although the client
	/// can usually ignore this. BeginRun + EndRun operates as expected.
	/// </summary>
	public class SimpleTestRunner : MarshalByRefObject, TestRunner
	{
		#region Instance Variables

		/// <summary>
		/// Identifier for this runner. Must be unique among all
		/// active runners in order to locate tests. Default
		/// value of 0 is adequate in applications with a single
		/// runner or a non-branching chain of runners.
		/// </summary>
		private int runnerID = 0;

		/// <summary>
		/// The loaded test suite
		/// </summary>
		private Test test;

		/// <summary>
		/// The builder we use to load tests, created for each load
		/// </summary>
		private TestSuiteBuilder builder;

		/// <summary>
		/// Results from the last test run
		/// </summary>
		private TestResult testResult;

		/// <summary>
		/// The thread on which Run was called. Set to the
		/// current thread while a run is in process.
		/// </summary>
		private Thread runThread;

		/// <summary>
		/// The settings for this runner
		/// </summary>
		private TestRunnerSettings settings;

		#endregion

		#region Constructor
		public SimpleTestRunner() : this( 0 ) { }

		public SimpleTestRunner( int runnerID )
		{
			this.runnerID = runnerID;
			this.settings = new TestRunnerSettings( this );
		}
		#endregion

		#region Properties
		public virtual int ID
		{
			get { return runnerID; }
		}

		public IList Extensions
		{
			get { return AddinManager.CurrentManager.Names; }
		}

		public IList AssemblyInfo
		{
			get { return builder.AssemblyInfo; }
		}
		
		public TestNode Test
		{
			get { return test == null ? null : new TestNode( test ); }
		}

		/// <summary>
		/// Results from the last test run
		/// </summary>
		public TestResult TestResult
		{
			get { return testResult; }
		}

		public virtual bool Running
		{
			get { return runThread != null && runThread.IsAlive; }
		}

		public TestRunnerSettings Settings
		{
			get { return settings; }
		}
		#endregion

		#region Methods for Loading Tests

		/// <summary>
		/// Load an assembly
		/// </summary>
		/// <param name="assemblyName">The name of the assembly to load</param>
		/// <returns>True on success, false on failure</returns>
		public bool Load( string assemblyName )
		{
			return Load( assemblyName, string.Empty );
		}

		/// <summary>
		/// Load a particular test in an assembly
		/// </summary>
		/// <param name="assemblyName">The name of the assembly to load</param>
		/// <param name="testName">The name of the test fixture or suite to be loaded</param>
		/// <returns>True on success, false on failure</returns>
		public bool Load( string assemblyName, string testName )
		{
			TestSuiteBuilder builder = CreateBuilder();

            this.test = builder.Build( assemblyName, testName );
			if ( test == null ) return false;

			test.SetRunnerID( this.runnerID, true );
			return true;
		}

		/// <summary>
		/// Load the assemblies in a test project
		/// </summary>
		/// <param name="projectName">The name of the test project being loaded</param>
		/// <param name="assemblies">The assemblies comprising the project</param>
		/// <returns>True on success, false on failure</returns>
		public bool Load( string projectName, string[] assemblies )
		{
			return Load( projectName, assemblies, string.Empty );
		}

		/// <summary>
		/// Load a particular test in a TestProject.
		/// </summary>
		/// <param name="projectName">The name of the test project being loaded</param>
		/// <param name="assemblies">The assemblies comprising the project</param>
		/// <param name="testName">The name of the test fixture or suite to be loaded</param>
		/// <returns>True on success, false on failure</returns>
		public bool Load( string projectName, string[] assemblies, string testName )
		{
			TestSuiteBuilder builder = CreateBuilder();

            this.test = builder.Build( projectName, assemblies, testName );
			if ( test == null ) return false;

			test.SetRunnerID( this.runnerID, true );
			return true;
		}

		/// <summary>
		/// Unload all tests previously loaded
		/// </summary>
		public void Unload()
		{
			this.test = null; // All for now
		}
		#endregion

		#region CountTestCases
		public int CountTestCases( TestFilter filter )
		{
			return test.CountTestCases( filter );
		}
		#endregion

		#region Methods for Running Tests
		public virtual TestResult Run( EventListener listener )
		{
			return Run( listener, TestFilter.Empty );
		}

		public virtual TestResult Run( EventListener listener, TestFilter filter )
		{
			AddinManager.Save();

			try
			{
				// Take note of the fact that we are running
				this.runThread = Thread.CurrentThread;

				listener.RunStarted( this.Test.TestName.FullName, test.CountTestCases( filter ) );
				
				testResult = test.Run( listener, filter );

				// Signal that we are done
				listener.RunFinished( testResult );

				// Return result array
				return testResult;
			}
			catch( Exception exception )
			{
				// Signal that we finished with an exception
				listener.RunFinished( exception );
				// Rethrow - should we do this?
				throw;
			}
			finally
			{
				runThread = null;
				AddinManager.Restore();
			}
		}

		public void BeginRun( EventListener listener )
		{
			testResult = this.Run( listener );
		}

		public void BeginRun( EventListener listener, TestFilter filter )
		{
			testResult = this.Run( listener, filter );
		}

		public virtual TestResult EndRun()
		{
			return TestResult;
		}

		/// <summary>
		/// Wait is a NOP for SimpleTestRunner
		/// </summary>
		public virtual void Wait()
		{
		}

		public virtual void CancelRun()
		{
			if (this.runThread != null)
			{
				// Cancel Synchronous run only if on another thread
				if ( runThread == Thread.CurrentThread )
					throw new InvalidOperationException( "May not CancelRun on same thread that is running the test" );

				// Make a copy of runThread, which will be set to 
				// null when the thread terminates.
				Thread cancelThread = this.runThread;

				// Tell the thread to abort
				this.runThread.Abort();
				
				// Wake up the thread if necessary
				// Figure out if we need to do an interupt
				if ( (cancelThread.ThreadState & ThreadState.WaitSleepJoin ) != 0 )
					cancelThread.Interrupt();
			}
		}
		#endregion

		#region Helper Routines
		private TestSuiteBuilder CreateBuilder()
		{
			builder = new TestSuiteBuilder();

			if ( settings.Contains( "AutoNamespaceSuites" ) )
				builder.AutoNamespaceSuites = (bool)settings["AutoNamespaceSuites"];
			if ( settings.Contains( "MergeAssemblies" ) )
				builder.MergeAssemblies = (bool)settings["MergeAssemblies"];

			return builder;
		}
		#endregion

        #region InitializeLifetimeService Override
        public override object InitializeLifetimeService()
        {
            return null;
        }
	#endregion
	}
}
