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
using System.Collections;
using System.Collections.Generic;
using System.Data.Bindings;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides collection of attributes prepended to types
	/// </summary>	
	public class DevelopmentDescriptionCollection
	{
		private List<DevelopmentInformationAttribute> descriptions = null;
		
		/// <value>
		/// Returns count of description types
		/// </value>
		public int Count {
			get {
				if (descriptions == null)
					return (0);
				return (descriptions.Count); 
			}
		}
		
		/// <value>
		/// Returns description at index
		/// </value>
		public DevelopmentInformationAttribute this [int aIndex] {
			get {
				if (descriptions == null)
					return (null);
				return (descriptions[aIndex]);
			}
		}
		
		/// <summary>
		/// Returns index of specified item
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
				if (aItem == this[i])
					return (i);
			return (-1);
		}
		
		/// <summary>
		/// Adds new description to collection
		/// </summary>
		/// <param name="aAttr">
		/// Description <see cref="DevelopmentInformationAttribute"/>
		/// </param>
		private void Add (DevelopmentInformationAttribute aAttr)
		{
			if (aAttr == null)
				return;
			if (descriptions == null)
				descriptions = new List<DevelopmentInformationAttribute>();
			descriptions.Add (aAttr);
		}

		private DevelopmentDescriptionCollection()
		{
		}
		
		public DevelopmentDescriptionCollection (Assembly aAssembly)
		{
			object[] attrs = aAssembly.GetCustomAttributes (false);
			foreach (object attr in attrs)
				if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true)
					Add (attr as DevelopmentInformationAttribute);
		}
				
		public DevelopmentDescriptionCollection (System.Type aType)
		{
			object[] attrs = aType.GetCustomAttributes (false);
			foreach (object attr in attrs) {
				if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true)
					Add (attr as DevelopmentInformationAttribute);
			}
		}
				
		public DevelopmentDescriptionCollection (MemberInfo aMember)
		{
			object[] attrs = aMember.GetCustomAttributes (false);
			foreach (object attr in attrs)
				if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true)
					Add (attr as DevelopmentInformationAttribute);
		}
	}
}
