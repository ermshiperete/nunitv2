using System;
using Microsoft.Win32;

namespace NUnit.Tests
{
	using NUnit.Util;
	using NUnit.Framework;

	/// <summary>
	/// Summary description for RegistryStorageTests.
	/// </summary>
	[TestFixture]
	public class RegistrySettingsStorageTests
	{
		RegistryKey testKey;

		public RegistrySettingsStorageTests()
		{
		}

		[SetUp]
		public void BeforeEachTest()
		{
			testKey = Registry.CurrentUser.CreateSubKey( "Software\\NunitTest" );
		}

		[TearDown]
		public void AfterEachTest()
		{
			testKey.Close();
			Registry.CurrentUser.DeleteSubKeyTree( "Software\\NunitTest" );
		}

		[Test]
		public void MakeRegistryStorage()
		{
			RegistrySettingsStorage storage = new RegistrySettingsStorage( "Test", testKey );

			Assertion.AssertNotNull( storage );
			Assertion.AssertEquals( "Test", storage.StorageName );
			Assertion.AssertNull( storage.ParentStorage );
			Assertion.AssertNotNull( "Null storage key", storage.StorageKey );
		}

		[Test]
		public void SaveAndLoadSettings()
		{
			RegistrySettingsStorage storage = new RegistrySettingsStorage( "Test", testKey );
			
			Assertion.AssertNull( "X is not null", storage.LoadSetting( "X" ) );
			Assertion.AssertNull( "NAME is not null", storage.LoadSetting( "NAME" ) );

			storage.SaveSetting("X", 5);
			storage.SaveSetting("NAME", "Charlie");

			Assertion.AssertEquals( 5, storage.LoadSetting("X") );
			Assertion.AssertEquals( "Charlie", storage.LoadSetting("NAME") );

			using( RegistryKey key = testKey.OpenSubKey( "Test" ) )
			{
				Assertion.AssertNotNull( key );
				Assertion.AssertEquals( 5, key.GetValue( "X" ) );
				Assertion.AssertEquals( "Charlie", key.GetValue( "NAME" ) );
			}
		}

		[Test]
		public void RemoveSettings()
		{
			RegistrySettingsStorage storage = new RegistrySettingsStorage( "Test", testKey );
			
			storage.SaveSetting("X", 5);
			storage.SaveSetting("NAME", "Charlie");

			storage.RemoveSetting( "X" );
			Assertion.AssertNull( "X not removed", storage.LoadSetting( "X" ) );
			Assertion.AssertEquals( "Charlie", storage.LoadSetting( "NAME" ) );

			storage.RemoveSetting( "NAME" );
			Assertion.AssertNull( "NAME not removed", storage.LoadSetting( "NAME" ) );
		}

		[Test]
		public void MakeSubStorages()
		{
			RegistrySettingsStorage storage = new RegistrySettingsStorage( "Test", testKey );
			RegistrySettingsStorage sub1 = new RegistrySettingsStorage( "Sub1", storage );
			RegistrySettingsStorage sub2 = new RegistrySettingsStorage( "Sub2", storage );

			Assertion.AssertNotNull( "Sub1 is null", sub1 );
			Assertion.AssertNotNull( "Sub2 is null", sub2 );

			Assertion.AssertEquals( "Sub1", sub1.StorageName );
			Assertion.AssertEquals( "Sub2", sub2.StorageName );
		}

		[Test]
		public void SubstorageSettings()
		{
			RegistrySettingsStorage storage = new RegistrySettingsStorage( "Test", testKey );
			RegistrySettingsStorage sub = new RegistrySettingsStorage( "Sub", storage );

			sub.SaveSetting( "X", 5 );
			sub.SaveSetting( "NAME", "Charlie" );

			Assertion.AssertEquals( 5, sub.LoadSetting( "X" ) );
			Assertion.AssertEquals( "Charlie", sub.LoadSetting( "NAME" ) );

			sub.RemoveSetting( "X" );
			Assertion.AssertNull( "X not removed", sub.LoadSetting( "X" ) );
			
			Assertion.AssertEquals( "Charlie", sub.LoadSetting( "NAME" ) );

			sub.RemoveSetting( "NAME" );
			Assertion.AssertNull( "NAME not removed", sub.LoadSetting( "NAME" ) );
		}
	}
}
