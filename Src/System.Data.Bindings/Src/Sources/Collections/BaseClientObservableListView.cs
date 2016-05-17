//  
//  Copyright (C) 2009 matooo
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections;

namespace System.Data.Bindings.Collections
{	
	/// <summary>
	/// Provides base for any list that filters events
	/// </summary>
	public class BaseClientObservableListView : BaseNotifyPropertyChanged, IObservableListClient
	{
		private IList parentView = null;
		/// <value>
		/// Parent list
		/// </value>
		public IList ParentView {
			get { return (parentView); }
		}

		#region ListEvents
		
		private event ListChangedEvent listChanged = null;
		/// <summary>
		/// OnListChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListChangedEvent ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}

		/// <summary>
		/// Emits event that list as changed
		/// </summary>
		protected void OnListChanged()
		{
			if (listChanged != null)
				listChanged(this);
		}
		
		private event ElementAddedInListObjectEvent elementAdded = null;
		/// <summary>
		/// OnElementAdded event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementAddedInListObjectEvent ElementAdded {
			add { elementAdded += value; }
			remove { elementAdded -= value; }
		}

		/// <summary>
		/// Emits event that element has been added in this lists hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected void OnElementAdded (int[] aIdx)
		{
			if (elementAdded != null)
				elementAdded (this, aIdx);
		}
		
		private event ElementChangedInListObjectEvent elementChanged = null;
		/// <summary>
		/// OnElementChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementChangedInListObjectEvent ElementChanged {
			add { elementChanged += value; }
			remove { elementChanged -= value; }
		}

		/// <summary>
		/// Emits event that element has been changed in this lists hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected void OnElementChanged (int[] aIdx)
		{
			if (elementChanged != null)
				elementChanged (this, aIdx);
		}
		
		private event ElementRemovedFromListObjectEvent elementRemoved = null;
		/// <summary>
		/// OnElementRemoved event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementRemovedFromListObjectEvent ElementRemoved {
			add { elementRemoved += value; }
			remove { elementRemoved -= value; }
		}

		/// <summary>
		/// Emits event that element has been removed in this lists hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Reference to removed object <see cref="System.Object"/>
		/// </param>
		protected void OnElementRemoved (int[] aIdx, object aObject)
		{
			if (elementRemoved != null)
				elementRemoved (this, aIdx, aObject);
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
		/// Wrapper to emit ElementsSorted event
		/// </summary>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected void OnElementsSorted (int[] aIdx)
		{
			if (elementsSorted != null)
				elementsSorted (this, aIdx);
		}

		#endregion ListEvents

		#region IObservableListClient
		
		/// <summary>
		/// Resolves master list for this view
		/// </summary>
		/// <returns>
		/// Master list <see cref="IList"/>
		/// </returns>
		public IList GetMasterList ()
		{
			IList lst = ParentView;
			while (TypeValidator.IsCompatible(lst.GetType(), typeof(IObservableListClient)) == true)
				lst = (lst as IObservableListClient).ParentView;
			return (lst);
		}
		
		/// <summary>
		/// Calls clear on parent view
		/// </summary>
		public virtual void ClearParent()
		{
			ParentView.Clear();
		}
		
		/// <summary>
		/// Calls clear on master view
		/// </summary>
		public virtual void ClearMaster()
		{
			GetMasterList().Clear();
		}

		#endregion IObservableListClient

		#region ListEvent_Handlers
		
		private ElementAddedInListObjectEvent myAddedMethod = null;
		private ElementChangedInListObjectEvent myChangedMethod = null;
		private ElementRemovedFromListObjectEvent myRemovedMethod = null;
		private ListChangedEvent myListChangedMethod = null;
		private ElementsInListSortedEvent mySortedMethod = null;

		/// <summary>
		/// Provides message when element was changed in master or its hierarchy
		/// </summary>
		/// <param name="aList">
		/// List that contains new element in its hierarchy <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected virtual void ElementAddedIntoMaster (object aList, int[] aIdx) 
		{
		}

		/// <summary>
		/// Provides message when element was changed in master or its hierarchy
		/// </summary>
		/// <param name="aList">
		/// List that contains changed element in its hierarchy <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected virtual void ElementChangedInMaster (object aList, int[] aIdx) 
		{
		}

		/// <summary>
		/// Provides message when element was removed from master or its hierarchy
		/// </summary>
		/// <param name="aList">
		/// List that removed element <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Object that was removed <see cref="System.Object"/>
		/// </param>
		protected virtual void ElementRemovedFromMaster (object aList, int[] aIdx, object aObject) 
		{
		}
		
		/// <summary>
		/// Provides message that elements in master have been sorted
		/// </summary>
		/// <param name="aObject">
		/// List that was sorted <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Hierarchical index of change <see cref="System.Int32"/>
		/// </param>
		protected virtual void ElementsSortedInMaster (object aObject, int[] aIdx) 
		{
		}

		/// <summary>
		/// Provides message that master list has changed
		/// </summary>
		/// <param name="aList">
		/// Master list <see cref="System.Object"/>
		/// </param>
		protected virtual void MasterListChanged (object aList) 
		{
		}
		
		#endregion ListEvent_Handlers
		
		#region IDisconnectable

		/// <summary>
		/// Disconnects everything
		/// </summary>
		/// <remarks>
		/// Do not call this directly
		/// </remarks>
		public virtual void Disconnect ()
		{
			if (TypeValidator.IsCompatible(parentView.GetType(), typeof(IListEvents)) == true) {
				(parentView as IListEvents).ElementAdded -= myAddedMethod;
				(parentView as IListEvents).ElementChanged -= myChangedMethod;
				(parentView as IListEvents).ElementRemoved -= myRemovedMethod;
				(parentView as IListEvents).ElementsSorted -= mySortedMethod;
				(parentView as IListEvents).ListChanged -= myListChangedMethod;
				myAddedMethod = null;
				myChangedMethod = null;
				myListChangedMethod = null;
				myRemovedMethod = null;
				mySortedMethod = null;
			}
			parentView = null;
		}

		#endregion IDisconnectable
		
		private BaseClientObservableListView()
			: this (null)
		{
		}
		
		public BaseClientObservableListView (IList aParentView)
		{
			if (aParentView == null)
				throw new NullReferenceException ("Parent view can't be null");
			parentView = aParentView;
			if (TypeValidator.IsCompatible(parentView.GetType(), typeof(IListEvents)) == true) {
				myAddedMethod = new ElementAddedInListObjectEvent (ElementAddedIntoMaster);
				myChangedMethod = new ElementChangedInListObjectEvent (ElementChangedInMaster);
				myListChangedMethod = new ListChangedEvent (MasterListChanged);
				myRemovedMethod = new ElementRemovedFromListObjectEvent (ElementRemovedFromMaster);
				mySortedMethod = new ElementsInListSortedEvent (ElementsSortedInMaster);
				(parentView as IListEvents).ElementAdded += myAddedMethod;
				(parentView as IListEvents).ElementChanged += myChangedMethod;
				(parentView as IListEvents).ElementRemoved += myRemovedMethod;
				(parentView as IListEvents).ElementsSorted += mySortedMethod;
				(parentView as IListEvents).ListChanged += myListChangedMethod;
			}
		}
		
		~BaseClientObservableListView()
		{
			Disconnect();
		}
	}
}
