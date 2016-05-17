//ConfigFile.cs - Description
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) 2008 m.
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of version 2 of the Lesser GNU General 
//Public License as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA 02111-1307, USA.
//
//

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Data.Bindings.DebugInformation;
using System.Threading;

namespace System.Data.Bindings.Utilities
{
	/// <summary>
	/// Defines where config file should be stored and which object to
	/// use to contain preferences. Object containing preferences must
	/// be Serializable.
	/// </summary>
	/// <remarks>
	/// Acceptable objects are only serializable, otherwise throws Exception
	/// </remarks>
	public class ConfigFile
	{
		private int modification = 0;
		private System.Type SerializableObjectType;
		private IO.FileSystemWatcher watcher = null;
		
		private ConfigFolder folder = null;
		/// <value>
		/// Returns Folder in which this config file belongs
		/// </value>
		public ConfigFolder Folder {
			get { return (folder); }
		}
		
		private string filename = ApplicationPreferences.MasterConfiguration;
		/// <summary>
		/// Specifies filename to be used for config file
		/// </summary>
		public string FileName {
			get { return (filename); }
			set {
				if (value == "")
					filename = ApplicationPreferences.MasterConfiguration;
				filename = value; 
			}
		}

		private object configuration = null;
		/// <value>
		/// Contains object loaded with preferences
		/// </value>
		/// <remarks>
		/// In case of using System.Data.Bindings type of adaptors, this property
		/// is unsuitable to provide as DataSource. User ConfigAdaptor instead
		/// </remarks>
		public object Configuration {
			get { return (configuration); }
		}

		/// <summary>
		/// Path to config file
		/// </summary>
		public string Path {
			get { return (Folder.Path + System.IO.Path.DirectorySeparatorChar + FileName); }
		}

		private bool watched = false;
		/// <summary>
		/// Defines if config file is watched or not 
		/// </summary>
		public bool Watched {
			get { return (watched); }
			set {
				if (watched == value)
					return;
				watched = value;
				EnableWatcher (value);
			}
		}
		
		private bool autosaveOnQuit = true;
		/// <summary>
		/// Defines if config file should be saved before quiting application
		/// </summary>
		/// <remarks>
		/// Default value is true
		/// </remarks>
		public bool AutosaveOnQuit {
			get { return (autosaveOnQuit); }
			set { autosaveOnQuit = value; }
		}
 
		private System.Data.Bindings.Adaptor configAdaptor = new System.Data.Bindings.Adaptor();
		/// <value>
		/// Provides Pointer like Binding Adaptor to this configuration
		/// </value>
		public System.Data.Bindings.Adaptor ConfigAdaptor {
			get { return (configAdaptor); }
		}

		/// <summary>
		/// Handles event when config file changes from outside
		/// </summary>
		/// <param name="sender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Arguments <see cref="FileSystemEventArgs"/>
		/// </param>
		private void FileChanged (object sender, FileSystemEventArgs e)
		{
			modification++;
			int i = modification;
			Thread.Sleep (500);
			if (i == modification) {
				Load();
				modification = 0;
			}
		}
		
		/// <summary>
		/// Handles event when config file is created from outside
		/// </summary>
		/// <param name="sender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Arguments <see cref="FileSystemEventArgs"/>
		/// </param>
		private void FileCreated (object sender, FileSystemEventArgs e)
		{
			modification++;
			int i = modification;
			Thread.Sleep (500);
			if (i == modification) {
				Load();
				modification = 0;
			}
		}
		
		/// <summary>
		/// Handles event when config file is deleted from outside
		/// </summary>
		/// <param name="sender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Arguments <see cref="FileSystemEventArgs"/>
		/// </param>
		private void FileDeleted (object sender, FileSystemEventArgs e)
		{
			RestoreToDefaults();
		}
		
		/// <summary>
		/// This is just the same case as deleted here
		/// </summary>
		/// <param name="sender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Arguments <see cref="RenamedEventArgs"/>
		/// </param>
		private void FileRenamed (object sender, RenamedEventArgs e)
		{
			RestoreToDefaults();
		}
		
		private void EnableWatcher (bool aEnabled)
		{
			if (watcher == null)
				if (aEnabled == false)
					return;
				else
					watcher = new FileSystemWatcher (Folder.Path, FileName);			

			if ((aEnabled == true) && (watcher.EnableRaisingEvents == false)) {
				watcher.Changed += FileChanged;
				watcher.Deleted += FileDeleted;
				watcher.Created += FileCreated;
				watcher.Renamed += FileRenamed;
				watcher.EnableRaisingEvents = true;
			}
			else {
				watcher.EnableRaisingEvents = false;
				watcher.Changed -= FileChanged;
				watcher.Deleted -= FileDeleted;
				watcher.Created -= FileCreated;
				watcher.Renamed -= FileRenamed;
			}
		}
		
		/// <summary>
		/// Creates new configuration object instance
		/// </summary>
		/// <returns>
		/// Returns instance of the new configuration object <see cref="System.Object"/>
		/// </returns>
		public object CreateConfigTypeInstance()
		{
			foreach (ConstructorInfo cons in SerializableObjectType.GetConstructors())
				if (cons.GetParameters().Length == 0) {
					return (cons.Invoke (null));
			}
			return (null);
		}
    
