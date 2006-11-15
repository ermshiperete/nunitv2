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

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
	/// <summary>
	/// Class that builds a TestAssembly suite from an assembly
	/// </summary>
	public class TestAssemblyBuilder
	{
		#region Instance Fields
		/// <summary>
		/// The loaded assembly
		/// </summary>
		Assembly assembly;

		/// <summary>
		/// Our LegacySuite builder, which is only used when a 
		/// fixture has been passed by name on the command line.
		/// </summary>
		ISuiteBuilder legacySuiteBuilder;

		/// <summary>
		/// Set to true to build automatic namespace suites,
		/// false to build a single suite with the fixtures.
		/// </summary>
		bool autoNamespaceSuites = true;

		private TestAssemblyInfo assemblyInfo = null;

		#endregion

		#region Properties

		public bool AutoNamespaceSuites
		{
			get { return autoNamespaceSuites; }
			set { autoNamespaceSuites = value; }
		}

		public Assembly Assembly
		{
			get { return assembly; }
		}

		public TestAssemblyInfo AssemblyInfo
		{
			get 
			{ 
				if ( assemblyInfo == null && assembly != null )
				{
					string path = new Uri( assembly.GetName().CodeBase ).LocalPath;
					AssemblyReader rdr = new AssemblyReader( path );
					Version runtimeVersion = new Version( rdr.ImageRuntimeVersion.Substring( 1 ) );
					IList frameworks = CoreExtensions.Current.TestFrameworks.GetReferencedFrameworks( assembly );
					assemblyInfo = new TestAssemblyInfo( path, runtimeVersion, frameworks );
				}

				return assemblyInfo;
			}
		}

		#endregion

		#region Constructor

		public TestAssemblyBuilder()
		{
			// TODO: Keeping this separate till we can make
			//it work in all situations.
			legacySuiteBuilder = new NUnit.Core.Builders.LegacySuiteBuilder();
		}

		#endregion

		#region Other Public Methods

		public Test Build( string assemblyName, string testName )
		{
			if ( testName == null || testName == string.Empty )
				return Build( assemblyName );

			this.assembly = Load( assemblyName );
			if ( assembly == null ) return null;

			// If provided test name is actually a fixture,
			// just build and return that!
			Type testType = assembly.GetType(testName);
			if( testType != null )
				return BuildSingleFixture( testType );
		
			// Assume that testName is a namespace and get all fixtures in it
			IList fixtures = GetFixtures( assembly, testName );
			if ( fixtures.Count > 0 ) 
				return BuildTestAssembly( assemblyName, fixtures );
			return null;
		}

		public TestSuite Build( string assemblyName )
		{
			this.assembly = Load( assemblyName );
			if ( this.assembly == null ) return null;

			IList fixtures = GetFixtures( assembly, null );
			return BuildTestAssembly( assemblyName, fixtures );
		}

		private TestSuite BuildTestAssembly( string assemblyName, IList fixtures )
		{
			TestSuite testAssembly = new TestSuite( assemblyName );

			if ( autoNamespaceSuites )
			{
				NamespaceTreeBuilder treeBuilder = 
					new NamespaceTreeBuilder( testAssembly );
				treeBuilder.Add( fixtures );
                testAssembly = treeBuilder.RootSuite;
			}
			else 
			foreach( TestSuite fixture in fixtures )
			{
				testAssembly.Add( fixture );
			}

			if ( fixtures.Count == 0 )
			{
				testAssembly.RunState = RunState.NotRunnable;
				testAssembly.IgnoreReason = "Has no TestFixtures";
			}
			
            NUnitFramework.ApplyCommonAttributes( assembly, testAssembly );

			// TODO: Make this an option? Add Option to sort assemblies as well?
			testAssembly.Sort();

			return testAssembly;
		}

		#endregion

		#region Nested TypeFilter Class

//		private class TypeFilter
//		{
//			private string rootNamespace;
//
//			TypeFilter( string rootNamespace ) 
//			{
//				this.rootNamespace = rootNamespace;
//			}
//
//			public bool Include( Type type )
//			{
//				if ( type.Namespace == rootNamespace )
//					return true;
//
//				return type.Namespace.StartsWith( rootNamespace + '.' );
//			}
//		}

		#endregion

		#region Helper Methods

		private Assembly Load(string path)
		{
			// Change currentDirectory in case assembly references unmanaged dlls
			using( new DirectorySwapper( Path.GetDirectoryName( path ) ) )
			{
                // Under .Net 2.0, throw if this isn't a managed assembly
				AssemblyName.GetAssemblyName( path );

                Assembly assembly = Assembly.Load(Path.GetFileNameWithoutExtension(path));
				
				if ( assembly != null )
					AddinManager.CurrentManager.Register( assembly );

				return assembly;
			}
		}

		private IList GetFixtures( Assembly assembly, string ns )
		{
			ArrayList fixtures = new ArrayList();

			IList testTypes = GetCandidateFixtureTypes( assembly, ns );
			foreach(Type testType in testTypes)
			{
				if( TestFixtureBuilder.CanBuildFrom( testType ) )
					fixtures.Add( TestFixtureBuilder.BuildFrom( testType ) );
			}

			return fixtures;
		}

		private Test BuildSingleFixture( Type testType )
		{
			// The only place we currently allow legacy suites
			if ( legacySuiteBuilder.CanBuildFrom( testType ) )
				return legacySuiteBuilder.BuildFrom( testType );

			return TestFixtureBuilder.BuildFrom( testType );
		}
		
		private IList GetCandidateFixtureTypes( Assembly assembly, string ns )
		{
			IList types = assembly.GetTypes();
				
			if ( ns == null || ns == string.Empty || types.Count == 0 ) 
				return types;

			string prefix = ns + "." ;
			
			ArrayList result = new ArrayList();
			foreach( Type type in types )
				if ( type.FullName.StartsWith( prefix ) )
					result.Add( type );

			return result;
		}

		#endregion
	}
}
