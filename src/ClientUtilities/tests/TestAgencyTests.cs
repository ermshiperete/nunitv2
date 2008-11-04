using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Core;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace NUnit.Util.Tests
{
    [TestFixture, Platform(Exclude = "Mono")]
    public class TestAgencyTests
    {
        private TestAgency agency;

        [SetUp]
        public void CreateAgency()
        {
            agency = new TestAgency("TempTestAgency", 9300);
            agency.Start();
        }

        [TearDown]
        public void StopAgency()
        {
            agency.Stop();
        }

        [Test]
        public void CanConnectToAgency()
        {
            object obj = Activator.GetObject(typeof(TestAgency), ServerUtilities.MakeUrl("TempTestAgency", 9300));
            Assert.IsNotNull(obj);
            Assert.That(obj is TestAgency);
        }

        [Test]
        public void CanLaunchAndConnectToAgent()
        {
            TestAgent agent = null;
            try
            {
                agent = agency.GetAgent(10000);
                Assert.IsNotNull(agent);
            }
            finally
            {
                if ( agent != null )
                    agency.ReleaseAgent(agent);
            }
        }

        // TODO: Decide if we really want to do this
        //[Test]
        public void CanReuseReleasedAgents()
        {
            TestAgent agent1 = agency.GetAgent(20000);
            Guid id1 = agent1.Id;
            agency.ReleaseAgent(agent1);
            TestAgent agent2 = agency.GetAgent(20000);
            Assert.AreEqual(id1, agent2.Id);
        }
    }
}
