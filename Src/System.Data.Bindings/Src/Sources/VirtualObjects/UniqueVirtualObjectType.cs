// UniqueVirtualObjectType.cs - UniqueVirtualObjectType implementation for Gtk#Databindings
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
	/// Creates unique VirtualObject that doesn't registers it self in registry
	/// </summary>
	public class UniqueVirtualObjectType : VirtualObjectType
	{
		/// <summary>
		/// Creates UniqueVirtualObjectType
		/// </summary>
		public UniqueVirtualObjectType()
			: base (true)
		{
		}

		/// <summary>
		/// Creates UniqueVirtualObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Name <see cref="System.String"/>
		/// </param>
		public UniqueVirtualObjectType (string a_Name)
			: base (true, a_Name)
		{
		}
	}
}
