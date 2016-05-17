// BufferClippingList.cs - Field Attribute to assign additional information for Gtk#Databindings
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

namespace System.Data.Bindings.Cached
{
	/// <summary>
	/// Specifies list containing informations about buffer which is clipped 
	/// by specified indexes
	/// </summary>
	public class BufferClippingList
	{
		private ArrayList buffers = null;

		/// <summary>
		/// Returns number of clipping informations inside this list
		/// </summary>
		public int Count {
			get {
				if (buffers == null)
					return (0);
				return (buffers.Count); 
			}
		}

		private event RangeAddEvent onRangeAdded = null;
		/// <summary>
		/// Event called whenever region is added
		/// </summary>
		public event RangeAddEvent OnRangeAdded {
			add { onRangeAdded += value; }
			remove { onRangeAdded -= value; }
		}
		
		private event RangeClearEvent onRangeCleared = null;
		/// <summary>
		/// Event called whenever region is cleared
		/// </summary>
		public event RangeClearEvent OnRangeCleared {
			add { onRangeCleared += value; }
			remove { onRangeCleared -= value; }
		}
		
		/// <summary>
		/// Sends invalidate information to owner buffer
		/// </summary>
		/// <param name="aFrom">
		/// Minimum parameter index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Maximum parameter index <see cref="System.Int32"/>
		/// </param>
		public void Invalidate (int aFrom, int aTo)
		{
		}
		
		/// <summary>
		/// Adds specified region into list
		/// </summary>
		/// <param name="aFrom">
		/// Minimum parameter index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Maximum parameter index <see cref="System.Int32"/>
		/// </param>
		public void Add (int aFrom, int aTo)
		{
		}
		
		/// <summary>
		/// Removes specified region from the list
		/// </summary>
		/// <param name="aFrom">
		/// Minimum parameter index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Maximum parameter index <see cref="System.Int32"/>
		/// </param>
		public void Remove (int aFrom, int aTo)
		{
		}
		
		/// <summary>
		/// Calls event to notify owner to add range specified elements
		/// </summary>
		/// <param name="aFrom">
		/// Specifies From value <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Specifies To value <see cref="System.Int32"/>
		/// </param>
		public void RangeAdded (int aFrom, int aTo)
		{
			if (onRangeAdded != null)
				onRangeAdded (aFrom, aTo);
		}
		
		/// <summary>
		/// Calls event to notify owner to clear elements in specified range
		/// </summary>
		/// <param name="aFrom">
		/// Specifies From value <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Specifies To value <see cref="System.Int32"/>
		/// </param>
		public void RangeCleared (int aFrom, int aTo)
		{
			if (onRangeCleared != null)
				onRangeCleared (aFrom, aTo);
		}

		/// <summary>
		/// Creates list to store clipping informations about buffer
		/// </summary>
		public BufferClippingList()
		{
		}
	}
}
