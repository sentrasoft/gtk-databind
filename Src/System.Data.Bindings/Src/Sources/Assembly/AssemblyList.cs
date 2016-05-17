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
using System.Data.Bindings.Collections;
using System.Data.Bindings.Collections.Generic;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides assembly list and its adaptor
	/// </summary>
	public class AssemblyList : BaseNotifyPropertyChanged
	{
		private GenericObservableList<AssemblyDescription> assemblies = null;
		
		private Adaptor adaptor = null;
		/// <value>
		/// Provides adaptor access to assembly list
		/// </value>
		public Adaptor Assemblies {
			get { 
				if (adaptor == null) {
					adaptor = new Adaptor();
					adaptor.Target = assemblies;
				}
				return (adaptor); 
			}
		}
		
		/// <value>
		/// Returns assembly count
		/// </value>
		public int Count {
			get {
				if (assemblies == null)
					return (0);
				return (assemblies.Count);
			}
		}
		
		/// <summary>
		/// Adds assembly into list
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly <see cref="Assembly"/>
		/// </param>
		public void Add (Assembly aAssembly)
		{
			if (aAssembly == null)
				return;
			if (assemblies == null) {
				assemblies = new GenericObservableList<AssemblyDescription>();
				if (adaptor != null)
					adaptor.Target = assemblies;
			}
			foreach (AssemblyDescription ad in assemblies)
				if (ad.Assembly == aAssembly)
					return;
			
			assemblies.Add (new AssemblyDescription (aAssembly));
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null)
					Add (Assembly.Load (mod));
			OnPropertyChanged ("Count");
		}

		/// <summary>
		/// Parses assembly and its references
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly <see cref="Assembly"/>
		/// </param>
		public void ParseAssembly (Assembly aAssembly)
		{
			Add (aAssembly);
		}
		
		public AssemblyList()
		{
		}
		
		public AssemblyList (Assembly aAssembly)
		{
			ParseAssembly (aAssembly);
		}
		
		public AssemblyList (string aAssembly)
		{
			ParseAssembly (Assembly.LoadFile (aAssembly));
		}
	}
}
