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

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies interface to describe objects described with runtime
	/// attributes
	/// </summary>
	public interface IRuntimeAttributesObject : IDisconnectable
	{
		/// <summary>
		/// Should be called whenever attribute changes
		/// </summary>
		/// <param name="aAttr">
		/// Attribute that changed <see cref="IRuntimeAttribute"/>
		/// </param>
		void OnRuntimeAttributeChange (IRuntimeAttribute aAttr);
	}

	/// <summary>
	/// Specifies interface which describes attributes that can be
	/// specified during runtime
	/// </summary>
	public interface IRuntimeAttribute : IDisconnectable
	{
		/// <value>
		/// Attribute owner
		/// </value>
		object Owner { get; }
	}
}
