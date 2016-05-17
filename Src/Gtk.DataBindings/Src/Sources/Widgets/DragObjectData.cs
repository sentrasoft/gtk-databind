// DragObjectData.cs - DragObjectData implementation for Gtk#Databindings
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
using System.Data.Bindings; 

namespace Gtk.DataBindings
{
	/// <summary>
	/// Drag object data description used in Adaptable controls like TreeView
	/// </summary>
	public static class DragObjectData
	{
		private static EDragObjectType objType;
		private static IList ownerList;
		private static int[] path;
		private static object tobject = null;

		/// <summary>
		/// Specifies Adaptor to current dragged item, after being linked to this
		/// adapor one can display current dragged item. And plugging anything
		/// is as easy as setting Object and calling Null() after drag is stopped
		/// </summary>
		public static Adaptor Current = new Adaptor();

		/// <summary>
		/// Clears data
		/// </summary>
		public static void Null()
		{
			ObjType = EDragObjectType.Object; 
			OwnerList = null;
			Path = null;
			Object = null;
		}
		
		/// <value>
		/// Returns type of the dragged object
		/// </value>
		public static EDragObjectType ObjType {
			get { return (objType); }
			set { objType = value; }
		}
		
		/// <value>
		/// Owner list of the dragged object
		/// </value>
		public static IList OwnerList {
			get { return (ownerList); }
			set { ownerList = value; }
		}

		/// <value>
		/// Path description
		/// </value>
		public static int[] Path {
			get { return (path); }
			set { path = value; }
		}

		/// <value>
		/// Object being dragged
		/// </value>
		public static object Object {
			get { return (tobject); }
			set { 
				tobject = value;
				Current.Target = value;
			}
		}
	}
}
