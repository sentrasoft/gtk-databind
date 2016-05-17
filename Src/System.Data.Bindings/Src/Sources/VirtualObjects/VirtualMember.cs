// VirtualMember.cs - VirtualMember implementation for Gtk#Databindings
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
	/// Describes the Virtual Member of the object
	///   Name is the Member name
	///   PropertyType specifies Type of the property
	///   Index specifies fast access index
	/// </summary>
	public class VirtualMember
	{
		/// <summary>
		/// Property type 
		/// </summary>
		public System.Type PropertyType;
		/// <summary>
		/// Member name 
		/// </summary>
		public string Name;
		/// <summary>
		/// Index 
		/// </summary>
		public int Index;
		
		/// <summary>
		/// Creates VirtualMember
		/// </summary>
		/// <param name="a_Name">
		/// Name <see cref="System.String"/>
		/// </param>
		/// <param name="a_Type">
		/// Type of property <see cref="System.Type"/>
		/// </param>
		/// <param name="a_Index">
		/// Index <see cref="System.Int32"/>
		/// </param>
		public VirtualMember (string a_Name, System.Type a_Type, int a_Index)
		{
			Name = a_Name;
			PropertyType = a_Type;
			Index = a_Index;
		}
	}
}