// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Delegate used to delay evaluation of the actual value
    /// to be used in evaluating a constraint
    /// </summary>
    public delegate object ActualValueDelegate();
    
    /// <summary>
    /// The Constraint class is the base of all built-in constraints
    /// within NUnit. It provides the operator overloads used to combine 
    /// constraints.
    /// </summary>
    public abstract class Constraint : IResolveConstraint
    {
        #region UnsetObject Class
        /// <summary>
        /// Class used to detect any derived constraints
        /// that fail to set the actual value in their
        /// Matches override.
        /// </summary>
        private class UnsetObject
        {
            public override string ToString()
            {
                return "UNSET";
            }
        }
        #endregion

		#region Static and Instance Fields
        /// <summary>
        /// Static UnsetObject used to detect derived constraints
        /// failing to set the actual value.
        /// </summary>
        protected static object UNSET = new UnsetObject();

		/// <summary>
        /// The actual value being tested against a constraint
        /// </summary>
        protected object actual = UNSET;

        /// <summary>
        /// The display name of this Constraint for use by ToString()
        /// </summary>
        private string displayName;

        /// <summary>
        /// Argument fields used by ToString();
        /// </summary>
        private readonly int argcnt;
        private readonly object arg1;
        private readonly object arg2;

        /// <summary>
        /// The builder holding this constraint
        /// </summary>
        private ConstraintBuilder builder;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a constraint with no arguments
        /// </summary>
        public Constraint()
        {
            argcnt = 0;
        }

        /// <summary>
        /// Construct a constraint with one argument
        /// </summary>
        public Constraint(object arg)
        {
            argcnt = 1;
            this.arg1 = arg;
        }

        /// <summary>
        /// Construct a constraint with two arguments
        /// </summary>
        public Constraint(object arg1, object arg2)
        {
            argcnt = 2;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
        #endregion

        #region Set Containing ConstraintBuilder
        /// <summary>
        /// Sets the ConstraintBuilder holding this constraint
        /// </summary>
        internal void SetBuilder(ConstraintBuilder builder)
        {
            this.builder = builder;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (displayName == null)
                {
                    displayName = this.GetType().Name.ToLower();
                    if (displayName.EndsWith("`1") || displayName.EndsWith("`2"))
                        displayName = displayName.Substring(0, displayName.Length - 2);
                    if (displayName.EndsWith("constraint"))
                        displayName = displayName.Substring(0, displayName.Length - 10);
                }

                return displayName;
            }

            set { displayName = value; }
        }
		#endregion

		#region Abstract and Virtual Methods
        /// <summary>
        /// Write the failure message to the MessageWriter provided
        /// as an argument. The default implementation simply passes
        /// the constraint and the actual value to the writer, which
        /// then displays the constraint description and the value.
        /// 
        /// Constraints that need to provide additional details,
        /// such as where the error occured can override this.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display the message</param>
        public virtual void WriteMessageTo(MessageWriter writer)
        {
            writer.DisplayDifferences(this);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public abstract bool Matches(object actual);

        /// <summary>
        /// Test whether the constraint is satisfied by an
        /// ActualValueDelegate that returns the value to be tested.
        /// The default implementation simply evaluates the delegate
        /// but derived classes may override it to provide for delayed 
        /// processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>True for success, false for failure</returns>
        public virtual bool Matches(ActualValueDelegate del)
        {
            return Matches(del());
        }

#if NET_2_0
        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public virtual bool Matches<T>(ref T actual)
        {
            return Matches(actual);
        }
#else
		/// <summary>
		/// Test whether the constraint is satisfied by a given bool reference.
		/// The default implementation simply dereferences the value but
		/// derived classes may override it to provide for delayed processing.
		/// </summary>
		/// <param name="actual">A reference to the value to be tested</param>
		/// <returns>True for success, false for failure</returns>
		public virtual bool Matches(ref bool actual)
		{
			return Matches(actual);
		}
#endif

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public abstract void WriteDescriptionTo(MessageWriter writer);

        /// <summary>
		/// Write the actual value for a failing constraint test to a
		/// MessageWriter. The default implementation simply writes
		/// the raw value of actual, leaving it to the writer to
		/// perform any formatting.
		/// </summary>
		/// <param name="writer">The writer on which the actual value is displayed</param>
		public virtual void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue( actual );
		}
		#endregion

        #region ToString Override
        /// <summary>
        /// Default override of ToString returns the constraint DisplayName
        /// followed by any arguments within angle brackets.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (argcnt)
            {
                default:
                case 0:
                    return string.Format("<{0}>", DisplayName);
                case 1:
                    return string.Format("<{0} {1}>", DisplayName, _displayable(arg1));
                case 2:
                    return string.Format("<{0} {1} {2}>", DisplayName, _displayable(arg1), _displayable(arg2));
            }
        }

        private string _displayable(object o)
        {
            if (o == null) return "null";

            string fmt = o is string ? "\"{0}\"" : "{0}";
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, fmt, o);
        }
        #endregion

        #region Operator Overloads
        /// <summary>
        /// This operator creates a constraint that is satisfied only if both 
        /// argument constraints are satisfied.
        /// </summary>
        public static Constraint operator &(Constraint left, Constraint right)
        {
            IResolveConstraint l = (IResolveConstraint)left;
            IResolveConstraint r = (IResolveConstraint)right;
            return new AndConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if either 
        /// of the argument constraints is satisfied.
        /// </summary>
        public static Constraint operator |(Constraint left, Constraint right)
        {
            IResolveConstraint l = (IResolveConstraint)left;
            IResolveConstraint r = (IResolveConstraint)right;
            return new OrConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if the 
        /// argument constraint is not satisfied.
        /// </summary>
        public static Constraint operator !(Constraint constraint)
        {
            IResolveConstraint r = constraint as IResolveConstraint;
            return new NotConstraint(r == null ? new NullConstraint() : r.Resolve());
        }
        #endregion

        #region Binary Operators
        /// <summary>
        /// Returns a ConstraintExpression by appending And
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression And
        {
            get
            {
                ConstraintBuilder builder = this.builder;
                if (builder == null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new AndOperator());

                return new ConstraintExpression(builder);
            }
        }

        /// <summary>
        /// Returns a ConstraintExpression by appending And
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression With
        {
            get { return this.And; }
        }

        /// <summary>
        /// Returns a ConstraintExpression by appending Or
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression Or
        {
            get
            {
                ConstraintBuilder builder = this.builder;
                if (builder == null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new OrOperator());

                return new ConstraintExpression(builder);
            }
        }
        #endregion

        #region After Modifier
        /// <summary>
        /// Returns a DelayedConstraint with the specified delay time.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint After(int delayInMilliseconds)
        {
            return new DelayedConstraint(
                builder == null ? this : builder.Resolve(),
                delayInMilliseconds);
        }

        /// <summary>
        /// Returns a DelayedConstraint with the specified delay time
        /// and polling interval.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <param name="pollingInterval">The interval at which to test the constraint.</param>
        /// <returns></returns>
        public DelayedConstraint After(int delayInMilliseconds, int pollingInterval)
        {
            return new DelayedConstraint(
                builder == null ? this : builder.Resolve(),
                delayInMilliseconds,
                pollingInterval);
        }
        #endregion

        #region IResolveConstraint Members
        Constraint IResolveConstraint.Resolve()
        {
            return builder == null ? this : builder.Resolve();
        }
        #endregion
    }

#if NET_2_0
    public abstract class Constraint<T> : Constraint
    {
        private int argcnt;
        private T arg1;
        private T arg2;

        #region Constructors
        /// <summary>
        /// Construct a constraint with no arguments
        /// </summary>
        public Constraint()
        {
            argcnt = 0;
        }

        /// <summary>
        /// Construct a constraint with one argument
        /// </summary>
        public Constraint(T arg)
        {
            argcnt = 1;
            this.arg1 = arg;
        }

        /// <summary>
        /// Construct a constraint with two arguments
        /// </summary>
        public Constraint(T arg1, T arg2)
        {
            argcnt = 2;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
        #endregion        

        #region ToString Override
        /// <summary>
        /// Default override of ToString returns the constraint DisplayName
        /// followed by any arguments within angle brackets.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (argcnt)
            {
                default:
                case 0:
                    return string.Format("<{0}>", DisplayName);
                case 1:
                    return string.Format("<{0} {1}>", DisplayName, _displayable(arg1));
                case 2:
                    return string.Format("<{0} {1} {2}>", DisplayName, _displayable(arg1), _displayable(arg2));
            }
        }

        private string _displayable(T arg)
        {
            if (arg == null) return "null";

            string fmt = arg is string ? "\"{0}\"" : "{0}";
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, fmt, arg);
        }
        #endregion
    }
#endif
}
