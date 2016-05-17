//ConfigFolder.cs - Description
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

namespace System.Data.Bindings.Utilities
{
	/// <summary>
	/// Corresponds to folder.
	/// </summary>
	public class ConfigFolder
	{
		private ArrayList list = new ArrayList();

		/// <summary>
		/// Checks if this folder is master folder
		/// </summary>
		public bool IsMasterConfig {
			get { return (ApplicationPreferences.GetMasterFolder() == this); }
		}

		/// <summary>
		/// Contains folder to be used to store config files 
		/// </summary>
		public string Path {
			get {
				string path;
				if (IsMasterConfig == true)
					path = ApplicationPreferences.ConfigurationFolder + Name;
				else
					path = ApplicationPreferences.ConfigurationFolder +
					       System.Reflection.Assembly.GetEntryAssembly().GetName().Name +
					       System.IO.Path.DirectorySeparatorChar + Name;
				if (System.IO.Directory.Exists(path) == false)
					System.IO.Directory.CreateDirectory (path);
				return (path);
			}
		}

		private string name = "";
		/// <summary>
		/// Name of folder which contains config files 
		/// </summary>
		public string Name {
			get { return (name); }
		}

		/// <value>
		/// Returns configuration count in this folder
		/// </value>
		public int Count {
			get { return (list.Count); }
		}
		
		/// <summary>
		/// Retrives config file object by name
		/// </summary>
		/// <param name="aFileName">
		/// FileName config object to retrieve <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// null if configuration doesn't exists, otherwise it returns 
		/// ConfigFile object with that name
		/// </returns>
		public ConfigFile Get (string aFileName)
		{
			if (aFileName == "")
				aFileName = ApplicationPreferences.MasterConfiguration;
			foreach (ConfigFile file in list)
				if (file.FileName.ToLower() == aFileName.ToLower())
					return (file);
			return (null);
		}

		/// <summary>
		/// Executes LoadAll() for all config folders registered
		/// </summary>
		public void LoadAll()
		{
			foreach (ConfigFile file in list)
				file.Load();
		}
		
		/// <summary>
		/// Executes SaveAll() for all config folders registered
		/// </summary>
		public void SaveAll()
		{
			foreach (ConfigFile file in list)
				file.Save();
		}
		
		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <remarks>
		/// This will create ConfigFile with
		///   FileName = ApplicationPreferences.MasterConfiguration
		///   Watched = false
		/// </remarks>
		public void Add (System.Type aSerializableObjectType)
		{
			list.Add (new ConfigFile (this, aSerializableObjectType));
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
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
		public void Add (System.Type aSerializableObjectType, bool aWatched)
		{
			list.Add (new ConfigFile (this, aSerializableObjectType, aWatched));
		}

		/// <summary>
		/// Creates ConfigFile 
		/// </summary>
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
		public void Add (string aFileName, System.Type aSerializableObjectType)
		{
			list.Add (new ConfigFile (this, aFileName, aSerializableObjectType));
		}

		/// <summary>
		/// Creates and adds ConfigFile to this folder 
		/// </summary>
		/// <param name="aFileName">
		/// Filename of this config file <see cref="System.String"/>
		/// </param>
		/// <param name="aSerializableObjectType">
		/// Object type to be loaded with values <see cref="System.Type"/>
		/// </param>
		/// <param name="aWatched">
		/// true if file should be watched <see cref="System.Boolean"/>
		/// </param>
		public void Add (string aFileName, System.Type aSerializableObjectType, bool aWatched)
		{
			list.Add (new ConfigFile (this, aFileName, aSerializableObjectType, aWatched));
		}

		/// <summary>
		/// Creates ConfigFolder
		/// </summary>
		/// <remarks>
		/// Throws Exception
		/// </remarks>
		private ConfigFolder()
		{
			throw new Exception ("ConfigFolder initialized abnormally");
		}
		
		/// <summary>
		/// Creates ConfigFolder
		/// </summary>
		/// <param name="aName">
		/// Name of config folder <see cref="System.String"/>
		/// </param>
		public ConfigFolder (string aName)
		{
			if (aName == "") {
				if (ApplicationPreferences.GetMasterFolder() != null)
					throw new Exception ("Error: There can't be two ConfigFolder master containers");
			}
			else
				if (ApplicationPreferences.GetFolder(aName) != null)
					throw new Exception ("Error: There can't be two ConfigFolder with same name");
					
			if (aName == "") {
				name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
				return;
			}
			name = aName; 
		}
	}
}
