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
using System.Xml;

namespace NUnit.ProjectEditor
{
    class XmlHelper
    {
        #region Attributes

        public static string GetAttribute(XmlNode node, string name)
        {
            XmlAttribute attr = node.Attributes[name];
            return attr == null ? null : attr.Value.ToString();
        }

        public static T GetAttributeAsEnum<T>(XmlNode node, string name, T defaultValue)
        {
            string attrVal = XmlHelper.GetAttribute(node, name);
            if (attrVal == null)
                return defaultValue;

            if (typeof(T).IsEnum)
            {
                foreach (string s in Enum.GetNames(typeof(T)))
                    if (s.Equals(attrVal, StringComparison.OrdinalIgnoreCase))
                        return (T)Enum.Parse(typeof(T), attrVal, true);
            }

            throw new ProjectFormatException(
                string.Format("Invalid attribute value: {0}", node.Attributes[name].OuterXml));
        }

        /// <summary>
        /// Adds an attribute with a specified name and value to an existing XmlNode.
        /// </summary>
        /// <param name="node">The node to which the attribute should be added.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public static void AddAttribute(XmlNode node, string name, string value)
        {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

        public static void RemoveAttribute(XmlNode node, string name)
        {
            XmlAttribute attr = node.Attributes[name];
            if (attr != null)
                node.Attributes.Remove(attr);
        }

        public static void SetAttribute(XmlNode node, string name, object value)
        {
            bool attrAdded = false;

            XmlAttribute attr = node.Attributes[name];
            if (attr == null)
            {
                attr = node.OwnerDocument.CreateAttribute(name);
                node.Attributes.Append(attr);
                attrAdded = true;
            }

            string valString = value.ToString();
            if (attrAdded || attr.Value != valString)
                attr.Value = valString;
        }
        
        #endregion

        /// <summary>
        /// Adds a new element as a child of an existing XmlNode and returns it.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <returns>The newly created child element</returns>
        public static XmlNode AddElement(XmlNode node, string name)
        {
            XmlNode childNode = node.OwnerDocument.CreateElement(name);
            node.AppendChild(childNode);
            return childNode;
        }
    }
}
