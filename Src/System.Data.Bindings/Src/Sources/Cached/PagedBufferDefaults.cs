// PagedBuffer.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Data.Bindings;

namespace System.Data.Bindings.Cached
{	
	/// <summary>
	/// Contains definitions how buffers should be loaded in order
	/// to either balance loading or best suit the end result of
	/// application
	/// </summary>
	public class PagedBufferDefaults
	{
		private BufferPivotLocation pivotLocation = BufferPivotLocation.Center;
		/// <value>
		/// Defines where pivot location in buffer is
		/// </value>
		public BufferPivotLocation PivotLocation {
			get { return (pivotLocation); }
			set { pivotLocation = value; }
		}

		private int size = 30;
		/// <summary>
		/// Defines size of page in buffer
		/// </summary>
		public int Size {
			get { return (size); }
			set { size = value; } 
		}

		/// <summary>
		/// Creates PagedBuffers
		/// </summary>
		public PagedBufferDefaults()
		{
		}
	}
}
