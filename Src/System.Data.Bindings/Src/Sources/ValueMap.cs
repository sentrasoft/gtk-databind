// ValueMap.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Provides structure for MappingBuilder
	/// </summary>
	public class ValueMap
	{
		private bool valid = false;
		/// <summary>
		/// Returns if mapping is valid
		/// </summary>
		public bool Valid {
			get { return (valid); }
		}

		private string val = "";
		/// <summary>
		/// String representation of mapping
		/// </summary>
		public string Value {
			get { return (val); }
			set { val = value; } 
		}

		/// <summary>
		/// Creates ValueMap
		/// </summary>
		protected ValueMap()
		{
		}
	}
}
