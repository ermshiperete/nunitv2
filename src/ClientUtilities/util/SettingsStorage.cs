namespace NUnit.Util
{
	using System;
	using System.IO;

	/// <summary>
	/// Abstract class representing a hierarchical storage used to hold
	/// application settings. The actual implementation is left to 
	/// derived classes, and may be based on the registry, isolated
	/// storage or any other mechanism.
	/// </summary>
	public abstract class SettingsStorage : IDisposable
	{
		#region Instance Variables

		/// <summary>
		/// The name of this storage
		/// </summary>
		private string storageName;
		
		/// <summary>
		/// The parent storage containing this storage
		/// </summary>
		private SettingsStorage parentStorage;
		#endregion

		#region Construction and Disposal

		/// <summary>
		/// Construct a SettingsStorage under a parent storage
		/// </summary>
		/// <param name="storageName">Name of the storage</param>
		/// <param name="parentStorage">The parent which contains the new storage</param>
		public SettingsStorage( string storageName, SettingsStorage parentStorage )
		{
			this.storageName = storageName;
			this.parentStorage = parentStorage;
		}

		/// <summary>
		/// Dispose of resources held by this storage
		/// </summary>
		public abstract void Dispose();

		#endregion

		#region Properties

		/// <summary>
		/// The number of settings in this group
		/// </summary>
		public abstract int SettingsCount
		{
			get;
		}

		/// <summary>
		/// The name of the storage
		/// </summary>
		public string StorageName
		{
			get { return storageName; }
		}

		/// <summary>
		/// The storage that contains this one
		/// </summary>
		public SettingsStorage ParentStorage
		{
			get { return parentStorage; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Find out if a substorage exists
		/// </summary>
		/// <param name="name">Name of the child storage</param>
		/// <returns>True if the storage exists</returns>
		public abstract bool ChildStorageExists( string name );

		/// <summary>
		/// Create a child storage of the same type
		/// </summary>
		/// <param name="name">Name of the child storage</param>
		/// <returns>New child storage</returns>
		public abstract SettingsStorage MakeChildStorage( string name );

		/// <summary>
		/// Clear all settings from the storage - empty storage remains
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Load a setting from the storage
		/// </summary>
		/// <param name="settingName">Name of the setting to load</param>
		/// <returns>Value of the setting or null</returns>
		public abstract object LoadSetting( string settingName );

		/// <summary>
		/// Remove a setting from the storage
		/// </summary>
		/// <param name="settingName">Name of the setting to remove</param>
		public abstract void RemoveSetting( string settingName );

		/// <summary>
		/// Save a setting in the storage
		/// </summary>
		/// <param name="settingName">Name of the setting to save</param>
		/// <param name="settingValue">Value to be saved</param>
		public abstract void SaveSetting( string settingName, object settingValue );

		#endregion
	}
}
