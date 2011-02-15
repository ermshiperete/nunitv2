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
using NSubstitute;
using NUnit.Framework;
using NUnit.TestUtilities;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    [TestFixture]
    public class MainPresenterTests
    {
        // TODO: Embed project resources
        private static readonly string GOOD_PROJECT = "NUnitTests.nunit";
        private static readonly string BAD_PROJECT = "BadProject.nunit";
        private static readonly string NONEXISTENT_PROJECT = "NonExistent.nunit";

        private IMainView view;
        private IProjectDocument doc;
        private MainPresenter presenter;

        [SetUp]
        public void Initialize()
        {
            view = Substitute.For<IMainView>();
            doc = new ProjectDocument();
            presenter = new MainPresenter(doc, view);
        }

        [Test]
        public void ActiveViewChanged_WhenNoProjectIsOpen_TabViewsRemainHidden()
        {
            view.SelectedView.Returns(SelectedView.XmlView);
            view.ActiveViewChanged += Raise.Event<ActiveViewChangedHandler>();
            Assert.False(view.PropertyView.Visible);
            Assert.False(view.XmlView.Visible);

            view.SelectedView.Returns(SelectedView.PropertyView);
            view.ActiveViewChanged += Raise.Event<ActiveViewChangedHandler>();
            Assert.False(view.PropertyView.Visible);
            Assert.False(view.XmlView.Visible);
        }

        [Test]
        public void ActiveViewChanged_WhenProjectIsOpen_TabViewsAreVisible()
        {
            doc.CreateNewProject();

            view.SelectedView.Returns(SelectedView.XmlView);
            view.ActiveViewChanged += Raise.Event<ActiveViewChangedHandler>();

            Assert.True(view.PropertyView.Visible);
            Assert.True(view.XmlView.Visible);

        }

        [Test]
        public void CloseProject_OnLoad_IsDisabled()
        {
            Assert.False(view.CloseProjectCommand.Enabled);
        }

        [Test]
        public void CloseProject_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandDelegate>();

            Assert.True(view.CloseProjectCommand.Enabled);
        }

        [Test]
        public void CloseProject_AfterOpeningGoodProject_IsEnabled()
        {
            using (new TempFile(GOOD_PROJECT))
            {
                view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
                view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

                Assert.True(view.CloseProjectCommand.Enabled);
            }
        }

        [Test]
        public void NewProject_OnLoad_IsEnabled()
        {
            Assert.True(view.NewProjectCommand.Enabled);
        }

        [Test]
        public void NewProject_WhenClicked_CreatesNewProject()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandDelegate>();

            Assert.IsNotNull(doc.RootNode);
            Assert.That(doc.Name, Is.StringMatching("Project\\d"));
        }

        [Test]
        public void OpenProject_OnLoad_IsEnabled()
        {
            Assert.True(view.OpenProjectCommand.Enabled);
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectIsValid_OpensProject()
        {
            using (new TempFile(GOOD_PROJECT))
            {
                view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
                view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

                Assert.NotNull(doc.XmlText);
                Assert.NotNull(doc.RootNode);
                Assert.AreEqual("NUnitTests", doc.Name);
            }
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectXmlIsNotValid_OpensProject()
        {
            using (new TempFile(BAD_PROJECT))
            {
                view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(BAD_PROJECT);
                view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

                Assert.NotNull(doc.XmlText);
                Assert.Null(doc.RootNode);
                Assert.AreEqual("BadProject", doc.Name);

                Assert.AreEqual(SelectedView.XmlView, view.SelectedView);
            }
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectDoesNotExist_DisplaysError()
        {
            view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(NONEXISTENT_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

            view.MessageDisplay.Received().Error(Arg.Is((string x) => x.StartsWith("Could not find file")));

            Assert.Null(doc.XmlText);
            Assert.Null(doc.RootNode);
        }

        [Test]
        public void SaveProject_OnLoad_IsDisabled()
        {
            Assert.False(view.SaveProjectCommand.Enabled);
        }

        [Test]
        public void SaveProject_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandDelegate>();

            Assert.True(view.SaveProjectCommand.Enabled);
        }

        [Test]
        public void SaveProject_AfterOpeningGoodProject_IsEnabled()
        {
            using (new TempFile(GOOD_PROJECT))
            {
                view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
                view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

                Assert.True(view.SaveProjectCommand.Enabled);
            }
        }

        [Test]
        public void SaveProjectAs_OnLoad_IsDisabled()
        {
            Assert.False(view.SaveProjectAsCommand.Enabled);
        }

        [Test]
        public void SaveProjectAs_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandDelegate>();

            Assert.True(view.SaveProjectAsCommand.Enabled);
        }

        [Test]
        public void SaveProjectAs_AfterOpeningGoodProject_IsEnabled()
        {
            using (new TempFile(GOOD_PROJECT))
            {
                view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
                view.OpenProjectCommand.Execute += Raise.Event<CommandDelegate>();

                Assert.True(view.SaveProjectAsCommand.Enabled);
            }
        }

        private class TempFile : TempResourceFile
        {
            public TempFile(string name) : base(typeof(NUnitProjectXml), "resources." + name, name) { }
        }
    }
}
#endif
