// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class SubstringTest : ConstraintTestBase, IExpectException
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new SubstringConstraint("hello");
            expectedDescription = "String containing \"hello\"";
        }

        object[] GoodData = new object[] { "hello", "hello there", "I said hello", "say hello to fred" };
        
        object[] BadData = new object[] { "goodbye", "HELLO", "What the hell?", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"HELLO\"", "\"What the hell?\"", "<string.Empty>", "null" }; 

        public void HandleException(Exception ex)
        {
            Assert.That(ex.Message, new EqualConstraint(
                TextMessageWriter.Pfx_Expected + "String containing \"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa...\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual   + "\"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx...\"" + Environment.NewLine));
        }
    }

    [TestFixture]
    public class SubstringTestIgnoringCase : ConstraintTestBase
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new SubstringConstraint("hello").IgnoreCase;
            expectedDescription = "String containing \"hello\", ignoring case";
        }

        object[] GoodData = new object[] { "Hello", "HellO there", "I said HELLO", "say hello to fred" };
        
        object[] BadData = new object[] { "goodbye", "What the hell?", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"What the hell?\"", "<string.Empty>", "null" };
    }

    [TestFixture]
    public class StartsWithTest : ConstraintTestBase
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new StartsWithConstraint("hello");
            expectedDescription = "String starting with \"hello\"";
        }

        object[] GoodData = new object[] { "hello", "hello there" };

        object[] BadData = new object[] { "goodbye", "HELLO THERE", "I said hello", "say hello to fred", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"HELLO THERE\"", "\"I said hello\"", "\"say hello to fred\"", "<string.Empty>", "null" };
    }

    [TestFixture]
    public class StartsWithTestIgnoringCase : ConstraintTestBase
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new StartsWithConstraint("hello").IgnoreCase;
            expectedDescription = "String starting with \"hello\", ignoring case";
        }

        object[] GoodData = new object[] { "Hello", "HELLO there" };
            
        object[] BadData = new object[] { "goodbye", "What the hell?", "I said hello", "say Hello to fred", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"What the hell?\"", "\"I said hello\"", "\"say Hello to fred\"", "<string.Empty>", "null" };
    }

    [TestFixture]
    public class EndsWithTest : ConstraintTestBase
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new EndsWithConstraint("hello");
            expectedDescription = "String ending with \"hello\"";
        }

        object[] GoodData = new object[] { "hello", "I said hello" };
            
        object[] BadData = new object[] { "goodbye", "What the hell?", "hello there", "say hello to fred", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"What the hell?\"", "\"hello there\"", "\"say hello to fred\"", "<string.Empty>", "null" };
    }

    [TestFixture]
    public class EndsWithTestIgnoringCase : ConstraintTestBase
    {
		[SetUp]
        public void SetUp()
        {
            theConstraint = new EndsWithConstraint("hello").IgnoreCase;
            expectedDescription = "String ending with \"hello\", ignoring case";
        }

        object[] GoodData = new object[] { "HELLO", "I said Hello" };
            
        object[] BadData = new object[] { "goodbye", "What the hell?", "hello there", "say hello to fred", string.Empty, null };

        object[] FailureMessages = new object[] { "\"goodbye\"", "\"What the hell?\"", "\"hello there\"", "\"say hello to fred\"", "<string.Empty>", "null" };
    }

    //[TestFixture]
    //public class EqualIgnoringCaseTest : ConstraintTest
    //{
    //    [SetUp]
    //    public void SetUp()
    //    {
    //        Matcher = new EqualConstraint("Hello World!").IgnoreCase;
    //        Description = "\"Hello World!\", ignoring case";
    //    }

    //    object[] GoodData = new object[] { "hello world!", "Hello World!", "HELLO world!" };
            
    //    object[] BadData = new object[] { "goodbye", "Hello Friends!", string.Empty, null };

    //    object[] FailureMessages = new object[] { "\"goodbye\"", "\"Hello Friends!\"", "<string.Empty>", "null" };
    //}
}
