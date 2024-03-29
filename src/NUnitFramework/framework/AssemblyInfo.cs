// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Reflection;
using System.Security;

[assembly: CLSCompliant(true)]
#if !NET_4_0
[assembly: AllowPartiallyTrustedCallers]
#endif

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("../../nunit.snk")]
[assembly: AssemblyKeyName("")]
