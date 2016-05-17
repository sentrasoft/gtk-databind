//ApplicationPreferences.cs - Description
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
using System.Collections;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings.Utilities
{
	/// <summary>
	/// Provides engine to load and save preferences
	/// </summary>	
	public static class ApplicationPreferences
	{
		private static ArrayList list = new ArrayList();
		private static bool warningshown = false;

		private static string masterConfiguration = "config.xml";
		/// <value>
		/// Default name of master configuration 
		/// </value>
		/// <remarks>
		/// By default specified as config.xml
		/// </remarks>
		public static string MasterConfiguration {
			get { return (masterConfiguration); }
		}
		
		/// <value>
		/// Returns folder count
		/// </value>
		public static int Count {
			get { return (list.Count); }
		}
		
		private static PreferencesType applicationType = PreferencesType.None;
		/// <value>
		/// Sets type of application, which describes where config files are
		/// stored by default
		/// </value>
		/// <remarks>
		/// When application starts using ApplicationPreferences it should
		/// set this property. Default value otherwise will be
		/// PreferencesType.None and warning will be displayed the first time
		/// unset property is accessed
		/// </remarks>
		public static PreferencesType ApplicationType {
			get {
				if (applicationType == PreferencesType.None) {
					if (warningshown == false) {
						warningshown = true;
						Debug.Warning ("Warning: ApplicationPreferences.ApplicationType is not set!",
						               "using PreferencesType.Application by defaut");
					}
					applicationType = PreferencesType.Application;
				}
				return (applicationType);
			}
		}
		
		/// <summary>
		/// Returns ApplicationData with ContainerFolder as subfolder
		/// </summary>
		public static string ConfigurationFolder {
			get {
				if (ApplicationType == PreferencesType.Application) 
					return (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData) + 
				           System.IO.Path.DirectorySeparatorChar);
				else
					if (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData) == "/usr/share")
						return ("/etc/");
					else
						return (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData) + 
					           System.IO.Path.DirectorySeparatorChar);
			}
		}

		/// <summary>
		/// Notifies clients connected to ApplicationPreferences that
		/// one of config objects has changed
		/// </summary>
		/// <param name="aObject">
		/// Object that has changed <see cref="System.Object"/>
		/// </param>
		public static void NotifyClients (object aObject)
		{
			if (aObject == null)
				return;
			if (onConfigChanged != null)
				onConfigChanged (aObject);
		}

		/// <summary>
		/// Creates folder in its default storage point
		/// </summary>
		/// <param name="aFolder">
		/// Folder name <see cref="ConfigFolder"/>
		/// </param>
		public static void CreateFolder (ConfigFolder aFolder)
		{
			if (IO.Directory.Exists(ConfigurationFolder) == false)
				IO.Directory.CreateDirectory (ConfigurationFolder);
			if (aFolder == null)
				return;
			if (IO.Directory.Exists(aFolder.Path) == false)
				IO.Directory.CreateDirectory (aFolder.Path);
		}
		
		/// <summary>
		/// Creates master configuration
		/// </summary>
		/// <param name="aSerializableObjectType">
		/// Default config type <see cref="System.Type"/>
		/// </param>
		/// <param name="aWatched">
		/// Set true if configuration should be monitored for file changes <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Config file created <see cref="ConfigFile"/>
		/// </returns>
		public static ConfigFile AddMasterConfiguration (System.Type aSerializableObjectType, bool aWatched)
		{
			ConfigFolder folder = GetMasterFolder();
			if (folder == null) {
				folder = new ConfigFolder ("");
				list.Add (folder);
			}
			if (folder.Get("config.xml") != null)
				throw new Exception ("Master configuration already exists");
			folder.Add (aSerializableObjectType, aWatched);
			return (folder.Get ("config.xml"));
		}
		
		/// <summary>
		/// Returns master configuration folder
		/// </summary>
		/// <returns>
		/// Config folder with "" name <see cref="ConfigFolder"/>
		/// </returns>
		public static ConfigFolder GetMasterFolder()
		{
			foreach (ConfigFolder folder in list)
				if (folder.Name.ToLower() == System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToLower())
					return (folder);
			return (null);
		}
		
		/// <summary>
		/// Returns configuration folder with specified name
		/// </summary>
		/// <param name="aName">
		/// Name of the searched folder <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Name of the searched config folder <see cref="ConfigFolder"/>
		/// </returns>
		public static ConfigFolder GetFolder(string aName)
		{
			foreach (ConfigFolder folder in list)
				if (folder.Name.ToLower() == aName.ToLower())
					return (folder);
			return (null);
		}
		
		/// <summary>
		/// Returns master configuration file 
		/// </summary>
		/// <returns>
		/// Config file with "config.xml" name <see cref="ConfigFolder"/>
		/// </returns>
		public static ConfigFile GetMasterConfiguration()
		{
			ConfigFolder folder = GetMasterFolder();
			if (folder != null)
				return (folder.Get ("config.xml"));
			return (null);
		}
		
		/// <summary>
		/// Executes LoadAll() for all config folders registered
		/// </summary>
		public static void LoadAll()
		{
			foreach (ConfigFolder folder in list)
				folder.LoadAll();
		}
		
		/// <summary>
		/// Executes SaveAll() for all config folders registered
		/// </summary>
		public static void SaveAll()
		{
			foreach (ConfigFolder folder in list)
				folder.SaveAll();
		}
		
		private static event ConfigChangedEvent onConfigChanged = null;
		/// <summary>
		/// Event is called when configuration is changed externally or
		/// when ConfigFile.Load() is called
		/// </summary>
		public static event ConfigChangedEvent OnConfigChanged {
			add { onConfigChanged += value; }
			remove { onConfigChanged -= value; }
		}
	}
}
