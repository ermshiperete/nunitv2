// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// TestCaseBuilderCollection is an ExtensionPoint for TestCaseBuilders 
	/// and implements the ITestCaseBuilder interface itself, passing calls 
	/// on to the individual builders.
	/// 
	/// The builders are added to the collection by inserting them at
	/// the start, as to take precedence over those added earlier. 
	/// </summary>
	public class TestCaseBuilderCollection : ExtensionPoint, ITestCaseBuilder
	{
		#region Constructor
		public TestCaseBuilderCollection(IExtensionHost host)
			: base("TestCaseBuilders", host) { }
		#endregion
		
		#region ITestCaseBuilder Members

		/// <summary>
		/// Examine the method and determine if it is suitable for
		/// any TestCaseBuilder to use in building a Test
		/// </summary>
		/// <param name="method">The method to be used as a test case</param>
		/// <returns>True if the method can be used to build a Test</returns>
		public bool CanBuildFrom( MethodInfo method, Test suite )
		{
			foreach( ITestCaseBuilder builder in Extensions )
				if ( builder.CanBuildFrom( method, suite ) )
					return true;
			return false;
		}

		/// <summary>
		/// Build a Test from the method provided.
		/// </summary>
		/// <param name="method">The method to be used</param>
		/// <returns>A Test or null</returns>
		public Test BuildFrom( MethodInfo method, Test suite )
		{
			foreach( ITestCaseBuilder builder in Extensions )
			{
				if ( builder.CanBuildFrom( method, suite ) )
					return builder.BuildFrom( method, suite );
			}

			return null;
		}
		#endregion

		#region ExtensionPoint Overrides
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestCaseBuilder; 
		}
		#endregion
	}
}
