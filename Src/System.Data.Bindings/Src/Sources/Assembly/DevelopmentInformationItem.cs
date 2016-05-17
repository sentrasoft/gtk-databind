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

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies development information item
	/// </summary>
	public class DevelopmentInformationItem
	{
		private DevelopmentInformationAttribute owner = null;
		/// <value>
		/// Provides backwards access to owner description
		/// </value>
		public DevelopmentInformationAttribute Owner {
			get { return (owner); }
			internal set { owner = value; }
		}
		
		private DevelopmentStatus status = DevelopmentStatus.NormalTask;
		/// <value>
		/// Status of development item
		/// </value>
		public DevelopmentStatus Status {
			get { return (status); }
		}
		
		private string name = "";
		/// <value>
		/// Name
		/// </value>
		public string Name {
			get { return (name); }
		}
		
		private string description = "";
		/// <value>
		/// Description
		/// </value>
		public string Description {
			get { return (description); }
		}
		
		public DevelopmentInformationItem (string aName)
		{
			name = aName;
		}
		
		public DevelopmentInformationItem (string aName, string aDescription)
			: this (aName)
		{
			description = aDescription;
		}
		
		public DevelopmentInformationItem (DevelopmentStatus aStatus, string aName)
			: this (aName)
		{
			status = aStatus;
		}
		
		public DevelopmentInformationItem (DevelopmentStatus aStatus, string aName, string aDescription)
			: this (aName, aDescription)
		{
			status = aStatus;
		}
	}
}
