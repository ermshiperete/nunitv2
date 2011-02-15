﻿// ***********************************************************************
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

#if NET_3_5
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.ProjectEditor.ViewElements;
using NSubstitute;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class PropertyPresenterTests
    {
        IProjectDocument doc;
        private IProjectModel model;
        private IPropertyView view;
        private PropertyPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            doc = new ProjectDocument();
            doc.LoadXml(NUnitProjectXml.NormalProject);
            model = new ProjectModel(doc);
            model.ActiveConfigName = "Release";

            view = Substitute.For<IPropertyView>();
            view.ConfigList.Returns(new SelectionStub("ConfigList"));
            view.ProcessModel.Returns(new SelectionStub("ProcessModel"));
            view.DomainUsage.Returns(new SelectionStub("DomainUsage"));
            view.Runtime.Returns(new SelectionStub("Runtime"));
            view.RuntimeVersion.Returns(new SelectionStub("RuntimeVersion"));
            view.AssemblyList.Returns(new SelectionStub("AssemblyList"));

            presenter = new PropertyPresenter(model, view);
            presenter.LoadViewFromModel();
        }

        [Test]
        public void ActiveConfigName_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.ActiveConfigName.Text, Is.EqualTo("Release"));
        }

        [Test]
        public void ApplicationBase_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.ApplicationBase.Text, Is.EqualTo(model.Configs[0].BasePath));
        }

        [Test]
        public void AssemblyPath_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.AssemblyPath.Text, Is.EqualTo(model.Configs[0].Assemblies[0]));
        }

        [Test]
        public void AssemblyList_LoadFromModel_SetsListCorrectly()
        {
            Assert.That(view.AssemblyList.SelectionList, 
                Is.EqualTo(new string[] {"assembly1.dll", "assembly2.dll"}));
        }

        [Test]
        public void AssemblyList_WhenEmpty_AddIsEnabled()
        {
            view.AssemblyList.SelectionList = new string[0];
            view.AssemblyList.SelectedIndex = -1;

            Assert.True(view.AddAssemblyCommand.Enabled);
        }

        [Test]
        public void AssemblyList_WhenEmpty_RemoveIsDisabled()
        {
            view.AssemblyList.SelectionList = new string[0];
            view.AssemblyList.SelectedIndex = -1;

            Assert.False(view.RemoveAssemblyCommand.Enabled);
        }

        [Test]
        public void AssemblyList_WhenEmpty_AssemblyPathBrowseIsDisabled()
        {
            view.AssemblyList.SelectionList = new string[0];
            view.AssemblyList.SelectedIndex = -1;

            Assert.False(view.BrowseAssemblyPathCommand.Enabled);
        }

        [Test]
        public void BinPathType_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.BinPathType.SelectedIndex, Is.EqualTo((int)model.Configs[0].BinPathType));
        }

        [Test]
        public void ConfigList_LoadFromModel_SelectsFirstConfig()
        {
            Assert.That(view.ConfigList.SelectedIndex, Is.EqualTo(0));
            Assert.That(view.ConfigList.SelectedItem, Is.EqualTo("Debug"));
        }

        [Test]
        public void ConfigList_LoadFromModel_SetsListCorrectly()
        {
            Assert.That(view.ConfigList.SelectionList, Is.EqualTo(
                new string[] { "Debug", "Release" }));
        }

        public void ConfigList_SelectionChanged_UpdatesRuntime()
        {
        }

        [Test]
        public void ConfigurationFile_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.ConfigurationFile.Text, Is.EqualTo(model.Configs[0].ConfigurationFile));
        }

        [Test]
        public void DomainUsage_LoadFromModel_SetsOptionsCorrectly()
        {
            Assert.That(view.DomainUsage.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single", "Multiple" }));
        }

        [Test]
        public void DomainUsage_LoadFromModel_SelectsDefaultEntry()
        {
            Assert.That(view.DomainUsage.SelectedItem, Is.EqualTo("Default"));
        }

        [Test]
        public void DomainUsage_WhenChanged_UpdatesProject()
        {
            view.DomainUsage.SelectedItem = "Multiple";
            Assert.That(model.DomainUsage, Is.EqualTo("Multiple"));
        }

        [Test]
        public void PrivateBinPath_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.PrivateBinPath.Text, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ProcessModel_LoadFromModel_SelectsDefaultEntry()
        {
            Assert.That(view.ProcessModel.SelectedItem, Is.EqualTo("Default"));
        }

        [Test]
        public void ProcessModel_LoadFromModel_SetsOptionsCorrectly()
        {
            Assert.That(view.ProcessModel.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single", "Separate", "Multiple" }));
        }

        [Test]
        public void ProcessModel_WhenChanged_UpdatesDomainUsageOptions()
        {
            view.ProcessModel.SelectedItem = "Single";
            Assert.That(view.DomainUsage.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single", "Multiple" }));

            view.ProcessModel.SelectedItem = "Multiple";
            Assert.That(view.DomainUsage.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single" }));
        }

        [Test]
        public void ProcessModel_WhenChanged_UpdatesProject()
        {
            view.ProcessModel.SelectedItem = "Multiple";
            Assert.That(model.ProcessModel, Is.EqualTo("Multiple"));
        }

        [Test]
        public void ProjectBase_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.ProjectBase.Text, Is.EqualTo(model.EffectiveBasePath));
        }

        [Test]
        public void ProjectBase_WhenChanged_UpdatesProject()
        {
            view.ProjectBase.Text = "test.nunit";

            view.ProjectBase.Validated += Raise.Event<ActionDelegate>();
            Assert.That(model.BasePath, Is.EqualTo("test.nunit"));
        }

        [Test]
        public void ProjectPath_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(view.ProjectPath.Text, Is.EqualTo(model.ProjectPath));
        }

        [Test]
        public void Runtime_LoadFromModel_SetsOptionsCorrectly()
        {
            Assert.That(view.Runtime.SelectionList, Is.EqualTo(
                new string[] { "Any", "Net", "Mono" }));
        }

        [Test]
        public void Runtime_LoadFromModel_SelectsAnyRuntime()
        {
            Assert.That(view.Runtime.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void Runtime_WhenChanged_UpdatesProject()
        {
            // Set to non-default values for this test
            IProjectConfig config = model.Configs[0];
            config.RuntimeFramework = new RuntimeFramework(RuntimeType.Net, new Version("2.0.50727"));

            view.ConfigList.SelectedIndex = 0;
            view.Runtime.SelectedItem = "Mono";

            Assert.That(config.RuntimeFramework.Runtime, Is.EqualTo(RuntimeType.Mono));
            Assert.That(config.RuntimeFramework.Version, Is.EqualTo(new Version("2.0.50727")));
        }

        [Test]
        public void RuntimeVersion_LoadFromModel_SetsOptionsCorretly()
        {
            Assert.That(view.RuntimeVersion.SelectionList, Is.EqualTo(
                new string[] { "1.0.3705", "1.1.4322", "2.0.50727", "4.0.21006" }));
            //Assert.That(xmlView.RuntimeVersion.SelectedItem, Is.EqualTo("2.0.50727"));
        }

        [Test]
        public void RuntimeVersion_WhenSelectionChanged_UpdatesProject()
        {
            // Set to non-default values for this test
            IProjectConfig config = model.Configs[0];
            config.RuntimeFramework = new RuntimeFramework(RuntimeType.Net, new Version("2.0.50727"));

            view.RuntimeVersion.SelectedItem = "4.0.21006";

            Assert.That(config.RuntimeFramework.Runtime, Is.EqualTo(RuntimeType.Net));
            Assert.That(config.RuntimeFramework.Version, Is.EqualTo(new Version(4, 0, 21006)));
        }

        public void RuntimeVersion_WhenTextChanged_UpdatesProject()
        {
            // Set to non-default values for this test
            IProjectConfig config = model.Configs[0];
            config.RuntimeFramework = new RuntimeFramework(RuntimeType.Net, new Version("2.0.50727"));

            view.RuntimeVersion.Text = "4.0";

            Assert.That(config.RuntimeFramework.Runtime, Is.EqualTo(RuntimeType.Net));
            Assert.That(config.RuntimeFramework.Version, Is.EqualTo(new Version(4, 0)));
        }
    }
}
#endif
