// VirtualObjects.cs - VirtualObjects implementation for Gtk#Databindings
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
using System.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// Registry that is keeping ObjectTypes in list, so they can be accessed with various methods
	/// </summary>
	public static class VirtualObjects
	{
		private static ArrayList objects = new ArrayList();
		
		/// <summary>
		/// Returns VirtualObjectType of the registered object by name
		/// </summary>
		/// <param name="a_Name">
		/// Object name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Type of found virtual object, null if not found <see cref="VirtualObjectType"/>
		/// </returns>
		public static VirtualObjectType ObjectByName (string a_Name) 
		{
			foreach (VirtualObjectType vo in objects)
				if (vo != null)
					if (vo.Name == a_Name)
						return (vo);
			return (null);
		}

		/// <summary>
		/// Adds VirtualObjectType to the registry
		/// </summary>
		/// <param name="a_Object">
		/// Object to add <see cref="VirtualObjectType"/>
		/// </param>
		/// <remarks>
		/// If object by that name already exists exception is thrown
		/// </remarks>
		public static void Add (VirtualObjectType a_Object)
		{
			if (a_Object == null)
				return;
			if (ObjectByName(a_Object.Name) != null)
				throw new ExceptionDuplicateVirtualObjectRegistered (a_Object.Name);
			objects.Add (a_Object);
		}

		/// <summary>
		/// Creates and adds new VirtualObjectType to registry
		/// </summary>
		/// <param name="a_Name">
		/// Name of new virtual object <see cref="System.String"/>
		/// </param>
		/// <remarks>
		/// When it is used this way it has to be resolved after to get the object for this type
		/// </remarks>
		public static void Add (string a_Name)
		{
			VirtualObjectType obj = new VirtualObjectType (a_Name);
			Add (obj);
			obj = null;
		}
		
		/// <summary>
		/// Removes VirtualObjectType from registry
		/// </summary>
		/// <param name="a_Name">
		/// Object type name <see cref="System.String"/>
		/// </param>
		public static void Remove (string a_Name)
		{
			VirtualObjectType obj = ObjectByName (a_Name);
			Remove (obj);
			obj = null;
		}
		
		/// <summary>
		/// Removes VirtualObjectType from registry
		/// </summary>
		/// <param name="a_Object">
		/// Object type to remove <see cref="VirtualObjectType"/>
		/// </param>
		public static void Remove (VirtualObjectType a_Object)
		{
			if (a_Object == null)
				return;
//			if (ObjectByName(a_Object.Name) != null)
//				throw new Exception ("Duplicate object registered '" + a_Object.Name + "'");
			objects.Remove (a_Object);
		}

		/// <summary>
		/// Locks the object type 
		/// </summary>
		/// <param name="a_Object">
		/// Object type <see cref="VirtualObjectType"/>
		/// </param>
		public static void Lock (VirtualObjectType a_Object)
		{
			if (a_Object == null)
				throw new ExceptionLockNullVirtualObject();
			a_Object.Lock();
		}
		
		/// <summary>
		/// Locks the object type 
		/// </summary>
		/// <param name="a_Name">
		/// Name of object type <see cref="System.String"/>
		/// </param>
		public static void Lock (string a_Name)
		{
			VirtualObjectType obj = ObjectByName(a_Name);
			if (obj == null)
				throw new ExceptionLockNullNonExistantVirtualObject (a_Name);
			Lock (obj);
			obj = null;
		}

		/// <summary>
		/// Unlocks the object type 
		/// </summary>
		/// <param name="a_Object">
		/// Object type <see cref="VirtualObjectType"/>
		/// </param>
		public static void Unlock (VirtualObjectType a_Object)
		{
			if (a_Object == null)
				throw new ExceptionUnlockNullVirtualObject();
			a_Object.Unlock();
		}
		
		/// <summary>
		/// Unlocks the object type 
		/// </summary>
		/// <param name="a_Name">
		/// Object type name <see cref="System.String"/>
		/// </param>
		public static void Unlock (string a_Name)
		{
			VirtualObjectType obj = ObjectByName(a_Name);
			if (obj == null)
				throw new ExceptionUnlockNullNonExistantVirtualObject (a_Name);
			Unlock (obj);
			obj = null;
		}
	}
}
