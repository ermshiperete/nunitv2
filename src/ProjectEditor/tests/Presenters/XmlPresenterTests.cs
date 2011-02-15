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
using System.Xml;
using NUnit.Framework;
using NSubstitute;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class XmlPresenterTests
    {
        private IProjectDocument doc;
        private IXmlView xmlView;
        private XmlPresenter presenter;

        private static readonly string initialText = "<NUnitProject />";
        private static readonly string changedText = "<NUnitProject processModel=\"Separate\" />";

        [SetUp]
        public void Initialize()
        {
            doc = new ProjectDocument();
            doc.LoadXml(initialText);
            xmlView = Substitute.For<IXmlView>();
            presenter = new XmlPresenter(doc, xmlView);
            presenter.LoadViewFromModel();
        }

        [Test]
        public void XmlText_OnLoad_IsInitializedCorrectly()
        {
            Assert.AreEqual(initialText, xmlView.Xml.Text);
        }

        [Test]
        public void XmlText_WhenChanged_ModelIsUpdated()
        {
            xmlView.Xml.Text = changedText;
            xmlView.Xml.Validated += Raise.Event<ActionDelegate>();
            Assert.AreEqual(changedText, doc.XmlText);
        }

        [Test]
        public void BadXmlSetsException()
        {
            xmlView.Xml.Text = "<NUnitProject>"; // Missing slash
            xmlView.Xml.Validated += Raise.Event<ActionDelegate>();
            
            Assert.AreEqual("<NUnitProject>", doc.XmlText);
            Assert.NotNull(doc.Exception);
            Assert.IsInstanceOf<XmlException>(doc.Exception);

            XmlException ex = doc.Exception as XmlException;
            xmlView.Received().DisplayError(ex.Message, ex.LineNumber, ex.LinePosition);
        }
    }
}
#endif
