// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace NUnit.Framework.Tests
{
	/// <summary>
	/// This test fixture attempts to exercise all the syntactic
	/// variations of Assert without getting into failures, errors 
	/// or corner cases. Thus, some of the tests may be duplicated 
	/// in other fixtures.
	/// 
	/// Each test performs the same operations using the classic
	/// syntax (if available) and the new syntax in both the
	/// helper-based and inherited forms.
	/// 
	/// This Fixture will eventually be duplicated in other
	/// supported languages. 
	/// </summary>
	[TestFixture]
	public class AssertSyntaxTests : AssertionHelper
	{
		[Test]
		public void IsNull()
		{
			// Classic syntax
			Assert.IsNull(null);

			// Helper syntax
			Assert.That(null, Is.Null);

			// Inherited syntax
			Expect(null, Null);
		}

		[Test]
		public void IsNotNull()
		{
			// Classic syntax
			Assert.IsNotNull(42);

			// Helper syntax
			Assert.That(42, Is.Not.Null);

			// Inherited syntax
			Expect( 42, Not.Null );
		}

		[Test]
		public void IsTrue()
		{
			// Classic syntax
			Assert.IsTrue(2+2==4);

			// Helper syntax
			Assert.That(2+2==4, Is.True);
			Assert.That(2+2==4);

			// Inherited syntax
			Expect(2+2==4, True);
			Expect(2+2==4);
		}

		[Test]
		public void IsFalse()
		{
			// Classic syntax
			Assert.IsFalse(2+2==5);

			// Helper syntax
			Assert.That(2+2== 5, Is.False);
			
			// Inherited syntax
			Expect(2+2==5, False);
		}

		[Test]
		public void IsNaN()
		{
			double d = double.NaN;
			float f = float.NaN;

			// Classic syntax
			Assert.IsNaN(d);
			Assert.IsNaN(f);

			// Helper syntax
			Assert.That(d, Is.NaN);
			Assert.That(f, Is.NaN);
			
			// Inherited syntax
			Expect(d, NaN);
			Expect(f, NaN);
		}

		[Test]
		public void EmptyStringTests()
		{
			// Classic syntax
			Assert.IsEmpty("");
			Assert.IsNotEmpty("Hello!");

			// Helper syntax
			Assert.That("", Is.Empty);
			Assert.That("Hello!", Is.Not.Empty);

			// Inherited syntax
			Expect("", Empty);
			Expect("Hello!", Not.Empty);
		}

		[Test]
		public void EmptyCollectionTests()
		{
			// Classic syntax
			Assert.IsEmpty(new bool[0]);
			Assert.IsNotEmpty(new int[] { 1, 2, 3 });

			// Helper syntax
			Assert.That(new bool[0], Is.Empty);
			Assert.That(new int[] { 1, 2, 3 }, Is.Not.Empty);

			// Inherited syntax
			Expect(new bool[0], Empty);
			Expect(new int[] { 1, 2, 3 }, Not.Empty);
		}

		[Test]
		public void ExactTypeTests()
		{
			// Classic syntax workarounds
			Assert.AreEqual(typeof(string), "Hello".GetType());
			Assert.AreEqual("System.String", "Hello".GetType().FullName);
			Assert.AreNotEqual(typeof(int), "Hello".GetType());
			Assert.AreNotEqual("System.Int32", "Hello".GetType().FullName);

			// Helper syntax
			Assert.That("Hello", Is.TypeOf(typeof(string)));
			Assert.That("Hello", Is.Not.TypeOf(typeof(int)));
			
			// Inherited syntax
			Expect( "Hello", TypeOf(typeof(string)));
			Expect( "Hello", Not.TypeOf(typeof(int)));
		}

		[Test]
		public void InstanceOfTypeTests()
		{
			// Classic syntax
			Assert.IsInstanceOfType(typeof(string), "Hello");
			Assert.IsNotInstanceOfType(typeof(string), 5);

			// Helper syntax
			Assert.That("Hello", Is.InstanceOfType(typeof(string)));
			Assert.That(5, Is.Not.InstanceOfType(typeof(string)));

			// Inherited syntax
			Expect("Hello", InstanceOfType(typeof(string)));
			Expect(5, Not.InstanceOfType(typeof(string)));
		}

		[Test]
		public void AssignableFromTypeTests()
		{
			// Classic syntax
			Assert.IsAssignableFrom(typeof(string), "Hello");
			Assert.IsNotAssignableFrom(typeof(string), 5);

			// Helper syntax
			Assert.That( "Hello", Is.AssignableFrom(typeof(string)));
			Assert.That( 5, Is.Not.AssignableFrom(typeof(string)));
			
			// Inherited syntax
			Expect( "Hello", AssignableFrom(typeof(string)));
			Expect( 5, Not.AssignableFrom(typeof(string)));
		}

		[Test]
		public void SubstringTests()
		{
			string phrase = "Hello World!";
			string[] array = new string[] { "Hello", "World", "!" };
			
			// Classic Syntax
			StringAssert.Contains("World", phrase);
			
			// Helper syntax
			Assert.That(phrase, Text.Contains("World"));
			// Only available using new syntax
			Assert.That(phrase, Text.DoesNotContain("goodbye"));
			Assert.That(phrase, Text.Contains("WORLD").IgnoreCase);

			// Inherited syntax
			Expect(phrase, Containing("World"));
			// Only available using new syntax
			Expect(phrase, Not.Containing("goodbye"));
			Expect(phrase, Containing("WORLD").IgnoreCase);
			Expect(phrase, Not.Containing("BYE").IgnoreCase);
		}

		[Test]
		public void StartsWithTests()
		{
			string phrase = "Hello World!";
			// Classic syntax
			StringAssert.StartsWith("Hello", phrase);

			// Helper syntax
			Assert.That(phrase, Text.StartsWith("Hello"));
			// Only available using new syntax
			Assert.That(phrase, Text.DoesNotStartWith("Hi!"));
			Assert.That(phrase, Text.StartsWith("HeLLo").IgnoreCase);
			Assert.That(phrase, Text.DoesNotStartWith("HI").IgnoreCase);

			// Inherited syntax
			Expect(phrase, Starting("Hello"));
			// Only available using new syntax
			Expect(phrase, Not.Starting("Hi!"));
			Expect(phrase, Starting("HeLLo").IgnoreCase);
			Expect(phrase, Not.Starting("HI").IgnoreCase);
		}

		[Test]
		public void EndsWithTests()
		{
			string phrase = "Hello World!";

			// Classic Syntax
			StringAssert.EndsWith("!", phrase);

			// Helper syntax
			Assert.That(phrase, Text.EndsWith("!"));
			// Only available using new syntax
			Assert.That(phrase, Text.DoesNotEndWith("?"));
			Assert.That(phrase, Text.EndsWith("WORLD!").IgnoreCase);
		
			// Inherited syntax
			Expect(phrase, Ending("!"));
			// Only available using new syntax
			Expect(phrase, Not.Ending("?"));
			Expect(phrase, Ending("WORLD!").IgnoreCase);
		}

		[Test]
		public void EqualIgnoringCaseTests()
		{
			string phrase = "Hello World!";

			// Classic syntax
			StringAssert.AreEqualIgnoringCase("hello world!",phrase);
            
			// Helper syntax
			Assert.That(phrase, Is.EqualTo("hello world!").IgnoreCase);
			//Only available using new syntax
			Assert.That(phrase, Is.Not.EqualTo("goodbye world!").IgnoreCase);
			Assert.That(new string[] { "Hello", "World" }, 
				Is.EqualTo(new object[] { "HELLO", "WORLD" }).IgnoreCase);
		            
			// Inherited syntax
			Expect(phrase, EqualTo("hello world!").IgnoreCase);
			//Only available using new syntax
			Expect(phrase, Not.EqualTo("goodbye world!").IgnoreCase);
			Expect(new string[] { "Hello", "World" }, 
				EqualTo(new object[] { "HELLO", "WORLD" }).IgnoreCase);
		}

		[Test]
		public void RegularExpressionTests()
		{
			string phrase = "Now is the time for all good men to come to the aid of their country.";

			// Classic syntax
			StringAssert.IsMatch( "all good men", phrase );
			StringAssert.IsMatch( "Now.*come", phrase );

			// Helper syntax
			Assert.That( phrase, Text.Matches( "all good men" ) );
			Assert.That( phrase, Text.Matches( "Now.*come" ) );
			// Only available using new syntax
			Assert.That(phrase, Text.DoesNotMatch("all.*men.*good"));
			Assert.That(phrase, Text.Matches("ALL").IgnoreCase);
		
			// Inherited syntax
			Expect( phrase, Matching( "all good men" ) );
			Expect( phrase, Matching( "Now.*come" ) );
			// Only available using new syntax
			Expect(phrase, Not.Matching("all.*men.*good"));
			Expect(phrase, Matching("ALL").IgnoreCase);
		}

		[Test]
		public void EqualityTests()
		{
			int[] i3 = new int[] { 1, 2, 3 };
			double[] d3 = new double[] { 1.0, 2.0, 3.0 };
			int[] iunequal = new int[] { 1, 3, 2 };

			// Classic Syntax
			Assert.AreEqual(4, 2 + 2);
			Assert.AreEqual(i3, d3);
			Assert.AreNotEqual(5, 2 + 2);
			Assert.AreNotEqual(i3, iunequal);

			// Helper syntax
			Assert.That(2 + 2, Is.EqualTo(4));
			Assert.That(2 + 2 == 4);
			Assert.That(i3, Is.EqualTo(d3));
			Assert.That(2 + 2, Is.Not.EqualTo(5));
			Assert.That(i3, Is.Not.EqualTo(iunequal));
		
			// Inherited syntax
			Expect(2 + 2, EqualTo(4));
			Expect(2 + 2 == 4);
			Expect(i3, EqualTo(d3));
			Expect(2 + 2, Not.EqualTo(5));
			Expect(i3, Not.EqualTo(iunequal));
		}

		[Test]
		public void EqualityTestsWithTolerance()
		{
			// CLassic syntax
			Assert.AreEqual(5.0d, 4.99d, 0.05d);
			Assert.AreEqual(5.0f, 4.99f, 0.05f);

			// Helper syntax
			Assert.That(4.99d, Is.EqualTo(5.0d).Within(0.05d));
			Assert.That(4.99f, Is.EqualTo(5.0f).Within(0.05f));
		
			// Inherited syntax
			Expect(4.99d, EqualTo(5.0d).Within(0.05d));
			Expect(4.99f, EqualTo(5.0f).Within(0.05f));
		}

		[Test]
		public void ComparisonTests()
		{
			// Classic Syntax
			Assert.Greater(7, 3);
			Assert.GreaterOrEqual(7, 3);
			Assert.GreaterOrEqual(7, 7);

			// Helper syntax
			Assert.That(7, Is.GreaterThan(3));
			Assert.That(7, Is.GreaterThanOrEqualTo(3));
			Assert.That(7, Is.AtLeast(3));
			Assert.That(7, Is.GreaterThanOrEqualTo(7));
			Assert.That(7, Is.AtLeast(7));

			// Inherited syntax
			Expect(7, GreaterThan(3));
			Expect(7, GreaterThanOrEqualTo(3));
			Expect(7, AtLeast(3));
			Expect(7, GreaterThanOrEqualTo(7));
			Expect(7, AtLeast(7));

			// Classic syntax
			Assert.Less(3, 7);
			Assert.LessOrEqual(3, 7);
			Assert.LessOrEqual(3, 3);

			// Helper syntax
			Assert.That(3, Is.LessThan(7));
			Assert.That(3, Is.LessThanOrEqualTo(7));
			Assert.That(3, Is.AtMost(7));
			Assert.That(3, Is.LessThanOrEqualTo(3));
			Assert.That(3, Is.AtMost(3));
		
			// Inherited syntax
			Expect(3, LessThan(7));
			Expect(3, LessThanOrEqualTo(7));
			Expect(3, AtMost(7));
			Expect(3, LessThanOrEqualTo(3));
			Expect(3, AtMost(3));
		}

		[Test]
		public void AllItemsTests()
		{
			object[] ints = new object[] { 1, 2, 3, 4 };
			object[] strings = new object[] { "a", "b", "c", "b", "d" };

			// Classic syntax
			CollectionAssert.AllItemsAreNotNull(ints);
			CollectionAssert.AllItemsAreInstancesOfType(ints, typeof(int));
			CollectionAssert.AllItemsAreInstancesOfType(strings, typeof(string));
			CollectionAssert.AllItemsAreUnique(ints);

			// Helper syntax
			Assert.That(ints, Is.All.Not.Null);
			Assert.That(ints, Is.All.InstanceOfType(typeof(int)));
			Assert.That(strings, Is.All.InstanceOfType(typeof(string)));
			Assert.That(ints, Is.Unique);
			// Only available using new syntax
			Assert.That(strings, Is.Not.Unique);
			Assert.That(ints, Is.All.GreaterThan(0));  
		
			// Inherited syntax
			Expect(ints, All.Not.Null);
			Expect(ints, All.InstanceOfType(typeof(int)));
			Expect(strings, All.InstanceOfType(typeof(string)));
			Expect(ints, Unique);
			// Only available using new syntax
			Expect(strings, Not.Unique);
			Expect(ints, All.GreaterThan(0));                 
		}

		[Test]
		public void CollectionContainsTests()
		{
			int[] iarray = new int[] { 1, 2, 3 };
			string[] sarray = new string[] { "a", "b", "c" };

			// Classic syntax
			Assert.Contains(3, iarray);
			Assert.Contains("b", sarray);
			CollectionAssert.Contains(iarray, 3);
			CollectionAssert.Contains(sarray, "b");
			CollectionAssert.DoesNotContain(sarray, "x");

			// Helper syntax
			Assert.That(iarray, Is.CollectionContaining(3));
			Assert.That(sarray, Is.CollectionContaining("b"));
			Assert.That(sarray, Is.Not.CollectionContaining("x"));
		
			// Inherited syntax
			Expect(iarray, Containing(3));
			Expect(sarray, Containing("b"));
			Expect(sarray, Not.Containing("x"));
		}

		[Test]
		public void CollectionEquivalenceTests()
		{
			int[] ints1to5 = new int[] { 1, 2, 3, 4, 5 };

			// Classic syntax
			CollectionAssert.AreEquivalent(new int[] { 2, 1, 4, 3, 5 }, ints1to5);
			CollectionAssert.AreNotEquivalent(new int[] { 2, 2, 4, 3, 5 }, ints1to5);
			CollectionAssert.AreNotEquivalent(new int[] { 2, 4, 3, 5 }, ints1to5);
			CollectionAssert.AreEquivalent(new int[] { 2, 2, 1, 1, 4, 3, 5 }, ints1to5);
		
			// Helper syntax
			Assert.That(new int[] { 2, 1, 4, 3, 5 }, Is.EquivalentTo(ints1to5));
			Assert.That(new int[] { 2, 2, 4, 3, 5 }, Is.Not.EquivalentTo(ints1to5));
			Assert.That(new int[] { 2, 4, 3, 5 }, Is.Not.EquivalentTo(ints1to5));
			Assert.That(new int[] { 2, 2, 1, 1, 4, 3, 5 }, Is.EquivalentTo(ints1to5));

			// Inherited syntax
			Expect(new int[] { 2, 1, 4, 3, 5 }, EquivalentTo(ints1to5));
			Expect(new int[] { 2, 2, 4, 3, 5 }, Not.EquivalentTo(ints1to5));
			Expect(new int[] { 2, 4, 3, 5 }, Not.EquivalentTo(ints1to5));
			Expect(new int[] { 2, 2, 1, 1, 4, 3, 5 }, EquivalentTo(ints1to5));
		}

		[Test]
		public void SubsetTests()
		{
			int[] ints1to5 = new int[] { 1, 2, 3, 4, 5 };

			// Classic syntax
			CollectionAssert.IsSubsetOf(new int[] { 1, 3, 5 }, ints1to5);
			CollectionAssert.IsSubsetOf(new int[] { 1, 2, 3, 4, 5 }, ints1to5);
			CollectionAssert.IsNotSubsetOf(new int[] { 2, 4, 6 }, ints1to5);

			// Helper syntax
			Assert.That(new int[] { 1, 3, 5 }, Is.SubsetOf(ints1to5));
			Assert.That(new int[] { 1, 2, 3, 4, 5 }, Is.SubsetOf(ints1to5));
			Assert.That(new int[] { 2, 4, 6 }, Is.Not.SubsetOf(ints1to5));
		
			// Inherited syntax
			Expect(new int[] { 1, 3, 5 }, SubsetOf(ints1to5));
			Expect(new int[] { 1, 2, 3, 4, 5 }, SubsetOf(ints1to5));
			Expect(new int[] { 2, 4, 6 }, Not.SubsetOf(ints1to5));
		}

		[Test]
		public void NotTests()
		{
			// Not available usin the classic syntax

			// Helper syntax
			Assert.That(42, Is.Not.Null);
			Assert.That(42, Is.Not.True);
			Assert.That(42, Is.Not.False);
			Assert.That(2.5, Is.Not.NaN);
			Assert.That(2 + 2, Is.Not.EqualTo(3));
			Assert.That(2 + 2, Is.Not.Not.EqualTo(4));
			Assert.That(2 + 2, Is.Not.Not.Not.EqualTo(5));

			// Inherited syntax
			Expect(42, Not.Null);
			Expect(42, Not.True);
			Expect(42, Not.False);
			Expect(2.5, Not.NaN);
			Expect(2 + 2, Not.EqualTo(3));
			Expect(2 + 2, Not.Not.EqualTo(4));
			Expect(2 + 2, Not.Not.Not.EqualTo(5));
		}

		[Test]
		public void NotOperator()
		{
			// The ! operator is only available in the new syntax
			Assert.That(42, !Is.Null);
			// Inherited syntax
			Expect( 42, !Null );
		}

		[Test]
		public void AndOperator()
		{
			// The & operator is only available in the new syntax
			Assert.That(7, Is.GreaterThan(5) & Is.LessThan(10));
			// Inherited syntax
			Expect( 7, GreaterThan(5) & LessThan(10));
		}

		[Test]
		public void OrOperator()
		{
			// The | operator is only available in the new syntax
			Assert.That(3, Is.LessThan(5) | Is.GreaterThan(10));
			Expect( 3, LessThan(5) | GreaterThan(10));
		}

		[Test]
		public void ComplexTests()
		{
			Assert.That(7, Is.Not.Null & Is.Not.LessThan(5) & Is.Not.GreaterThan(10));
			Expect(7, Not.Null & Not.LessThan(5) & Not.GreaterThan(10));

			Assert.That(7, !Is.Null & !Is.LessThan(5) & !Is.GreaterThan(10));
			Expect(7, !Null & !LessThan(5) & !GreaterThan(10));

			// TODO: Remove #if when mono compiler can handle null
#if MONO
            Constraint x = null;
            Assert.That(7, !x & !Is.LessThan(5) & !Is.GreaterThan(10));
			Expect(7, !x & !LessThan(5) & !GreaterThan(10));
#else
			Assert.That(7, !(Constraint)null & !Is.LessThan(5) & !Is.GreaterThan(10));
			Expect(7, !(Constraint)null & !LessThan(5) & !GreaterThan(10));
#endif
		}

		// This method contains assertions that should not compile
		// You can check by uncommenting it.
		//public void WillNotCompile()
		//{
		//    Assert.That(42, Is.Not);
		//    Assert.That(42, Is.All);
		//    Assert.That(42, Is.Null.Not);
		//    Assert.That(42, Is.Not.Null.GreaterThan(10));
		//    Assert.That(42, Is.GreaterThan(10).LessThan(99));

		//    object[] c = new object[0];
		//    Assert.That(c, Is.Null.All);
		//    Assert.That(c, Is.Not.All);
		//    Assert.That(c, Is.All.Not);
		//}
	}

}
