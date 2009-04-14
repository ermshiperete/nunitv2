﻿using System;
using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualityAdapter class handles all equality comparisons
    /// that use an IEqualityComparer, IEqualityComparer&lt;T&gt;
    /// or a ComparisonAdapter.
    /// </summary>
    public abstract class EqualityAdapter
    {
        /// <summary>
        /// Compares two objects, returning true if they are equal
        /// </summary>
        public abstract bool Equals(object x, object y);

        /// <summary>
        /// Returns the default EqualityAdapter, which wraps an
        /// instance of the standard NUnitEqualityComparer.
        /// </summary>
        public static EqualityAdapter Default
        {
            get { return new DefaultEqualityAdapter(); }
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IComparer.
        /// </summary>
        public static EqualityAdapter For(IComparer comparer)
        {
            return new ComparisonAdapterAdapter(ComparisonAdapter.For(comparer));
        }

#if NET_2_0
        /// <summary>
        /// Returns an EqualityAdapter that wraps an IEqualityComparer.
        /// </summary>
        public static EqualityAdapter For(IEqualityComparer comparer)
        {
            return new EqualityComparerAdapter(comparer);
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IEqualityComparer&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
        {
            return new EqualityComparerAdapter<T>(comparer);
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IComparer&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(IComparer<T> comparer)
        {
            return new ComparisonAdapterAdapter( ComparisonAdapter.For(comparer) );
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps a Comparison&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(Comparison<T> comparer)
        {
            return new ComparisonAdapterAdapter( ComparisonAdapter.For(comparer) );
        }

        class EqualityComparerAdapter : EqualityAdapter
        {
            private IEqualityComparer comparer;

            public EqualityComparerAdapter(IEqualityComparer comparer)
            {
                this.comparer = comparer;
            }

            public override bool Equals(object x, object y)
            {
                return comparer.Equals(x, y);
            }
        }

        class EqualityComparerAdapter<T> : EqualityAdapter
        {
            private IEqualityComparer<T> comparer;

            public EqualityComparerAdapter(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool Equals(object x, object y)
            {
                if (!typeof(T).IsAssignableFrom(x.GetType()))
                    throw new ArgumentException("Cannot compare " + x.ToString());

                if (!typeof(T).IsAssignableFrom(y.GetType()))
                    throw new ArgumentException("Cannot compare to " + y.ToString());

                return comparer.Equals((T)x, (T)y);
            }
        }
#endif

        class DefaultEqualityAdapter : EqualityAdapter
        {
            public override bool Equals(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        class ComparisonAdapterAdapter : EqualityAdapter
        {
            private ComparisonAdapter comparer;

            public ComparisonAdapterAdapter(ComparisonAdapter comparer)
            {
                this.comparer = comparer;
            }

            public override bool Equals(object x, object y)
            {
                return comparer.Compare(x, y) == 0;
            }
        }
    }
}
