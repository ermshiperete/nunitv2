// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org/?p=license&r=2.4.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NUnit.Core.Builders
{
	/// <summary>
	/// Built-in SuiteBuilder for NUnit TestFixture
	/// </summary>
	public class NUnitTestFixtureBuilder : Extensibility.ISuiteBuilder
	{
		#region Instance Fields
		/// <summary>
		/// The TestSuite being constructed;
		/// </summary>
		private TestSuite suite;

	    private Extensibility.ITestCaseBuilder testBuilders = CoreExtensions.Host.TestBuilders;

	    private Extensibility.ITestDecorator testDecorators = CoreExtensions.Host.TestDecorators;

		#endregion

        #region ISuiteBuilder Methods
        /// <summary>
		/// Checks to see if the fixture type has the TestFixtureAttribute
		/// </summary>
		/// <param name="type">The fixture type to check</param>
		/// <returns>True if the fixture can be built, false if not</returns>
		public bool CanBuildFrom(Type type)
		{
			return Reflect.HasAttribute( type, NUnitFramework.TestFixtureAttribute, true );
		}

		/// <summary>
		/// Build a TestSuite from type provided.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Test BuildFrom(Type type)
		{
#if NET_2_0
            if (type.IsGenericType)
                return BuildGenericFixture(type);
#endif
            return BuildSingleFixture(type);
        }

        private Test BuildSingleFixture(Type type)
        {
			this.suite = new NUnitTestFixture(type);

            string reason = null;
            if (!IsValidFixtureType(type, ref reason))
            {
                this.suite.RunState = RunState.NotRunnable;
                this.suite.IgnoreReason = reason;
            }

            NUnitFramework.ApplyCommonAttributes(type, suite);

			AddTestCases(type);

			if ( this.suite.RunState != RunState.NotRunnable && this.suite.TestCount == 0)
			{
				this.suite.RunState = RunState.NotRunnable;
				this.suite.IgnoreReason = suite.TestName.Name + " does not have any tests";
			}

			return this.suite;
		}
		#endregion

		#region Helper Methods
#if NET_2_0
        private Test BuildGenericFixture(Type type)
        {
            TestSuite suite = new TestSuite(type.Namespace, type.Name);

            Attribute[] attrs = Reflect.GetAttributes(type, NUnitFramework.TestFixtureAttribute, false);;
            if ( attrs.Length == 0 )
            {
                suite.RunState = RunState.NotRunnable;
                suite.IgnoreReason = "No type arguments supplied for generic test fixture";
                return suite;
            }

            foreach (Attribute attr in attrs)
            {
                Type[] typeArgs = (Type[])Reflect.GetPropertyValue(attr, "TypeArguments");
                suite.Add( BuildSingleFixture( type.MakeGenericType( typeArgs ) ) );
            }

            return suite;
        }
#endif

		/// <summary>
		/// Method to add test cases to the newly constructed suite.
		/// The default implementation looks at each candidate method
		/// and tries to build a test case from it. It will only need
		/// to be overridden if some other approach, such as reading a 
		/// datafile is used to generate test cases.
		/// </summary>
		/// <param name="fixtureType"></param>
		protected virtual void AddTestCases( Type fixtureType )
		{
			IList methods = fixtureType.GetMethods( 
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );

			foreach(MethodInfo method in methods)
			{
				Test test = BuildTestCase(method, this.suite);

				if(test != null)
				{
					this.suite.Add( test );
				}
			}
		}

		/// <summary>
		/// Method to create a test case from a MethodInfo and add
		/// it to the suite being built. It first checks to see if
		/// any global TestCaseBuilder addin wants to build the
		/// test case. If not, it uses the internal builder
		/// collection maintained by this fixture builder. After
		/// building the test case, it applies any decorators
		/// that have been installed.
		/// 
		/// The default implementation has no test case builders.
		/// Derived classes should add builders to the collection
		/// in their constructor.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		private Test BuildTestCase( MethodInfo method, TestSuite suite )
		{
            Test test = testBuilders.BuildFrom( method, suite );

			if ( test != null )
				test = testDecorators.Decorate( test, method );

			return test;
		}

		/// <summary>
        /// Check that the fixture is valid. In addition to the base class
        /// check for a valid constructor, this method ensures that there 
        /// is no more than one of each setup or teardown method and that
        /// their signatures are correct.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture to check</param>
        /// <param name="reason">A message indicating why the fixture is invalid</param>
        /// <returns>True if the fixture is valid, false if not</returns>
        private bool IsValidFixtureType(Type fixtureType, ref string reason)
        {
            if (!NUnitFramework.IsValidFixtureType(fixtureType, ref reason))
                return false;

            return NUnitFramework.IsSetUpMethodValid(fixtureType, ref reason)
                && NUnitFramework.IsTearDownMethodValid(fixtureType, ref reason)
                && NUnitFramework.IsFixtureSetUpMethodValid(fixtureType, ref reason)
                && NUnitFramework.IsFixtureTearDownMethodValid(fixtureType, ref reason);
        }
		#endregion
	}
}