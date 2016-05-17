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
using System.Data.Bindings.Collections;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides notification engine for application where registrators
	/// can be notified about new asssemblies that were loaded
	/// </summary>
	/// <remarks>
	/// As soon as new assembly is loaded in runtime
	/// AssemblyEngine.NewAssemblyLoaded should be called
	/// </remarks>
	public static class AssemblyEngine
	{
		/// <value>
		/// Convenience wrapper around Assembly.GetEntryAssembly
		/// </value>
		public static Assembly EntryAssembly {
			get { return (Assembly.GetEntryAssembly()); }
		}
		
		/// <value>
		/// Convenience wrapper around Assembly.GetCallingAssembly
		/// </value>
		public static Assembly CallingAssembly {
			get { return (Assembly.GetCallingAssembly()); }
		}
		
		/// <value>
		/// Convenience wrapper around Assembly.GetExecutingAssembly
		/// </value>
		public static Assembly ExecutingAssembly {
			get { return (Assembly.GetExecutingAssembly()); }
		}
		
		private static event NewAssemblyLoadedEvent assemblyLoaded = null;
		/// <summary>
		/// All registered methods will be notified when new assembly
		/// is loaded and OnAssemblyLoaded is called
		/// </summary>
		public static event NewAssemblyLoadedEvent AssemblyLoaded {
			add { assemblyLoaded += value; }
			remove { assemblyLoaded -= value; }
		}
	
		private static AssemblyList referencedAssemblies = null;
		/// <value>
		/// Returns assembly list to entry assembly
		/// </value>
		public static AssemblyList ReferencedAssemblies {
			get { 
				if (referencedAssemblies == null) {
					referencedAssemblies = new AssemblyList();
					referencedAssemblies.ParseAssembly (EntryAssembly);
				}
				return (referencedAssemblies);
			}
		}
		
		/// <summary>
		/// Applications should call this when loading new assembly
		/// </summary>
		/// <param name="aNewAssembly">
		/// A <see cref="Assembly"/>
		/// </param>
		public static void NewAssemblyLoaded (Assembly aNewAssembly)
		{
			if (aNewAssembly == null)
				return;
			if (referencedAssemblies != null)
				referencedAssemblies.Add (aNewAssembly);
			if (assemblyLoaded != null)
				assemblyLoaded (Assembly.GetEntryAssembly(), new NewAssemblyLoadedEventArgs (aNewAssembly));
		}
	}
}
