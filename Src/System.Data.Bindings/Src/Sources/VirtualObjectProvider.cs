// VirtualObjectProvider.cs - Field Attribute to assign additional information for Gtk#Databindings
//
// Author: m. <ml@arsis.net>
//
// Copyright (c) 2006 m.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.



using System;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides needed routines for resolving and hadling data with
	/// virtual objects
	/// </summary>
	[ToDo ("Add copy routines")]
	public static class VirtualObjectProvider
	{
		/// <summary>
		/// Checks if object is virtual object related or not
		/// </summary>
		/// <param name="aObject">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsValidType (object aObject)
		{
			if (aObject == null)
				return (false);
			if (TypeValidator.IsCompatible(aObject.GetType(), typeof(IVirtualObject)) == true)
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Checks if specified property exists in virtual object 
		/// </summary>
		/// <param name="aObject">
		/// Object to check in <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// VirtualProperty if property exists, null if it doesn't <see cref="System.Object"/>
		/// </returns>
		public static object PropertyExists (object aObject, string aPropertyName)
		{
			if (IsValidType(aObject) == false)
				return (null);
			int i = (aObject as IVirtualObject).GetPropertyIndex(aPropertyName);
			if (i > -1)
				return ((aObject as IVirtualObject)[i]);
			return (null);
		}
		
		/// <summary>
		/// Resolves adaptor target for specific object
		/// </summary>
		/// <param name="aObject">
		/// Object to check target for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Returns final destination or null <see cref="System.Object"/>
		/// </returns>
		public static object ResolveTarget (object aObject)
		{
			if (IsValidType(aObject) == false)
				return (aObject);
			return (aObject);
		}
	}
}
