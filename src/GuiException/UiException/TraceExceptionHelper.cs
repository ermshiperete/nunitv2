﻿// ----------------------------------------------------------------
// ExceptionBrowser
// Version 1.0.0
// Copyright 2008, Irénée HOTTIER,
// 
// This is free software licensed under the NUnit license, You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.UiException
{
    /// <summary>
    /// Exposes static methods to assert predicates and throw exceptions
    /// as needed.
    /// </summary>
    public class TraceExceptionHelper
    {
        /// <summary>
        /// Asserts that reference is not null; otherwise throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="value">The reference to be tested.</param>
        /// <param name="paramName">The name of this reference</param>
        public static void CheckNotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);

            return;
        }

        /// <summary>
        /// Asserts that 'test' is true or throws an ArgumentException.
        /// </summary>
        /// <param name="test">The boolean to be tested.</param>
        /// <param name="message">The error message.</param>
        /// <param name="paramName">The parameter name to be passed to ArgumentException.</param>
        public static void CheckTrue(bool test, string message, string paramName)
        {
            if (!test)
                throw new ArgumentException(message, paramName);

            return;
        }

        /// <summary>
        /// Asserts that 'test' is false or throws an ArgumentException.
        /// </summary>
        /// <param name="test">The boolean to be tested.</param>
        /// <param name="message">The error message.</param>
        /// <param name="paramName">The parameter name to be passed to ArgumentException.</param>
        public static void CheckFalse(bool test, string message, string paramName)
        {
            if (test)
                throw new ArgumentException(message, paramName);

            return;
        }

        /// <summary>
        /// Throws an ApplicationException with the given message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public static void Fail(string message)
        {
            throw new ApplicationException(message);
        }
    }
}
