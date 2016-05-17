// ListAdaptor.cs - ListAdaptor implementation for Gtk#Databindings
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
using System.ComponentModel;

namespace System.Data.Bindings
{
	/// <summary>
	/// Adaptor which takes care of list
	/// </summary>
	public class ListAdaptor : Adaptor, IListEvents
	{
/*		private event ListChangedEvent onListChanged = null;
		/// <summary>
		/// OnListChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListChangedEvent OnListChanged {
			add { onListChanged += value; }
			remove { onListChanged -= value; }
		}*/

		private event ListChangedEvent listChanged = null;
		/// <summary>
		/// OnListChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListChangedEvent ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}

/*		private event ListElementAddedEvent onElementAdded = null;
		/// <summary>
		/// OnElementAdded event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListElementAddedEvent OnElementAdded {
			add { onElementAdded += value; }
			remove { onElementAdded -= value; }
		}*/

		private event ElementAddedInListObjectEvent elementAdded = null;
		/// <summary>
		/// OnElementAdded event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementAddedInListObjectEvent ElementAdded {
			add { elementAdded += value; }
			remove { elementAdded -= value; }
		}

/*		private event ListElementChangedEvent onElementChanged = null;
		/// <summary>
		/// OnElementChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListElementChangedEvent OnElementChanged {
			add { onElementChanged += value; }
			remove { onElementChanged -= value; }
		}*/
		
		private event ElementChangedInListObjectEvent elementChanged = null;
		/// <summary>
		/// OnElementChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementChangedInListObjectEvent ElementChanged {
			add { elementChanged += value; }
			remove { elementChanged -= value; }
		}
		
/*		private event ListElementRemovedEvent onElementRemoved = null;
		/// <summary>
		/// OnElementRemoved event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListElementRemovedEvent OnElementRemoved {
			add { onElementRemoved += value; }
			remove { onElementRemoved -= value; }
		}*/
		
		private event ElementRemovedFromListObjectEvent elementRemoved = null;
		/// <summary>
		/// OnElementRemoved event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementRemovedFromListObjectEvent ElementRemoved {
			add { elementRemoved += value; }
			remove { elementRemoved -= value; }
		}
		
		private event ElementsInListSortedEvent elementsSorted = null;
		/// <summary>
		/// Message handling the event of list or any of its children sorted
		/// </summary>
		public event ElementsInListSortedEvent ElementsSorted {
			add { elementsSorted += value; }
			remove { elementsSorted -= value; }
		}

		/// <summary>
		/// Additional actions to be taken on connecting DataSource
		/// </summary>
		/// <param name="aTarget">
		/// Target object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// Connect all the messaging derived adaptors need here
		/// </remarks>
		internal override void OnDataSourceConnect (object aTarget)
		{
			base.OnDataSourceConnect (aTarget);
			if (aTarget == null)
				return;
			if (aTarget is IListEvents) {
				IListEvents list = (aTarget as IListEvents);
				list.ListChanged += OnListChanged;
				list.ElementAdded += OnElementAdded;
				list.ElementChanged += OnElementChanged;
				list.ElementRemoved += OnElementRemoved;
				list.ElementsSorted += OnElementsSorted;
				list = null;
			}
			else if (aTarget is IBindingList) {
				Console.WriteLine ("DataSourceConnect to IBindingList");
				(aTarget as IBindingList).ListChanged += IBindingListChangedHandler;
			}
//			else if (aTarget is System.Data.DataTable) {
//				Console.WriteLine ("DataSourceConnect to DataTable");
//			}
//			else
//				Console.WriteLine ("DataSourceConnect (Target=" + aTarget + ")");
		}
		
		/// <summary>
		/// Additional actions to be taken on disconnecting DataSource
		/// </summary>
		/// <param name="aTarget">
		/// Target object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// Disconnect all the messaging derived adaptors need here
		/// </remarks>
		internal override void OnDataSourceDisconnect (object aTarget)
		{
			base.OnDataSourceDisconnect (aTarget);
			if (aTarget == null)
				return;
			if (aTarget is IListEvents) {
				IListEvents list = (aTarget as IListEvents);
				// No need to connect OnListChanged when taking the Adaptor DataSource
				// Should be used from different approach forms from software
				list.ListChanged -= OnListChanged;
				list.ElementAdded -= OnElementAdded;
				list.ElementChanged -= OnElementChanged;
				list.ElementRemoved -= OnElementRemoved;
				list.ElementsSorted -= OnElementsSorted;
				list = null;
			}
			else if (aTarget is IBindingList) {
//				Console.WriteLine ("DataSourceDisconnect from IBindingList");
				(aTarget as IBindingList).ListChanged -= IBindingListChangedHandler;
			}
			else if (aTarget is System.Data.DataTable) {
//				Console.WriteLine ("DataSourceDisconnect from DataTable");
				(aTarget as DataTable).RowDeleted += delegate(object sender, DataRowChangeEventArgs e) {
					
				};
			}
//			else if (aTarget is System.Data.DataTable) {
//				Console.WriteLine ("DataSourceDisconnect to DataTable");
//			}
//			else
//				Console.WriteLine ("DataSourceDisconnect (Target=" + aTarget + ")");
		}

		private void IBindingListChangedHandler (object aSender, ListChangedEventArgs e)
		{
			int[] idx = new int[1];
			switch (e.ListChangedType) {
			case ListChangedType.ItemAdded:
				idx[0] = e.NewIndex;
				OnElementAdded (aSender, idx);
				idx = null;
				break;
			case ListChangedType.ItemChanged:
				idx[0] = e.NewIndex;
				OnElementChanged (aSender, idx);
				idx = null;
				break;
			case ListChangedType.ItemDeleted:
				idx[0] = e.NewIndex;
				System.Console.Error.WriteLine("Warning: IBindingList ListChangedType.ItemDeleted idiotic behaviour");
				System.Console.Error.WriteLine("         item doesn't exists and reference to object is not passed here");
				System.Console.Error.WriteLine("         DO NOT REPORT THIS AS BUGGY BEHAVIOUR!!!");
				OnElementRemoved (aSender, idx, null);
				idx = null;
				break;
			case ListChangedType.ItemMoved:
				idx[0] = e.OldIndex;
				OnElementRemoved (aSender, idx, (aSender as IBindingList)[e.NewIndex]);
				idx = null;
				idx = new int[1];
				idx[0] = e.NewIndex;
				OnElementAdded (aSender, idx);
				idx = null;
				break;
			case ListChangedType.Reset:
				OnListChanged (aSender);
				break;
			}
		}
		
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
		public virtual void ListChildChanged (object aObject, EListAction aAction, int[] aPath)
		{
		}

		/// <summary>
		/// Handles message on the event of list change
		/// </summary>
		/// <param name="aList">
		/// List that changed <see cref="IList"/>
		/// </param>
		protected virtual void OnListChanged(object aList)
