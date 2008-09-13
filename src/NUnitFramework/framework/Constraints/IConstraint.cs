// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        bool Matches(object actual);

        /// <summary>
        /// Write the failure message to a MessageWriter.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display the message</param>
        void WriteMessageTo(MessageWriter writer);

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        void WriteDescriptionTo(MessageWriter writer);

        /// <summary>
        /// Write the actual value for a failing constraint test to a
        /// MessageWriter.
        /// </summary>
        /// <param name="writer">The writer on which the actual value is displayed</param>
        void WriteActualValueTo(MessageWriter writer);
    }
}
