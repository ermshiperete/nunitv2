// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ConstraintBuilder is used to resolve the Not and All properties,
    /// which serve as prefix operators for constraints. With the addition
    /// of an operand stack, And and Or could be supported, but we have
    /// left them out in favor of a simpler, more type-safe implementation.
    /// Use the &amp; and | operator overloads to combine constraints.
    /// </summary>
    public class ConstraintBuilder
    {
		private enum Op
		{
			Not,
			All
		}

		Stack ops = new Stack();

        #region Constraints Without Arguments
        /// <summary>
        /// Resolves the chain of constraints using
        /// EqualConstraint(null) as base.
        /// </summary>
        public Constraint Null
        {
            get { return Resolve(new EqualConstraint(null)); }
        }

        /// <summary>
        /// Resolves the chain of constraints using
        /// EqualConstraint(true) as base.
        /// </summary>
        public Constraint True
        {
            get { return Resolve(new EqualConstraint(true)); }
        }

        /// <summary>
        /// Resolves the chain of constraints using
        /// EqualConstraint(false) as base.
        /// </summary>
        public Constraint False
        {
            get { return Resolve(new EqualConstraint(false)); }
        }

        /// <summary>
        /// Resolves the chain of constraints using
        /// Is.NaN as base.
        /// </summary>
        public Constraint NaN
        {
            get { return Resolve(new EqualConstraint(double.NaN)); }
        }

        /// <summary>
        /// Resolves the chain of constraints using
        /// Is.Empty as base.
        /// </summary>
        public Constraint Empty
        {
            get { return Resolve(new EmptyConstraint()); }
        }

        /// <summary>
        /// Resolves the chain of constraints using
        /// Is.Unique as base.
        /// </summary>
        public Constraint Unique
        {
            get { return Resolve(new UniqueItemsConstraint()); }
        }
        #endregion

        #region Constraints with an expected value

        #region Equality and Identity
        /// <summary>
        /// Resolves the chain of constraints using an
        /// EqualConstraint as base.
        /// </summary>
        public Constraint EqualTo(object expected)
        {
            return Resolve(new EqualConstraint(expected));
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// SameAsConstraint as base.
        /// </summary>
        public Constraint SameAs(object expected)
        {
            return Resolve(new SameAsConstraint(expected));
        }
        #endregion

        #region Comparison Constraints
        /// <summary>
        /// Resolves the chain of constraints using a
        /// LessThanConstraint as base.
        /// </summary>
        public Constraint LessThan(IComparable expected)
        {
            return Resolve(new LessThanConstraint(expected));
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// GreaterThanConstraint as base.
        /// </summary>
        public Constraint GreaterThan(IComparable expected)
        {
            return Resolve(new GreaterThanConstraint(expected));
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// LessThanOrEqualConstraint as base.
        /// </summary>
        public Constraint LessThanOrEqualTo(IComparable expected)
        {
            return Resolve(new LessThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// LessThanOrEqualConstraint as base.
        /// </summary>
        public Constraint AtMost(IComparable expected)
        {
            return Resolve(new LessThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// GreaterThanOrEqualConstraint as base.
        /// </summary>
        public Constraint GreaterThanOrEqualTo(IComparable expected)
        {
            return Resolve(new GreaterThanOrEqualConstraint(expected));
        }
        /// <summary>
        /// Resolves the chain of constraints using a
        /// GreaterThanOrEqualConstraint as base.
        /// </summary>
        public Constraint AtLeast(IComparable expected)
        {
            return Resolve(new GreaterThanOrEqualConstraint(expected));
        }
        #endregion

        #region Type Constraints
        /// <summary>
        /// Resolves the chain of constraints using an
        /// ExactTypeConstraint as base.
        /// </summary>
        public Constraint TypeOf(Type expectedType)
        {
            return Resolve(new ExactTypeConstraint(expectedType));
        }

        /// <summary>
        /// Resolves the chain of constraints using an
        /// InstanceOfTypeConstraint as base.
        /// </summary>
        public Constraint InstanceOfType(Type expectedType)
        {
            return Resolve(new InstanceOfTypeConstraint(expectedType));
        }

        /// <summary>
        /// Resolves the chain of constraints using an
        /// AssignableFromConstraint as base.
        /// </summary>
        public Constraint AssignableFrom(Type expectedType)
        {
            return Resolve(new AssignableFromConstraint(expectedType));
        }
        #endregion

		#region Containing Constraint
		/// <summary>
		/// Resolves the chain of constraints using a
		/// ContainsConstraint as base. This constraint
		/// will, in turn, make use of the appropriate
		/// second-level constraint, depending on the
		/// type of the actual argument.
		/// </summary>
		public Constraint Contains(object expected)
		{
			return Resolve( new ContainsConstraint(expected) );
		}
		#endregion

		#region String Constraints
		/// <summary>
		/// Resolves the chain of constraints using a
		/// StartsWithConstraint as base.
		/// </summary>
		public Constraint StartsWith(string substring)
        {
            return Resolve( new StartsWithConstraint(substring) );
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// StringEndingConstraint as base.
        /// </summary>
        public Constraint EndsWith(string substring)
        {
            return Resolve( new EndsWithConstraint(substring) );
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// StringMatchingConstraint as base.
        /// </summary>
        public Constraint Matches(string pattern)
        {
            return Resolve(new RegexConstraint(pattern));
        }
        #endregion

        #region Collection Constraints
        /// <summary>
        /// Resolves the chain of constraints using a
        /// CollectionEquivalentConstraint as base.
        /// </summary>
        public Constraint EquivalentTo(ICollection expected)
        {
            return Resolve( new CollectionEquivalentConstraint(expected) );
        }

        /// <summary>
        /// Resolves the chain of constraints using a
        /// CollectionContainingConstraint as base.
        /// </summary>
        public Constraint CollectionContaining(object expected)
		{
			return Resolve( new CollectionContainsConstraint(expected) );
		}

        /// <summary>
        /// Resolves the chain of constraints using a
        /// CollectionSubsetConstraint as base.
        /// </summary>
        public Constraint SubsetOf(ICollection expected)
        {
            return Resolve(new CollectionSubsetConstraint(expected));
        }
        #endregion

		#region Property Constraints
        /// <summary>
        /// Resolves the chain of constraints using a 
        /// PropertyConstraint as base
        /// </summary>
		public Constraint Property( string name, object expected )
		{
			return Resolve( new PropertyConstraint( name, expected ) );
		}

        /// <summary>
        /// Resolves the chain of constraints using a
        /// PropertyCOnstraint on Length as base
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
		public Constraint Length( int length )
		{
			return Property( "Length", length );
		}
		#endregion

        #endregion

        #region Prefix Operators
        /// <summary>
        /// Modifies the ConstraintBuilder by pushing a Not operator on the stack.
        /// </summary>
        public ConstraintBuilder Not
        {
            get
            {
                ops.Push(Op.Not);
                return this;
            }
        }

        /// <summary>
        /// Modifies the ConstraintBuilder by pushing an All oeprator on the stack.
        /// </summary>
        public ConstraintBuilder All
        {
            get
            {
                ops.Push(Op.All);
                return this;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Resolve a constraint that has been recognized by applying
        /// any pending operators and returning the resulting Constraint.
        /// </summary>
        /// <param name="constraint">The root constraint</param>
        /// <returns>A constraint that incorporates all pending operators</returns>
        private Constraint Resolve(Constraint constraint)
        {
            while (ops.Count > 0)
                switch ((Op)ops.Pop())
                {
                    case Op.Not:
                        constraint = new NotConstraint(constraint);
                        break;
                    case Op.All:
                        constraint = new AllItemsConstraint(constraint);
                        break;
                }

            return constraint;
        }
        #endregion
    }
}
