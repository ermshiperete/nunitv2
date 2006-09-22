using System;
using System.Reflection;

namespace NUnit.Core
{
	/// <summary>
	/// Ignore Decorator is an alternative method of marking tests to
	/// be ignored. It is currently not used, since the test builders
	/// take care of the ignore attribute.
	/// </summary>
	public class IgnoreDecorator : ITestDecorator
	{
		public IgnoreDecorator( string ignoreAttributeType )
		{
		}

		#region ITestDecorator Members

		public TestCase Decorate(TestCase testCase, MethodInfo method)
		{
			return (TestCase)DecorateTest( testCase, method );
		}

		public TestSuite Decorate(TestSuite suite, Type fixtureType)
		{
			return (TestSuite)DecorateTest( suite, fixtureType );
		}

		private Test DecorateTest( Test test, MemberInfo member )
		{
			Attribute ignoreAttribute = Reflect.GetAttribute( member, NUnitFramework.IgnoreAttribute, false );

			if ( ignoreAttribute != null )
			{
				test.RunState = RunState.Ignored;
				test.IgnoreReason = NUnitFramework.GetIgnoreReason( ignoreAttribute );
			}

			return test;
		}

		#endregion
	}
}
