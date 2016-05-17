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
	/// Provides basic assembly description
	/// </summary>
	public class AssemblyDescription
	{
		private string name = "";
		/// <value>
		/// Indexing name
		/// </value>
		public string Name {
			get { return (name); }
		}
		
		/// <value>
		/// Public description
		/// </value>
		public string Description {
			get { return (name); }
		}
		
		private Assembly assembly = null;
		/// <value>
		/// Reffered assembly
		/// </value>
		public Assembly Assembly {
			get { return (assembly); }
		}
		
		private AssemblyDevelopmentInformation developmentInformation = null;
		/// <value>
		/// Provides list of development informations
		/// </value>
		public AssemblyDevelopmentInformation DevelopmentInformation {
			get { 
				if (developmentInformation == null)
					developmentInformation = new AssemblyDevelopmentInformation (Assembly);
				return (developmentInformation);
			}
		}
		
		public AssemblyDescription (Assembly aAssembly)
			: this (aAssembly.GetName().Name, aAssembly)
		{
		}
		
		public AssemblyDescription (string aName, Assembly aAssembly)
		{
			name = aName;
			assembly = aAssembly;
		}
	}
}
