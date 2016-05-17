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
	public interface IDevelopmentInformation
	{
		string Name { get; }
		string Description { get; }
//		object Icon { get; }
		DevelopmentDescriptionCollection DevelopmentDescriptions { get; }
	}
	
	/// <summary>
	/// Specifies need for DevelopmentInformationItem
	/// </summary>
	public enum DevelopmentStatus
	{
		/// <summary>
		/// Not existing
		/// </summary>
		[ItemIcon ("status-none")]
		[ItemTitle ("None")]
		None,
		/// <summary>
		/// Critical
		/// </summary>
		[ItemIcon ("status-critical")]
		[ItemTitle ("Critical bug")]
		CriticalBug,
		/// <summary>
		/// Not critical
		/// </summary>
		[ItemIcon ("status-noncritical")]
		[ItemTitle ("Not critical bug")]
		NotCriticalBug,
		/// <summary>
		/// Normal task
		/// </summary>
		[ItemIcon ("status-normal")]
		[ItemTitle ("Normal task")]
		NormalTask,
		/// <summary>
		/// Not needed, but would be nice to have
		/// </summary>
		[ItemIcon ("status-wouldbenice")]
		[ItemTitle ("Would be nice")]
		WouldBeNice
	}

	/// <summary>
	/// Event passed when new assembly is loaded in runtime and
	/// </summary>
	public delegate void NewAssemblyLoadedEvent (object aSender, NewAssemblyLoadedEventArgs aArgs);
	
	/// <summary>
	/// Event args passed when new assembly is loaded
	/// </summary>
	public class NewAssemblyLoadedEventArgs : EventArgs
	{
		private Assembly newAssembly = null;
		/// <value>
		/// Assembly that was just loaded
		/// </value>
		public Assembly NewAssembly {
			get { return (newAssembly); }
		}
		
		public NewAssemblyLoadedEventArgs (Assembly aAssembly)
		{
			newAssembly = aAssembly;
		}
	}
}
