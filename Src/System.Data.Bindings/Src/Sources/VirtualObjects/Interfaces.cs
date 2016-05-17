// Interfaces.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Delegate method which notifies about property change
	/// </summary>
	/// <param name="aObject">
	/// Object being changed <see cref="IVirtualObject"/>
	/// </param>
	/// <param name="aProperty">
	/// Property being changed <see cref="VirtualProperty"/>
	/// </param>
	/// <remarks>
	/// Delegate is valid forVirtual objects
	/// </remarks>
	public delegate void EventOnPropertyChange (IVirtualObject aObject, VirtualProperty aProperty);

	/// <summary>
	/// Defines a simple abstract way to access VirtualObjects
	/// </summary>
	public interface IVirtualObject : IEnumerable
	{
		/// <summary>
		/// Index access to property by name
		/// </summary>
		VirtualProperty this [string aName] { get; } 
		// <summary>
		/// Index access to property by index
		// </summary>
		VirtualProperty this [int aIdx] { get; }
		/// <summary>
		/// ObjectType description of VirtualObject
		/// </summary>
		VirtualObjectType ObjectType { get; set; }
		/// <summary>
		/// Event called whenever property changes
		/// </summary>
		event EventOnPropertyChange OnChange;
		/// <summary>
		/// Called whenever property changes
		/// </summary>
		/// <param name="aObject">
		/// Object related to change <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="aProperty">
		/// Property that changed <see cref="VirtualProperty"/>
		/// </param>
		void Changed (IVirtualObject aObject, VirtualProperty aProperty);
		/// <summary>
		/// Returns proprty index for quick access 
		/// </summary>
		/// <param name="aName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// index of specified property <see cref="System.Int32"/>
		/// </returns>
		int GetPropertyIndex (string aName);
		/// <summary>
		/// Adds new property to virtual object
		/// </summary>
		/// <param name="aName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type of value <see cref="System.Type"/>
		/// </param>
		void AddProperty (string aName, System.Type aType);
		/// <summary>
		/// Removes property from VirtualObject
		/// </summary>
		/// <param name="aName">
		/// Name of property <see cref="System.String"/>
		/// </param>
		void RemoveProperty (string aName);
		/// <summary>
		/// Copies this virtual object to another
		/// </summary>
		/// <param name="aObject">
		/// Destination object <see cref="IVirtualObject"/>
		/// </param>
		void CopyTo (IVirtualObject aObject);
		/// <summary>
		/// Copies virtual object to real object
		/// </summary>
		/// <param name="aObject">
		/// Destination object <see cref="System.Object"/>
		/// </param>
		void CopyTo (object aObject);
		/// <summary>
		/// Copies to this virtual object from another
		/// </summary>
		/// <param name="aObject">
		/// Source object <see cref="IVirtualObject"/>
		/// </param>
		void CopyFrom (IVirtualObject aObject);
		/// <summary>
		/// Copies to this virtual object from real object
		/// </summary>
		/// <param name="aObject">
		/// Source object <see cref="System.Object"/>
		/// </param>
		void CopyFrom (object aObject);
		/// <summary>
		/// Strict inheriting from real object
		/// </summary>
		/// <param name="aObject">
		/// Template object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritStrict (object aObject);
		/// <summary>
		/// Strict inheriting from VirtualObject type
		/// </summary>
		/// <param name="aType">
		/// Template type <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritStrict (VirtualObjectType aType);
		/// <summary>
		/// Strict inheriting from VirtualObject
		/// </summary>
		/// <param name="aObject">
		/// Template VirtualObject <see cref="IVirtualObject"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritStrict (IVirtualObject aObject);
		/// <summary>
		/// Relaxed inheriting from real object
		/// </summary>
		/// <param name="aObject">
		/// Template object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritRelaxed (object aObject);
		/// <summary>
		/// Relaxed inheriting from virtual object type
		/// </summary>
		/// <param name="aType">
		/// Virtual object type <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritRelaxed (VirtualObjectType aType);
		/// <summary>
		/// Relaxed inheriting from virtual object
		/// </summary>
		/// <param name="aObject">
		/// Template VirtuallObject <see cref="IVirtualObject"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool InheritRelaxed (IVirtualObject aObject); 
	}
}