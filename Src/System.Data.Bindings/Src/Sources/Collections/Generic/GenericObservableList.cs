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
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data.Bindings.Collections.Generic
{
	/// <summary>
	/// Observable
	/// </summary>	
	public class GenericObservableList<T> : IList<T>, IObservableList, INotifyPropertyChanged, IListEvents
	{
		private IList<T> items = null;
		
		/// <summary>
		/// PropertyChanged delegate as specified in INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Method calls PropertyChanged if it is not null, but it allows external
		/// objects to access this one for convinience
		/// </summary>
		/// <param name="aPropertyName">
		/// Name of the property which changed <see cref="System.String"/>
		/// </param>
		public virtual void OnPropertyChanged (string aPropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs(aPropertyName));
		}

		private EListReorderable isreorderable = EListReorderable.None;
		/// <summary>
		/// Returns is this list can be rearranged by controls and such
		/// </summary>
		public bool IsReorderable {
			get { return (isreorderable == EListReorderable.Reorderable); }
			set {
				if (value == true)
					isreorderable = EListReorderable.Reorderable;
				else
					isreorderable = EListReorderable.None;
			}
		}
		
		/// <summary>
		// Return items count
		/// </summary>
		public int Count {
			get { return (items.Count); }
		}
		
		/// <summary>
		// IList interface needs
		/// </summary>
		public object SyncRoot {
			get { return (items); }
		}
		
		/// <summary>
		// IList interface needs
		/// </summary>
		public bool IsFixedSize {
			get { return (false); }
		}
		
		/// <summary>
		// IList interface needs
		/// </summary>
		public bool IsReadOnly {
			get { return (items.IsReadOnly); }
		}
		
		/// <summary>
		// IList interface needs
		/// </summary>
		public bool IsSynchronized {
			get { return (false); }
		}
		
		private event ListChangedEvent listChanged = null;
		/// <summary>
		/// OnListChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListChangedEvent ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}

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

		protected void OnElementsSorted (int[] aIdx)
		{
			if (elementsSorted != null)
				elementsSorted (this, aIdx);
		}
		
		internal void ListItemsPropertyChanged (object aSender, PropertyChangedEventArgs e)
		{
			if (aSender == null)
				return;
			int i = IndexOf((T) aSender);
			if (i != -1) {
				int[] Idx = new int [1];
				Idx[0] = i;
				OnElementChanged (Idx);
			}
		}

		internal PropertyChangedEventHandler ListItemPropertyChangedMethod = null;
		
		/// <summary>
		/// Returns default NotifyPropertyChanged handler method
		/// </summary>
		/// <returns>
		/// Default handler method <see cref="PropertyChangedEventHandler"/>
		/// </returns>
		public PropertyChangedEventHandler GetDefaultNotifyPropertyChangedHandler()
		{
			return (ListItemPropertyChangedMethod);
		}
		
		/// <summary>
		/// Responsible for creating of new list class based on its own needs
		/// </summary>
		/// <returns>
		/// Creates list object container <see cref="IList"/>
		/// </returns>
		/// <remarks>
		/// Throws exception since this is template class and does not specify
		/// list object type
		/// </remarks>
		public virtual IList CreateList()
		{
			return ((IList) new List<T>());
		}

		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return (items.GetEnumerator());
		}
		
		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return (items.GetEnumerator());
		}

		protected event ListChangedEvent internalListChanged = null;
		protected void ListChangedMethod(object aList)
		{
			OnListChanged();
		}
				
		protected event ElementAddedInListObjectEvent internalElementAdded = null;
		protected void ElementAddedMethod(object aList, int[] aIdx)
		{
			int pos = IndexOf((T) aList);
			if (pos < 0)
				return;
			int[] idx = new int[aIdx.Length+1];
			idx[0] = pos;
			for (int i=0; i<aIdx.Length; i++)
				idx[i+1] = aIdx[i];
			OnElementAdded(idx);
		}
				
		protected event ElementChangedInListObjectEvent internalElementChanged = null;
		protected void ElementChangedMethod(object aList, int[] aIdx)
		{
			int pos = IndexOf((T) aList);
			if (pos < 0)
				return;
			int[] idx = new int[aIdx.Length+1];
			idx[0] = pos;
			for (int i=0; i<aIdx.Length; i++)
				idx[i+1] = aIdx[i];
			OnElementChanged(idx);
		}
				
		protected event ElementRemovedFromListObjectEvent internalElementRemoved = null;
		protected void ElementRemovedMethod(object aList, int[] aIdx, object aObject)
		{
			int pos = IndexOf((T) aList);
			if (pos < 0)
				return;
			int[] idx = new int[aIdx.Length+1];
			idx[0] = pos;
			for (int i=0; i<aIdx.Length; i++)
				idx[i+1] = aIdx[i];
			OnElementRemoved(idx, aObject);
		}
				
		protected event ElementsInListSortedEvent internalElementsSorted = null;
		protected void ElementsSortedMethod(object aList, int[] aIdx)
		{
			int pos = IndexOf((T) aList);
			if (pos < 0)
				return;
			int[] idx = new int[aIdx.Length+1];
			idx[0] = pos;
			for (int i=0; i<aIdx.Length; i++)
				idx[i+1] = aIdx[i];
			OnElementsSorted(idx);
		}
				
		protected event ListChangedEventHandler internalBindingListChanged = null;
		protected void BindingListChangedMethod(object aList, ListChangedEventArgs e)
		{
			OnListChanged();
		}
				
		protected void ConnectToObject (object aObject)
		{
			if (aObject is IListEvents) {
				(aObject as IListEvents).ListChanged += internalListChanged;
				(aObject as IListEvents).ElementAdded += internalElementAdded;
				(aObject as IListEvents).ElementChanged += internalElementChanged;
				(aObject as IListEvents).ElementRemoved += internalElementRemoved;
				(aObject as IListEvents).ElementsSorted += internalElementsSorted;
			}
			else if (aObject is IBindingList) {
				(aObject as IBindingList).ListChanged += internalBindingListChanged;
			}
			else
				MSEventSupport.ConnectEventToIObservableList (this, aObject);
		}
		
		protected void DisconnectFromObject (object aObject)
		{
			if (aObject is IListEvents) {
				(aObject as IListEvents).ListChanged -= internalListChanged;
				(aObject as IListEvents).ElementAdded -= internalElementAdded;
				(aObject as IListEvents).ElementChanged -= internalElementChanged;
				(aObject as IListEvents).ElementRemoved -= internalElementRemoved;
				(aObject as IListEvents).ElementsSorted -= internalElementsSorted;
			}
			else if (aObject is IBindingList) {
				(aObject as IBindingList).ListChanged -= internalBindingListChanged;
			}
			else
				MSEventSupport.DisconnectEventFromIObservableList (this, aObject);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to add <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index of added item <see cref="System.Int32"/>
		/// </returns>
		int IList.Add (object aObject)
		{
			if (aObject.GetType() != typeof(T))
				throw new ExceptionWrongGenericType (aObject.GetType(), typeof(T));
			items.Add ((T) aObject);
			int i = IndexOf((T) aObject);
			if (i >= 0) {
				ConnectToObject (aObject);
				OnPropertyChanged ("Items");
				int[] idx = new int[1];
				idx[0] = i;
				OnElementAdded(idx);
			}
			return (i);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to add <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index of added item <see cref="System.Int32"/>
		/// </returns>
		public void Add (T aObject)
		{
			AddWithReturn (aObject);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to add <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index of added item <see cref="System.Int32"/>
		/// </returns>
		public int AddWithReturn (T aObject)
		{
			items.Add (aObject);
			int i = IndexOf(aObject);
			if (i >= 0) {
				ConnectToObject (aObject);
				OnPropertyChanged ("Items");
				int[] idx = new int[1];
				idx[0] = i;
				OnElementAdded(idx);
			}
			return (i);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		public void Clear()
		{
			foreach (object o in this)
				DisconnectFromObject (o);
				
			if (Count < 1)
				return;
			items.Clear();
			OnListChanged();
			OnPropertyChanged("Items");
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to be searched for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if object exists in this list <see cref="System.Boolean"/>
		/// </returns>
		bool IList.Contains (object aObject)
		{
			if (aObject.GetType() != typeof(T))
				throw new ExceptionWrongGenericType (aObject.GetType(), typeof(T));
			return (items.Contains((T) aObject));
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to be searched for <see cref="T"/>
		/// </param>
		/// <returns>
		/// true if object exists in this list <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (T aObject)
		{
			return (items.Contains(aObject));
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to search for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index of found object, -1 if not found <see cref="System.Int32"/>
		/// </returns>
		int IList.IndexOf (object aObject)
		{
			if (aObject.GetType() != typeof(T))
				throw new ExceptionWrongGenericType (aObject.GetType(), typeof(T));
			return (items.IndexOf((T) aObject));
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to search for <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index of found object, -1 if not found <see cref="System.Int32"/>
		/// </returns>
		public int IndexOf (T aObject)
		{
			return (items.IndexOf(aObject));
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aArray">
		/// Array to copy items into <see cref="System.Array"/>
		/// </param>
		/// <param name="aIndex">
		/// Start index <see cref="System.Int32"/>
		/// </param>
		public void CopyTo (System.Array aArray, int aIndex)
		{
			items.CopyTo ((T[]) aArray, aIndex);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="T">
		/// Array to copy items into <see cref="System.Array"/>
		/// </param>
		/// <param name="aIndex">
		/// Start index <see cref="System.Int32"/>
		/// </param>
		public void CopyTo (T[] aArray, int aIndex)
		{
			items.CopyTo (aArray, aIndex);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to be removed <see cref="System.Object"/>
		/// </param>
		void IList.Remove (object aObject)
		{
			int i = IndexOf ((T) aObject);
			if (i > -1)
				RemoveAt (i);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to be removed <see cref="T"/>
		/// </param>
		public bool Remove (T aObject)
		{
			int c = Count;
			int i = IndexOf (aObject);
			if (i > -1)
				RemoveAt (i);
			return (Count != c);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aIndex">
		/// Index of object which needs to be removed <see cref="System.Int32"/>
		/// </param>
		public void RemoveAt (int aIndex)
		{
			if ((aIndex > -1) && (aIndex < Count)) {
				DisconnectFromObject (items[aIndex]);

				int[] idx = new int [1];
				idx[0] = aIndex;
				object o = this[idx];				
				items.RemoveAt (aIndex);
				OnElementRemoved (idx, o);
				idx = null;
				OnPropertyChanged("Items");
			}
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aIndex">
		/// Index where to insert <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Object to insert <see cref="System.Object"/>
		/// </param>
		void IList.Insert (int aIndex, object aObject)
		{
			if (aObject.GetType() != typeof(T))
				throw new ExceptionWrongGenericType (aObject.GetType(), typeof(T));
			if ((aIndex > -1) && (aIndex < Count)) {
				int c = Count;
				items.Insert (aIndex, (T) aObject);
				if (c != Count) {
					ConnectToObject (aObject);
					
					int[] idx = new int [1];
					idx[0] = aIndex;
					OnElementAdded (idx);
					OnPropertyChanged("Items");
					idx = null;
				}
			}
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aIndex">
		/// Index where to insert <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Object to insert <see cref="T"/>
		/// </param>
		public void Insert (int aIndex, T aObject)
		{
			if ((aIndex > -1) && (aIndex < Count)) {
				int c = Count;
				items.Insert (aIndex, aObject);
				if (c != Count) {
					ConnectToObject (aObject);
					
					int[] idx = new int [1];
					idx[0] = aIndex;
					OnElementAdded (idx);
					OnPropertyChanged("Items");
					idx = null;
				}
			}
		}
		
		/// <summary>
		/// Resolves object in this list only, does not support hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Index of requested object <see cref="System.Int32"/>
		/// </param>
		object IList.this [int aIdx] {
			get { return (items[aIdx]); }
			set {
				if (value.GetType() != typeof(T))
					throw new ExceptionWrongGenericType (value.GetType(), typeof(T));
				if (items[aIdx].Equals(value))
					return;
				DisconnectFromObject (items[aIdx]);
				items[aIdx] = (T) value; 
				ConnectToObject (value);
				
				int[] idx = new int [1];
				idx[0] = aIdx;
				OnElementChanged (idx);
				idx = null;
			}
		}
		
		/// <summary>
		/// Resolves object in this list only, does not support hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Index of requested object <see cref="System.Int32"/>
		/// </param>
		public T this [int aIdx] {
			get { return (items[aIdx]); }
			set {
				if (items[aIdx].Equals(value))
					return;
				DisconnectFromObject (items[aIdx]);
				items[aIdx] = value; 
				ConnectToObject (value);
				
				int[] idx = new int [1];
				idx[0] = aIdx;
				OnElementChanged (idx);
				idx = null;
			}
		}
		
		/// <summary>
		/// Resolves any object in this list, supports hierarchy
		/// </summary>
		/// <param name="aIdx">
		/// Hierarchical path <see cref="System.Int32"/>
		/// </param>
		public object this [int[] aIdx] {
			get { return (HierarchicalList.Get (this, aIdx)); }
		}
/*
		/// <summary>
		/// Checks if list is valid type to be wrapped around with ObserveableList
		/// </summary>
		/// <param name="aObject">
		/// Object to check <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if valid, false if not <see cref="System.Boolean"/>
		/// </returns>
		[ToDo ("Check if additional valid stores should be specified")]
		public static bool IsValidListStore (object aObject)
		{
			if ((ConnectionProvider.ResolveTargetForObject (aObject) is IList) ||
			    (ConnectionProvider.ResolveTargetForObject (aObject) is IListSource))
				return (true);
			//TODO: Add resolving for other valid List stores
			return (false);
		}
*/
		/// <summary>
		/// Creates ObserveableList
		/// </summary>
		public GenericObservableList()
			: base ()
		{
			internalListChanged += ListChangedMethod;
			internalElementAdded += ElementAddedMethod;
			internalElementChanged += ElementChangedMethod;
			internalElementRemoved += ElementRemovedMethod;
			internalBindingListChanged += BindingListChangedMethod;
			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			items = (IList<T>) CreateList();
			if (items == null)
				throw new ExceptionObserveableListCreatedWithNullList();
		}

		/// <summary>
		/// Creates Observeable list wit aList as its list storage
		/// </summary>
		/// <param name="aList">
		/// List to use as storage <see cref="IList"/>
		/// </param>
		public GenericObservableList (IList<T> aList)
		{
			internalListChanged += ListChangedMethod;
			internalElementAdded += ElementAddedMethod;
			internalElementChanged += ElementChangedMethod;
			internalElementRemoved += ElementRemovedMethod;
			internalElementsSorted += ElementsSortedMethod;
			internalBindingListChanged += BindingListChangedMethod;
			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			items = aList;
			if (items == null)
				throw new ExceptionObserveableListCreatedWithNullListParameter();
		}
		
		/// <summary>
		/// Destroys ObserveableList 
		/// </summary>
		~GenericObservableList()
		{
			items.Clear();
			items = null;
		}
	}
}
