// FieldAttribute.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Attribute which assigns additional naming information to property
	/// This should enable generic remaping.
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, AllowMultiple=false)]
	public class FieldAttribute : Attribute
	{
		private string name = "";
		//// <summary>
		/// Name value for this attribute
		//// </summary>
		public string Name {
			get { return (name); }
			set { name = value; }
		}

		//// <summary>
		/// Creates new FieldAttribute with specified name
		//// </summary>
		/// <param name="aName">
		/// A <see cref="System.String"/>
		/// </param>
		public FieldAttribute (string aName)
		{
			Name = aName;
		}

		/// <summary>
		/// Creates Field attribute
		/// </summary>
		private FieldAttribute()
		{
		}
	}
}
