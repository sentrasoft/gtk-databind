// GtkListAdaptor.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Threading;
using System.Data.Bindings;

namespace System.Data.Bindings
{
	/// <summary>
	/// Adaptor which takes care of list
	/// </summary>
	public class GtkListAdaptor : ListAdaptor
	{
		/// <summary>
		/// Bogus and not needed, it is only needed to fullfill the IObserveableList
		/// </summary>
		/// <param name="aObject">
		/// Object changed <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Action <see cref="EListAction"/>
		/// </param>
		/// <param name="aPath">
		/// Path to changed object <see cref="System.Int32"/>
		/// </param>
		public override void ListChildChanged (object aObject, EListAction aAction, int[] aPath)
		{
			//if (aAction == EListAction.Add)

			Gtk.Application.Invoke (delegate {
				base.ListChildChanged (aObject, aAction, aPath);
			});
		}
		
		/// <summary>
		/// Handles message on the event of list change
		/// </summary>
		/// <param name="aList">
		/// List that changed <see cref="IList"/>
		/// </param>
		protected override void OnListChanged(object aList)
		{
			if ((aList == null) || (aList != FinalTarget))
				return;
			Gtk.Application.Invoke (delegate {
				base.OnListChanged (aList);
			});
		}
		
		/// <summary>
		/// Handles message on the event of adding element
		/// </summary>
		/// <param name="aList">
		/// List that had added element <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to added element <see cref="System.Int32"/>
		/// </param>
		protected override void OnElementAdded (object aList, int[] aIdx)
		{
			if ((aList == null) || (aList != FinalTarget))
				return;
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			Gtk.Application.Invoke (delegate {
				base.OnElementAdded (aList, idx);
				idx = null;
			});
		}

		/// <summary>
		/// Handles message on the event of changing element
		/// </summary>
		/// <param name="aList">
		/// List that had changed element <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to changed element <see cref="System.Int32"/>
		/// </param>
		protected override void OnElementChanged (object aList, int[] aIdx)
		{
			if ((aList == null) || (aList != FinalTarget))
				return;
			
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			Gtk.Application.Invoke (delegate {
				base.OnElementChanged (aList, aIdx);
				idx = null;
			});
		}

		/// <summary>
		/// Handles message on the event of removing element
		/// </summary>
		/// <param name="aList">
		/// List that had removed element <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to removed element <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Deleted element <see cref="System.Object"/>
		/// </param>
		protected override void OnElementRemoved (object aList, int[] aIdx, object aObject)
		{
			if ((aList == null) || (aList != FinalTarget))
				return;
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			Gtk.Application.Invoke (delegate {
				base.OnElementRemoved (aList, aIdx, aObject);
				idx = null;
			});
		}

		/// <summary>
		/// Creates ListAdaptor
		/// </summary>
		public GtkListAdaptor()
			: base ()
		{
		}

		/// <summary>
		/// Creates GtkListAdaptor
		/// </summary>
		/// <param name="aIsBoundary">
		/// Defines if this is boundary adapter <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aControlAdaptor">
		/// ControlAdapter to which this one is connected <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control connected to this adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMappingOnly">
		/// Adaptor supports single mapping only <see cref="System.Boolean"/>
		/// </param>
		public GtkListAdaptor(bool aIsBoundary, ControlAdaptor aControlAdaptor, object aControl, bool aSingleMappingOnly)
			: base (aIsBoundary, aControlAdaptor, aControl, aSingleMappingOnly)
		{
		}		
	}
}
