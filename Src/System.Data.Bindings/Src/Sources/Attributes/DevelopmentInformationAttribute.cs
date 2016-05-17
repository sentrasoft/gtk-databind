//  
//  Copyright (C) 2009 matooo
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides ground to specify in-source development information to application
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=false)]
	public class DevelopmentInformationAttribute : Attribute
	{
		private string taskName = "Unspecified";
		/// <value>
		/// Task name
		/// </value>
		public string TaskName {
			get { return (taskName); }
		}
		
		public string Name {
			get { return (TaskName); }
		}
		
		private string description = "";
		/// <value>
		/// Returns description
		/// </value>
		public string Description {
			get { return (description); }
		}
		
		private DevelopmentInformationItem[] items = null;
		/// <value>
		/// 
		/// </value>
		public DevelopmentInformationItem[] Items {
			get { return (items); }
		}
		
		/// <value>
		/// Returns item count
		/// </value>
		public int Count {
			get {
				if (items == null)
					return (0);
				return (items.Length);
			}
		}
		
		/// <value>
		/// Returns development information item
		/// </value>
		public DevelopmentInformationItem this [int aIndex] {
			get {
				if (Count == 0)
					return (null);
				return (items[aIndex]);
			}
		}
		
		/// <summary>
		/// Searches for location of item
		/// </summary>
		/// <param name="aItem">
		/// Searched item <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		public int IndexOf (object aItem)
		{
			for (int i=0; i<Count; i++)
				if (this[i] == aItem)
					return (i);
			return (-1);
		}
		
		/// <summary>
		/// Returns specified task status
		/// </summary>
		/// <param name="aIndex">
		/// Task index <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Development status <see cref="DevelopmentStatus"/>
		/// </returns>
		public DevelopmentStatus GetTaskStatus (int aIndex) 
		{
			if (Count == 0)
				return (DevelopmentStatus.None);
			return (items[aIndex].Status);
		}
		
		/// <summary>
		/// Returns specified task description
		/// </summary>
		/// <param name="aIndex">
		/// Task index <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Development description <see cref="System.String"/>
		/// </returns>
		public string GetTaskDescription (int aIndex) 
		{
			if (Count == 0)
				return ("");
			return (items[aIndex].Description);
		}
		
		/// <summary>
		/// Returns specified task name
		/// </summary>
		/// <param name="aIndex">
		/// Task index <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Development description <see cref="System.String"/>
		/// </returns>
		public string GetTaskName (int aIndex) 
		{
			if (Count == 0)
				return ("");
			return (items[aIndex].Name);
		}
		
		private DevelopmentInformationAttribute()
		{
		}

		private DevelopmentInformationItem Parse (string aInformation)
		{
			string information = aInformation;
			information = information.Trim();
			if (information == "")
				return (null);
			int i = information.IndexOf('[');
			if (i == -1)
				return (new DevelopmentInformationItem (information));
			string status = information.Substring (0, i).Trim().ToLower();
			information = information.Remove (0, i);
			i = information.IndexOf(']');
			if (i == -1)
				throw new Exception (string.Format ("Wrong parsing of development information item\n{0}", aInformation));
			string title = information.Substring (0, i+1).Trim();
			title = title.Remove (0, 1);
			title = title.Remove (title.Length-1, 1);
			information = information.Remove (0, i+1).Trim();
			DevelopmentStatus ds = DevelopmentStatus.NormalTask;
			Array arr = Enum.GetValues(typeof(DevelopmentStatus));
			for (int j=0; j<arr.Length; j++)
				if (arr.GetValue(j).ToString().ToLower() == status)
					ds = (DevelopmentStatus) arr.GetValue(j);
			return (new DevelopmentInformationItem (ds, title, information));
		}
		
		private DevelopmentInformationAttribute (string aTaskName)
		{
			taskName = aTaskName;
		}
		
		public DevelopmentInformationAttribute (string aTaskName, params string[] aItems)
			: this (aTaskName)
		{
			items = new DevelopmentInformationItem[aItems.Length];
			for (int i=0; i<aItems.Length; i++)
				items[i] = Parse (aItems[i]);
			for (int i=0; i<aItems.Length; i++)
				items[i].Owner = this;
		}
	}
}