		/// <summary>
		/// Forces loading of the config file
		/// </summary>
		public void Load()
		{
			try {
				if (System.IO.File.Exists(Path) == true) {
					try {
						object newconfiguration = null;

						XmlSerializer serializer = new XmlSerializer (SerializableObjectType);
					
						TextReader reader = new StreamReader (Path);
						newconfiguration = serializer.Deserialize (reader);
						reader.Close();
						reader = null;

						// Sets Adaptor on new target
						if (System.Data.Bindings.TypeValidator.IsCompatible(newconfiguration.GetType(), SerializableObjectType) == true) {
							ConfigAdaptor.Target = newconfiguration;
							configuration = null;
							configuration = newconfiguration;
						}
						else {
							Debug.Warning ("ConfigFile.Load()", 
							               "Warning: configuration " + FileName + " was loaded but serializable type is doubtfull\n" +
							               "         loadedType: " + newconfiguration.GetType() + 
							               " originalType: " + SerializableObjectType);
						}
					}
					catch { 
						Debug.Warning ("configuration " + FileName + " is corrupted, using defaults", "");
						if (configuration == null)
							RestoreToDefaults();
					}
				}
				else
					if (configuration == null)
						RestoreToDefaults();
			}
			finally {
				ApplicationPreferences.NotifyClients (Configuration);
			}
		}
		
		/// <summary>
		/// Forces saving of the config file
		/// </summary>
		public void Save()
		{
			if (Configuration == null) {
				Console.Error.WriteLine ("Error: ConfigFile " + FileName + " tried to save null object");
				return;
			}
			// Store watched property
			bool w = watched;
			// Disable watching of the file to avoid unnecessary triggering
			if (w == true)
				Watched = false;

			// Create folder to store in if needed
			ApplicationPreferences.CreateFolder (Folder);
			
			// Load from configuration
			XmlSerializer serializer = new XmlSerializer (SerializableObjectType);
			TextWriter writer = new StreamWriter (Path);
			serializer.Serialize (writer, Configuration);
			writer.Close();
			serializer = null;
			writer = null;

			// Enable watching of the file if it was enabled before saving
			if (w == true)
				Watched = true;
		}

		/// <summary>
		/// Restores Configuration by executing CreateConfigTypeInstance()
		/// and sets ConfigAdaptor to this new object
		/// </summary>
		public void RestoreToDefaults()
		{
			configuration = null;
			configuration = CreateConfigTypeInstance();
			ConfigAdaptor.Target = Configuration;
		}
		
		/// <summary>
		/// Throws exception
		/// </summary>
		private ConfigFile()
		{
			throw new Exception ("Cannot create undefined config file");
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
		/// <param name="aFolder">
		/// Folder containing this configuration <see cref="ConfigFolder"/>
		/// </param>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <remarks>
		/// This will create ConfigFile with
		///   FileName = ApplicationPreferences.MasterConfiguration
		///   Watched = false
		/// </remarks>
		public ConfigFile (ConfigFolder aFolder, System.Type aSerializableObjectType)
			: this (aFolder, "", aSerializableObjectType, false)
		{
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
		/// <param name="aFolder">
		/// Folder containing this configuration <see cref="ConfigFolder"/>
		/// </param>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <param name="aWatched">
		/// true if file should be watched <see cref="System.Boolean"/>
		/// </param>
		/// <remarks>
		/// This will create ConfigFile with
		///   FileName = ApplicationPreferences.MasterConfiguration
		/// </remarks>
		public ConfigFile (ConfigFolder aFolder, System.Type aSerializableObjectType, bool aWatched)
			: this (aFolder, "", aSerializableObjectType, aWatched)
		{
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
		/// <param name="aFolder">
		/// Folder containing this configuration <see cref="ConfigFolder"/>
		/// </param>
		/// <param name="aFileName">
		/// Filename of this config file <see cref="System.String"/>
		/// </param>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <remarks>
		/// This will create ConfigFile with
		///   Watched = false
		/// </remarks>
		public ConfigFile (ConfigFolder aFolder, string aFileName, System.Type aSerializableObjectType)
			: this (aFolder, aFileName, aSerializableObjectType, false)
		{
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
		/// <param name="aFolder">
		/// Folder containing this configuration <see cref="ConfigFolder"/>
		/// </param>
		/// <param name="aFileName">
		/// Filename of this config file <see cref="System.String"/>
		/// </param>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <param name="aWatched">
		/// true if file should be watched <see cref="System.Boolean"/>
		/// </param>
		public ConfigFile (ConfigFolder aFolder, string aFileName, System.Type aSerializableObjectType, bool aWatched)
		{
			if (aFolder == null)
				throw new Exception ("Config file can;t be created without master folder");
			
			folder = aFolder;
			if (aFileName.Trim() == "")
				aFileName = ApplicationPreferences.MasterConfiguration;
			
			if (folder.Get(aFileName) != null)
				throw new Exception ("Config file with name " + aFileName + " already exists in folder " + aFolder.Name);
			FileName = aFileName;
			SerializableObjectType = aSerializableObjectType;
			Watched = aWatched;
			try {
				XmlSerializer serializer = new XmlSerializer (SerializableObjectType);
				serializer = null;
			}
			catch { throw new Exception ("Type " + aSerializableObjectType + " is not valid class for serialization"); }
			
			Load();
		}

		/// <summary>
		/// Saves configuration if AutosaveOnQuit is enabled and destroys object
		/// </summary>
		~ConfigFile()
		{
			if (AutosaveOnQuit == true)
				Save();
		}
	}
}
