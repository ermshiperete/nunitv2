// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Text;
using System.Xml;
using System.IO;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests
{
	[TestFixture]
	public class NUnitProjectSave
	{
		static readonly string xmlfile = "test.nunit";

        private ProjectModel project;

		[SetUp]
		public void SetUp()
		{
            project = new ProjectModel();
            project.CreateNewProject();
		}

		[TearDown]
		public void TearDown()
		{
			if ( File.Exists( xmlfile ) )
				File.Delete( xmlfile );
		}

		[Test]
		public void EmptyProject()
		{
			CheckContents( NUnitProjectXml.EmptyProject );
		}

		[Test]
		public void EmptyConfigs()
		{
            project.Configs.Add("Debug");
            project.Configs.Add("Release");
            project.ActiveConfigName = "Debug";
            project.Configs["Debug"].BinPathType = BinPathType.Auto;
            project.Configs["Release"].BinPathType = BinPathType.Auto;

			CheckContents( NUnitProjectXml.EmptyConfigs );			
		}

        [Test]
        public void NormalProject()
        {
            ProjectConfig config1 = project.Configs.Add("Debug");
            config1.BasePath = "bin" + Path.DirectorySeparatorChar + "debug";
            config1.BinPathType = BinPathType.Auto;
            config1.Assemblies.Add("assembly1.dll");
            config1.Assemblies.Add("assembly2.dll");

            ProjectConfig config2 = project.Configs.Add("Release");
            config2.BasePath = "bin" + Path.DirectorySeparatorChar + "release";
            config2.BinPathType = BinPathType.Auto;
            config2.Assemblies.Add("assembly1.dll");
            config2.Assemblies.Add("assembly2.dll");

            project.ActiveConfigName = "Debug";

            CheckContents(NUnitProjectXml.NormalProject);
        }

        [Test]
        public void NormalProject_RoundTrip()
        {
            project.LoadXml(NUnitProjectXml.NormalProject);
            CheckContents(NUnitProjectXml.NormalProject);
        }

        [Test]
        public void ProjectWithComplexSettings()
        {
            ProjectConfig config1 = project.Configs.Add("Debug");
            config1.BasePath = "debug";
            config1.BinPathType = BinPathType.Auto;
            config1.RuntimeFramework = new RuntimeFramework(RuntimeType.Any, new Version(2, 0));
            config1.Assemblies.Add("assembly1.dll");
            config1.Assemblies.Add("assembly2.dll");

            ProjectConfig config2 = project.Configs.Add("Release");
            config2.BasePath = "release";
            config2.BinPathType = BinPathType.Auto;
            config2.RuntimeFramework = new RuntimeFramework(RuntimeType.Any, new Version(4, 0));
            config2.Assemblies.Add("assembly1.dll");
            config2.Assemblies.Add("assembly2.dll");

            project.ActiveConfigName = "Release";
            project.BasePath = "bin";
            project.ProcessModel = ProcessModel.Separate;
            project.DomainUsage = DomainUsage.Multiple;

            CheckContents(NUnitProjectXml.ComplexSettingsProject);
        }

        [Test]
        public void ProjectWithComplexSettings_RoundTrip()
        {
            project.LoadXml(NUnitProjectXml.ComplexSettingsProject);
            CheckContents(NUnitProjectXml.ComplexSettingsProject);
        }

        [Test]
        public void ProjectWithComplexSettings_RoundTripWithChanges()
        {
            project.LoadXml(NUnitProjectXml.ComplexSettingsProject);
            project.ProcessModel = ProcessModel.Single;
            CheckContents(NUnitProjectXml.ComplexSettingsProject
                .Replace("Separate", "Single"));
        }

        private void CheckContents(string expected)
        {
            project.Save(xmlfile);
            StreamReader reader = new StreamReader(xmlfile);
            string contents = reader.ReadToEnd();
            reader.Close();
            Assert.That(contents, Is.EqualTo(expected));
        }
    }
}
