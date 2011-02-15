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

using System;
using System.Collections.Generic;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor
{
    public interface IPropertyView : IView
    {
        #region Properties

        IDialogManager DialogManager { get; }
        IConfigurationEditorDialog ConfigurationEditorDialog { get; }

        #region Command Elements

        ICommand BrowseProjectBaseCommand { get; }
        ICommand EditConfigsCommand { get; }
        ICommand BrowseConfigBaseCommand { get; }

        ICommand AddAssemblyCommand { get; }
        ICommand RemoveAssemblyCommand { get; }
        ICommand BrowseAssemblyPathCommand { get; }

        #endregion

        #region Properties of the Model as a Whole

        ITextElement ProjectPath { get; }
        ITextElement ProjectBase { get; }
        ISelectionList ProcessModel { get; }
        ISelectionList DomainUsage { get; }
        ITextElement ActiveConfigName { get; }

        ISelectionList ConfigList { get; }

        #endregion

        #region Properties of the Selected Config

        ISelectionList Runtime { get; }
        IComboBox RuntimeVersion { get; }
        ITextElement ApplicationBase { get; }
        ITextElement ConfigurationFile { get; }

        ISelection BinPathType { get; }
        ITextElement PrivateBinPath { get; }

        ISelectionList AssemblyList { get; }
        ITextElement AssemblyPath { get; }

        #endregion

        #endregion
    }
}