//		protected virtual void ListChanged(IList aList)
		{
			if (Target == null)
				return;
				
			if (listChanged != null)
				listChanged (aList);
			AdapteeDataChanged (FinalTarget);
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
		protected virtual void OnElementAdded (object aList, int[] aIdx)
//		protected virtual void OnElementAdded (IList aList, int[] aIdx)
		{
			if (aList == null)
				return;
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			if (elementAdded != null)
				elementAdded (aList, idx);
			//AdapteeDataChanged (FinalTarget, "");
			idx = null;
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
		protected virtual void OnElementChanged (object aList, int[] aIdx)
//		protected virtual void OnElementChanged (IList aList, int[] aIdx)
		{
			if (aList == null)
				return;
			
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			if (elementChanged != null)
				elementChanged (aList, idx);
			//AdapteeDataChanged (FinalTarget, "");
			idx = null;
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
		protected virtual void OnElementRemoved (object aList, int[] aIdx, object aObject)
//		protected virtual void OnElementRemoved (IList aList, int[] aIdx)
		{
			if (aList == null)
				return;
//System.Console.WriteLine("ListAdaptor.ElementRemoved(" + ConnectionProvider.PathToString(aIdx) + "on=" + (onElementRemoved != null));			
			int[] idx = new int [aIdx.Length];
			aIdx.CopyTo (idx, 0);
			if (elementRemoved != null)
				elementRemoved (aList, idx, aObject);
			//AdapteeDataChanged (FinalTarget, "");
			idx = null;
		}

		/// <summary>
		/// Handles message on the event of srting list or any of its elements
		/// </summary>
		/// <param name="aList">
		/// List that had been sorted <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to sorted element or null if aList is specifying it <see cref="System.Int32"/>
		/// </param>
		protected virtual void OnElementsSorted (object aList, int[] aIdx)
		{
			if (aList == null)
				return;
			int[] idx = null;
			if (aIdx != null) {
				idx = new int [aIdx.Length];
				aIdx.CopyTo (idx, 0);
			}
			if (elementsSorted != null)
				elementsSorted (aList, idx);
			idx = null;
		}

		/// <summary>
		/// Complete disconnect of all events to make it easier for GC to step in as
		/// soon as possible
		/// </summary>
		public override void Disconnect()
		{
//			onListChanged = null;
			listChanged = null;
			elementAdded = null;
			elementChanged = null;
			elementRemoved = null;
			base.Disconnect();
		}

		/// <summary>
		/// Resolves final target for Adaptor 
		/// </summary>
		/// <returns>
		/// Object of the final target this adaptor is pointing to <see cref="System.Object"/>
		/// </returns>
		protected override object DoGetFinalTarget (out bool aCallControl)
		{
			return (DatabaseProvider.ResolveTarget (base.DoGetFinalTarget (out aCallControl))); 
		}

		/// <summary>
		/// Creates ListAdaptor
		/// </summary>
		public ListAdaptor()
			: base ()
		{
		}

		/// <summary>
		/// Creates ListAdaptor
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
		public ListAdaptor(bool aIsBoundary, ControlAdaptor aControlAdaptor, object aControl, bool aSingleMappingOnly)
			: base (aIsBoundary, aControlAdaptor, aControl, aSingleMappingOnly)
		{
		}		
	}
}
