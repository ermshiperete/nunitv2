﻿using System;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Delegate used by tests that execute code and
    /// capture any thrown exception.
    /// </summary>
    public delegate void TestDelegate();

    /// <summary>
    /// Provides a base for Assert, Assume and any other
    /// additional classes that have static methods for
    /// applying constraints and reporting the result.
    /// 
    /// Unfortunately, since the derived class are
    /// all static, we can't put much in this base.
    /// </summary>
    public class AssertBase
    {
        #region Assert Counting

        private static int counter = 0;

        /// <summary>
        /// Gets the number of assertions executed so far and 
        /// resets the counter to zero.
        /// </summary>
        public static int Counter
        {
            get
            {
                int cnt = counter;
                counter = 0;
                return cnt;
            }
        }

        protected static void IncrementAssertCount()
        {
            ++counter;
        }

        #endregion

        #region Equals and ReferenceEquals

        /// <summary>
        /// The Equals method throws an AssertionException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            // TODO: This should probably be InvalidOperationException
            throw new AssertionException("Assert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an AssertionException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Helper for Assert.AreEqual(double expected, double actual, ...)
        /// allowing code generation to work consistently.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="delta">The maximum acceptable difference between the
        /// the expected and the actual</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string message, object[] args)
        {
            if (double.IsNaN(expected) || double.IsInfinity(expected))
                Assert.That(actual, Is.EqualTo(expected), message, args);
            else
                Assert.That(actual, Is.EqualTo(expected).Within(delta), message, args);
        }
        #endregion

        #region Utility Asserts

        #region Pass

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Pass(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            throw new SuccessException(message);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Pass(string message)
        {
            Assert.Pass(message, null);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        static public void Pass()
        {
            Assert.Pass(string.Empty, null);
        }

        #endregion

        #region Fail

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message and arguments 
        /// that are passed in. This is used by the other Assert functions. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Fail(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            throw new AssertionException(message);
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message that is 
        /// passed in. This is used by the other Assert functions. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Fail(string message)
        {
            Assert.Fail(message, null);
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/>. 
        /// This is used by the other Assert functions. 
        /// </summary>
        static public void Fail()
        {
            Assert.Fail(string.Empty, null);
        }

        #endregion

        #region Ignore

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message and arguments 
        /// that are passed in.  This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Ignore(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            throw new IgnoreException(message);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message that is 
        /// passed in. This causes the test to be reported as ignored. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Ignore(string message)
        {
            Assert.Ignore(message, null);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/>. 
        /// This causes the test to be reported as ignored. 
        /// </summary>
        static public void Ignore()
        {
            Assert.Ignore(string.Empty, null);
        }

        #endregion

        #region InConclusive
        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message and arguments 
        /// that are passed in.  This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Inconclusive(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            throw new InconclusiveException(message);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message that is 
        /// passed in. This causes the test to be reported as inconclusive. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        static public void Inconclusive(string message)
        {
            Assert.Inconclusive(message, null);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/>. 
        /// This causes the test to be reported as Inconclusive. 
        /// </summary>
        static public void Inconclusive()
        {
            Assert.Inconclusive(string.Empty, null);
        }

        #endregion

        #endregion

        #region Assert.That

        #region Object
        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        static public void That(object actual, IResolveConstraint expression)
        {
            Assert.That(actual, expression, null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void That(object actual, IResolveConstraint expression, string message)
        {
            Assert.That(actual, expression, message, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void That(object actual, IResolveConstraint expression, string message, params object[] args)
        {
            Constraint constraint = expression.Resolve();

            Assert.IncrementAssertCount();
            if (!constraint.Matches(actual))
            {
                MessageWriter writer = new TextMessageWriter(message, args);
                constraint.WriteMessageTo(writer);
                throw new AssertionException(writer.ToString());
            }
        }
        #endregion

        #region ActualValueDelegate
        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        static public void That(ActualValueDelegate del, IResolveConstraint expr)
        {
            Assert.That(del, expr.Resolve(), null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void That(ActualValueDelegate del, IResolveConstraint expr, string message)
        {
            Assert.That(del, expr.Resolve(), message, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void That(ActualValueDelegate del, IResolveConstraint expr, string message, params object[] args)
        {
            Constraint constraint = expr.Resolve();

            Assert.IncrementAssertCount();
            if (!constraint.Matches(del))
            {
                MessageWriter writer = new TextMessageWriter(message, args);
                constraint.WriteMessageTo(writer);
                throw new AssertionException(writer.ToString());
            }
        }
        #endregion

        #region ref Object
#if NET_2_0
        /// <summary>
        /// Apply a constraint to a referenced value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        static public void That<T>(ref T actual, IResolveConstraint constraint)
        {
            Assert.That(ref actual, constraint.Resolve(), null, null);
        }

        /// <summary>
        /// Apply a constraint to a referenced value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void That<T>(ref T actual, IResolveConstraint constraint, string message)
        {
            Assert.That(ref actual, constraint.Resolve(), message, null);
        }

        /// <summary>
        /// Apply a constraint to a referenced value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void That<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
        {
            Constraint constraint = expression.Resolve();

            Assert.IncrementAssertCount();
            if (!constraint.Matches(ref actual))
            {
                MessageWriter writer = new TextMessageWriter(message, args);
                constraint.WriteMessageTo(writer);
                throw new AssertionException(writer.ToString());
            }
        }
#else
        /// <summary>
        /// Apply a constraint to a referenced boolean, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        static public void That(ref bool actual, IResolveConstraint constraint)
        {
            Assert.That(ref actual, constraint.Resolve(), null, null);
        }

        /// <summary>
        /// Apply a constraint to a referenced value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void That(ref bool actual, IResolveConstraint constraint, string message)
        {
            Assert.That(ref actual, constraint.Resolve(), message, null);
        }

        /// <summary>
        /// Apply a constraint to a referenced value, succeeding if the constraint
        /// is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="constraint">A Constraint to be applied</param>
        /// <param name="actual">The actual value to test</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void That(ref bool actual, IResolveConstraint expression, string message, params object[] args)
        {
            Constraint constraint = expression.Resolve();

            Assert.IncrementAssertCount();
            if (!constraint.Matches(ref actual))
            {
                MessageWriter writer = new TextMessageWriter(message, args);
                constraint.WriteMessageTo(writer);
                throw new AssertionException(writer.ToString());
            }
        }
#endif
        #endregion

        #region Boolean
        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void That(bool condition, string message, params object[] args)
        {
            Assert.That(condition, Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        static public void That(bool condition, string message)
        {
            Assert.That(condition, Is.True, message, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        static public void That(bool condition)
        {
            Assert.That(condition, Is.True, null, null);
        }
        #endregion

        /// <summary>
        /// Asserts that the code represented by a delegate throws an exception
        /// that satisfies the constraint provided.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A ThrowsConstraint used in the test</param>
        static public void That(TestDelegate code, IResolveConstraint constraint)
        {
            Assert.That((object)code, constraint);
        }
        #endregion

        #region Throws and DoesNotThrow

        #region Throws
        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception Throws(IResolveConstraint expression, TestDelegate code, string message, params object[] args)
        {
            Exception caughtException = Catch.Exception(code);
            Assert.That(caughtException, expression, message, args);

            return caughtException;
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static Exception Throws(IResolveConstraint expression, TestDelegate code, string message)
        {
            return Throws(expression, code, message, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Exception Throws(IResolveConstraint expression, TestDelegate code)
        {
            return Throws(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception Throws(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
        {
            return Throws(new ExactTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static Exception Throws(Type expectedExceptionType, TestDelegate code, string message)
        {
            return Throws(new ExactTypeConstraint(expectedExceptionType), code, message, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Exception Throws(Type expectedExceptionType, TestDelegate code)
        {
            return Throws(new ExactTypeConstraint(expectedExceptionType), code, string.Empty, null);
        }

        #endregion

        #region Throws<T>
#if NET_2_0
        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="T">Type of the expected exception</typeparam>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static T Throws<T>(TestDelegate code, string message, params object[] args) where T : Exception
        {
            return (T)Throws(typeof(T), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="T">Type of the expected exception</typeparam>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static T Throws<T>(TestDelegate code, string message) where T : Exception
        {
            return Throws<T>(code, message, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="T">Type of the expected exception</typeparam>
        /// <param name="code">A TestSnippet delegate</param>
        public static T Throws<T>(TestDelegate code) where T : Exception
        {
            return Throws<T>(code, string.Empty, null);
        }
#endif
        #endregion

        #region DoesNotThrow

        /// <summary>
        /// Verifies that a delegate does not throw an exception
        /// </summary>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotThrow(TestDelegate code, string message, params object[] args)
        {
            try
            {
                code();
            }
            catch (Exception ex)
            {
                TextMessageWriter writer = new TextMessageWriter(message, args);
                writer.WriteLine("Unexpected exception: {0}", ex.GetType());
                Assert.Fail(writer.ToString());
            }
        }

        /// <summary>
        /// Verifies that a delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void DoesNotThrow(TestDelegate code, string message)
        {
            DoesNotThrow(code, message, null);
        }

        /// <summary>
        /// Verifies that a delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A TestSnippet delegate</param>
        public static void DoesNotThrow(TestDelegate code)
        {
            DoesNotThrow(code, string.Empty, null);
        }

        #endregion

        #endregion
    }
}
