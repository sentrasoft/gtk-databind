// ObserveableList.cs - ObserveableList implementation for Gtk#Databindings
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
using System.Xml;
using System.Data.Bindings;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// List provider class, functioning as observeable object
	///
	/// This type of list also automaticaly adds her self as one of objects parents. This
	/// is the reason why list can go down with hierarchy
	/// </summary>
	/// <remarks>
	///TODO: Check the DataSet functionality, DataSet is not IList and try to fit it in this logic to simplify
	/// Every new type has to provide IList abstraction
	///
	/// This object needs its own counterpart, aka. Mirror reflection for ordinary objects that were not provided
	/// as ObserveableList, basically same story as Observeable and DataSourceInfo. One serves as direct handler
	/// and one is a wrapper providing functionality while editing is in progress
	///
	///UPDATE: As DataSet is not IList it is probably much more sane to provide DataSet proxy wrapper, which will
	/// handle all the traffic between reading the DataSet and ObserveableList. Which means that one could use this 
	/// as simple view for part of Data. This kind of approach should provide network speedups as well as faster
	/// application loading. Instead of reading complete DataSet one could simply read (for example SQL) with OFFSET 
	/// and LIMIT. Move the offset and update the local cache while ObserveableList should only provide visible data.
	/// This is planed for Gtk.DataBindings.Data (classes ProxyList and ProxyListView).
	/// Main reason for that implementation showed on low bandwidth access (all the testing is based on bluetooth
	/// connection speed which would ammount a bit faster than ordinary 56K modem), where working was almost impossible by
	/// using standard approach
	/// Gtk.DataBindings.Data will be part of gtk-databinddata-lib project which will show up (testing versions already
	/// exist from pre-crash age) as soon as this one takes off in full
	/// </remarks>
	[Serializable()]
//	[Obsolete ("Class ObserveableList will be replaced with ObservableList")]
	public class ObserveableList : ObservableList //Observeable, IObserveableList
	{
/*		/// <summary>
		/// Defines action 
		/// </summary>
		public struct ListAction {
			/// <summary>
			/// List action
			/// </summary>
			public EListAction Action;
			/// <summary>
			/// Path index
			/// </summary>
			public int[] Index;
			
			/// <summary>
			/// Creates ListAction
			/// </summary>
			/// <param name="aAction">
			/// Action type <see cref="EListAction"/>
			/// </param>
			/// <param name="aIndex">
			/// Path for action <see cref="System.Int32"/>
			/// </param>
			public ListAction (EListAction aAction, int[] aIndex)
			{
				Action = aAction;
				Index = aIndex;
			}
		}

		private event ElementsInListSortedEvent elementsSorted = null;
		/// <summary>
		/// Message handling the event of list or any of its children sorted
		/// </summary>
		public event ElementsInListSortedEvent ElementsSorted {
			add { elementsSorted += value; }
			remove { elementsSorted -= value; }
		}

		private bool quietMode = false;
		public bool QuietMode {
			get { return ((quietMode == true) || (IsFrozen == true)); }
			set { quietMode = value; } 
		}
		
		private IList items = null;
		private int lastSearchedIndex = -1;
		private object lastSearched = null;
		private bool noChange = false;
		private ArrayList queue = new ArrayList();		
		private bool calledForReaction = false;
		
		private bool elementCheckEnabled = false;
		/// <summary>
		/// Controls if values assigned with this list should be checked for type
		/// </summary>
		public bool ElementCheckEnabled {
			get { return (elementCheckEnabled); }
			set { elementCheckEnabled = value; }
		}
		
		[NonSerialized()]
		private Type listElementType;
		/// <summary>
		/// Element Type restriction
		/// </summary>
		public Type ListElementType {
			get { return (listElementType); }
			set { 
				listElementType = value;
				ElementCheckEnabled = true;
			}
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
		/// Returns is this list can be rearranged by controls and such
		/// </summary>
		public bool GUIDragDrop {
			get { return ((isreorderable == EListReorderable.DragDrop) || (IsReorderable == true)); }
			set {
				if (value == true)
					isreorderable = EListReorderable.DragDrop;
				else
					isreorderable = EListReorderable.None;
			}
		}
		
		/// <summary>
		/// Sets internaly the ability for items to be rearranged from gui
		/// </summary>
		/// <remarks>
		/// Degrades this option to DragDrop, for fully disabling the drag drop
		/// set CanDoGUIDragDrop to false
		/// </remarks>
		protected bool CanBeReordered {
			set {
				if (value == false) {
					if (isreorderable == EListReorderable.Reorderable)
						isreorderable = EListReorderable.DragDrop;
				}
				else
					isreorderable = EListReorderable.Reorderable; 
			}
		}
		
		/// <summary>
		/// Sets internaly the ability for items to be dragged by gui
		/// </summary>
		/// <remarks>
		/// Degrades this option to None, for the disabling 
		/// set GUIDragDrop to false
		/// </remarks>
		protected bool CanDoGUIDragDrop {
			set {
				if (value == false) {
					isreorderable = EListReorderable.None;
				}
				else
					if (IsReorderable == false)
						isreorderable = EListReorderable.DragDrop; 
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
			get { return (items.SyncRoot); }
		}
		
		/// <summary>
		// IList interface needs
		/// </summary>
		public bool IsFixedSize {
			get { return (items.IsFixedSize); }
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
			get { return (items.IsSynchronized); }
		}
		
		private event ListChangedEvent listChanged = null;
		/// <summary>
		/// OnListChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ListChangedEvent ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}

		private event ListBeforeElementAdd onBeforeElementAdd = null;
		/// <summary>
		/// OnBeforeElementAdd event is called to enable the possibility of object modification
		/// before addition to list
		/// </summary>
		public event ListBeforeElementAdd OnBeforeElementAdd {
			add { onBeforeElementAdd += value; }
			remove { onBeforeElementAdd -= value; }
		}

		private event ListAfterElementAdd onAfterElementAdd = null;
		/// <summary>
		/// OnAfterElementAdd event is called to enable the possibility of object modification
		/// after addition to list
		/// </summary>
		public event ListAfterElementAdd OnAfterElementAdd {
			add { onAfterElementAdd += value; }
			remove { onAfterElementAdd -= value; }
		}

		private event ListBeforeElementRemove onBeforeElementRemove = null;
		/// <summary>
		/// OnBeforeElementRemove event is called to enable the possibility of object modification
		/// before removing it from the list
		/// </summary>
		public event ListBeforeElementRemove OnBeforeElementRemove {
			add { onBeforeElementRemove += value; }
			remove { onBeforeElementRemove -= value; }
		}

		private event ListAfterElementRemove onAfterElementRemove = null;
		/// <summary>
		/// OnAfterElementRemove event is called to enable the possibility of object modification
		/// after removing it from the list
		/// </summary>
		public event ListAfterElementRemove OnAfterElementRemove {
			add { onAfterElementRemove += value; }
			remove { onAfterElementRemove -= value; }
		}

		private event ElementAddedInListObjectEvent elementAdded = null;
		/// <summary>
		/// OnElementAdded event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementAddedInListObjectEvent ElementAdded {
			add { elementAdded += value; }
			remove { elementAdded -= value; }
		}

		private event ElementChangedInListObjectEvent elementChanged = null;
		/// <summary>
		/// OnElementChanged event is crossconnecting this List and objects connected to this event
		/// </summary>
		public event ElementChangedInListObjectEvent ElementChanged {
			add { elementChanged += value; }
			remove { elementChanged -= value; }
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
		/// Static method to handle SDB notification
		/// </summary>
		/// <param name="aSender">
		/// Object that changed <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Will be ignored as SDB doesn't use names for flexibility and speed <see cref="PropertyChangedEventArgs"/>
		/// </param>
		internal void ListItemsPropertyChanged (object aSender, PropertyChangedEventArgs e)
		{
			if (aSender == null)
				return;
			int i = this.IndexOf(aSender);
			if (i != -1) {
				int[] Idx = new int [1];
				Idx[0] = i;
				OnElementChanged (Idx);
			}
//				if (aSender != null)
//				ListChildChanged (aSender, EListAction.Change, null);
//				ChildChanged (aSender, null);
		}

		internal PropertyChangedEventHandler ListItemPropertyChangedMethod = null;

		/// <summary>
		/// Checks if this object is valid to be part of this list
		/// </summary>
		/// <param name="aObject">
		/// Object to check <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if object is valid <see cref="System.Boolean"/>
		/// </returns>
		public virtual bool IsValidObject (object aObject)
		{
			if (aObject == null)
				return (false);
			
			if (ElementCheckEnabled == true)
				return (TypeValidator.IsCompatible(aObject.GetType(), ListElementType));

			return (true);
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
			throw new ExceptionObserveableSpawnedWithUntypedArray();
		}

		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (items.GetEnumerator());
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
		/// Receiving a notification about child being changed
		/// </summary>
		/// <param name="aObject">
		/// Observeable object that changed <see cref="IObserveable"/>
		/// </param>
		/// <param name="aPath">
		/// Path to observeable object <see cref="System.Int32"/>
		/// </param>
		public override void ChildChanged (IObserveable aObject, int[] aPath)
		{
			if (QuietMode == true)
				return;
			if (aObject == this)
				return;
			int i;
			if ((aObject == lastSearched) && (noChange == true) && (lastSearchedIndex != -1))
				i = lastSearchedIndex;
			else {
				noChange = true;
				lastSearched = aObject;
				i = IndexOf(aObject);
			}
			int[] path = null;
			if (i > -1)
				path = aPath.AddPathIndexOnStart(i);
			if (parents != null)
				foreach (WeakReference wr in parents)
					if ((wr.Target != null) && (wr.Target is IObserveable))
						((IObserveable) wr.Target).ChildChanged (this, path);
			
//			ElementChanged (path);
			path = null;
			// Now do the actual change about this one
			//DataSourceController.GetRequest (this);
			//TODO: Some optimization queue where selective update is possible
		}
		
		/// <summary>
		/// Receiving a notification about child being changed
		/// </summary>
		/// <param name="aObject">
		/// Object that changed <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Action performed <see cref="EListAction"/>
		/// </param>
		/// <param name="aPath">
		/// Path to changed object <see cref="System.Int32"/>
		/// </param>
		public virtual void ListChildChanged (object aObject, EListAction aAction, int[] aPath)
		{
			if (QuietMode == true)
				return;
			int[] path = null;
			if (aObject == this)
				return;
			int i;
			if (aObject == null)
				path = aPath;
			else {
				if ((aObject == lastSearched) && (noChange == true) && (lastSearchedIndex != -1))
					i = lastSearchedIndex;
				else {
					noChange = true;
					lastSearched = aObject;
					lastSearchedIndex = IndexOf(aObject);
					i = lastSearchedIndex;
				}
				if (i > -1)
					path = aPath.AddPathIndexOnStart(i);
			}
			if (IsFrozen == false)
				if (parents != null)
					foreach (WeakReference wr in parents)
						if ((wr.Target != null) && (wr.Target is IObserveableList))
							((IObserveableList) wr.Target).ListChildChanged (this, aAction, path);

			if ((calledForReaction == false) && (aObject != null))
				switch (aAction) {
					case EListAction.Add:
						OnElementAdded (path);
						break;
					case EListAction.Remove:
						OnElementRemoved (path, aObject);
						break;
					case EListAction.Change:
						OnElementChanged (path);
						break;
				}
			path = null;
			// Now do the actual change about this one
			//DataSourceController.GetRequest (this);
			//TODO: Some optimization queue where selective update is possible
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
		public int Add (object aObject)
		{
			if (IsValidObject (aObject) == false)
				return (-1);

			int c = Count;
			// Wrap list if this is IList type
			if ((aObject is IList) && !(aObject is IObserveableList))
				aObject = new ObserveableList (aObject as IList);
			// Start addition
			if (QuietMode == false)
				if (onBeforeElementAdd != null)
					this.onBeforeElementAdd (this, aObject);
			int i = items.Add (aObject);
			if (QuietMode == false)
				if (onAfterElementAdd != null)
					this.onAfterElementAdd (this, aObject);
			if (c != Count) {
				if (aObject is Observeable)
					(aObject as Observeable).AddParent (this);
				else
					MSEventSupport.ConnectEventToObserveableList (this, aObject);
						
				int[] idx = new int [1];
				idx[0] = Count-1;
				lastSearched = items[Count-1];
				lastSearchedIndex = Count-1;
				OnElementAdded (idx);
				idx = null;
			}
			return (i);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		public void Clear()
		{
			foreach (object o in this)
				if (o is Observeable)
					(o as Observeable).RemoveParent (this);
				else
					MSEventSupport.DisconnectEventFromObserveableList (this, o);
				
			if (Count < 1)
				return;
			noChange = false;
			items.Clear();
			OnListChanged();
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
		public bool Contains (object aObject)
		{
			if (IsValidObject (aObject) == false)
				return (false);
			 
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
		public int IndexOf (object aObject)
		{
			if (IsValidObject (aObject) == false) {
				Console.Error.WriteLine ("Shouldn't be");
				return (-1);
			}
			
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
			items.CopyTo (aArray, aIndex);
		}
		
		/// <summary>
		/// IList interface needs
		/// </summary>
		/// <param name="aObject">
		/// Object to be removed <see cref="System.Object"/>
		/// </param>
		public void Remove (object aObject)
		{
			if (IsValidObject (aObject) == true) {
				int i = IndexOf (aObject);
				if (i > -1) {
					if (onBeforeElementRemove != null)
						onBeforeElementRemove (this, aObject);
					RemoveAt (i);
					if (onAfterElementRemove != null)
						onAfterElementRemove (this, aObject);
				}
			}
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
				int c = Count;
				if (items[aIndex] is Observeable)
					(items[aIndex] as Observeable).RemoveParent (this);
				else
					MSEventSupport.DisconnectEventFromObserveableList (this, items[aIndex]);
				object o = this[aIndex];
				items.RemoveAt (aIndex);
				if (c != Count) {
					if (aIndex <= lastSearchedIndex)
						noChange = false;
					int[] idx = new int [1];
					idx[0] = aIndex;
					OnElementRemoved (idx, o);
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
		/// Object to insert <see cref="System.Object"/>
		/// </param>
		public void Insert (int aIndex, object aObject)
		{
			if ((IsValidObject (aObject) == true) && (aIndex > -1) && (aIndex < Count)) {
				int c = Count;
				items.Insert (aIndex, aObject);
				if (c != Count) {
					if (aObject is Observeable)
						(aObject as Observeable).AddParent (this);
					else
						MSEventSupport.ConnectEventToObserveableList (this, aObject);
					
					if (aIndex <= lastSearchedIndex)
						noChange = false;
					int[] idx = new int [1];
					lastSearched = items[aIndex];
					lastSearchedIndex = aIndex;
					idx[0] = aIndex;
					OnElementAdded (idx);
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
		public object this [int aIdx] {
			get { return (items[aIdx]); }
			set {
				if (items[aIdx] == value)
					return;
				if (items[aIdx] is Observeable)
					(items[aIdx] as Observeable).RemoveParent (this);
				else
					MSEventSupport.DisconnectEventFromObserveableList (this, items[aIdx]);
				
				items[aIdx] = value; 
				if (value is Observeable)
					(value as Observeable).AddParent (this);
				else
					MSEventSupport.ConnectEventToObserveableList (this, value);
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
		
		/// <summary>
		/// Notifies all connected parties and in case of Observeable list removes unneeded messages
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public override void AdapteeDataChanged (object aSender)
		{
			queue.Clear();		
			base.AdapteeDataChanged (aSender);
		}
		
		/// <summary>
		/// Posts messages about this list being changed
		/// </summary>
		private void OnListChanged()
		{
			if (IsFrozen == true) {
				queue.Clear();		
				if (HasCalledForChange == true)
					return;
				HasChanged = true;
			}
			else
				if (listChanged != null)
					listChanged (this);
			// notify parent lists
		}
		
		/// <summary>
		/// Unfreezes and if completely unfrozen, calls update notifiers
		/// </summary>
		/// <returns>
		/// Current freeze counter <see cref="System.Int32"/>
		/// </returns>
		public override int Unfreeze()
		{
			int res = base.Unfreeze();
			if (IsFrozen == false)
				if (queue.Count > 0) {
					foreach (ListAction la in queue)
						switch (la.Action) {
							case EListAction.Add:
								OnElementAdded (la.Index);
								break;
							case EListAction.Change:
								OnElementChanged (la.Index);
								break;
							case EListAction.Remove:
								OnElementRemoved (la.Index, null);
								break;
						}
					queue.Clear();
				}
			return (res);
		}
		
		/// <summary>
		/// Posts messages about this element being added to this list
		/// </summary>
		/// <param name="aIdx">
		/// Path to added element <see cref="System.Int32"/>
		/// </param>
		private void OnElementAdded (int[] aIdx)
		{
			if (IsFrozen == true) {
				queue.Add (new ListAction (EListAction.Add, aIdx));
				return;
			}
			calledForReaction = true;
			if (elementAdded != null)
				elementAdded (this, aIdx);
			// notify parent lists
			if (aIdx.Length == 1)
				ListChildChanged (null, EListAction.Add, aIdx);
			// Now call for each and every object to preserve sinchronization
			// between threads. Addition is the only feature that has to expose
			// action this way
			object o = HierarchicalList.Get (this, aIdx);
			if (o != null)
				if (o is IObserveableList)
					CallElementAddedInTree ((o as IObserveableList), aIdx);

			calledForReaction = false;
		}

		/// <summary>
		/// Posts messages about elements being added. This way sinchronization
		/// can be preserved between different threads
		/// </summary>
		/// <param name="aList">
		/// List which added new list as element <see cref="IObserveableList"/>
		/// </param>
		/// <param name="aIdx">
		/// Original index of change <see cref="System.Int32"/>
		/// </param>
		private void CallElementAddedInTree (IObserveableList aList, int[] aIdx)
		{
			int[] newpath = aIdx.AddPathIndexOnEnd (0);
			for (int i=0; i<aList.Count; i++) {
				newpath[newpath.Length-1] = i;
				if (elementAdded != null)
					elementAdded (this, newpath);

				object o = HierarchicalList.Get (this, newpath);
				if (o != null)
					if (o is IObserveableList)
						CallElementAddedInTree ((o as IObserveableList), newpath);
				// notify parent lists
				if (aIdx.Length == 1)
					ListChildChanged (null, EListAction.Add, newpath);
			}
		}
		
		/// <summary>
		/// Posts messages about this element being changed in this list
		/// </summary>
		/// <param name="aIdx">
		/// Path to changed element <see cref="System.Int32"/>
		/// </param>
		private void OnElementChanged (int[] aIdx)
		{
			if (IsFrozen == true) {
				queue.Add (new ListAction (EListAction.Change, aIdx));
				return;
			}
			calledForReaction = true;
			if (elementChanged != null)
				elementChanged (this, aIdx);
			// notify parent lists
			if (aIdx.Length == 1)
				ListChildChanged (null, EListAction.Change, aIdx);
			calledForReaction = false;
		}
		
		/// <summary>
		/// Posts messages about this element being removed from this list
		/// </summary>
		/// <param name="aIdx">
		/// Path to removed element <see cref="System.Int32"/>
		/// </param>
		private void OnElementRemoved (int[] aIdx, object aObject)
		{
			if (IsFrozen == true) {
				queue.Add (new ListAction (EListAction.Remove, aIdx));
				return;
			}
			calledForReaction = true;
			if (elementRemoved != null)
				elementRemoved (this, aIdx, aObject);
			// notify parent lists
			if (aIdx.Length == 1)
				ListChildChanged (null, EListAction.Remove, aIdx);
			calledForReaction = false;
		}
		
		/// <summary>
		/// Checks if list is valid type to be wrapped around with ObserveableList
		/// </summary>
		/// <param name="aObject">
		/// Object to check <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if valid, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsValidListStore (object aObject)
		{
			if ((ConnectionProvider.ResolveTargetForObject (aObject) is IList) ||
			    (ConnectionProvider.ResolveTargetForObject (aObject) is IListSource))
				return (true);
			return (false);
		}

		/// <summary>
		/// Creates ObserveableList
		/// </summary>
		public ObserveableList()
			: base ()
		{
			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			items = CreateList();
			if (items == null)
				throw new ExceptionObserveableListCreatedWithNullList();
		}

		/// <summary>
		/// Creates Observeable list wit aList as its list storage
		/// </summary>
		/// <param name="aList">
		/// List to use as storage <see cref="IList"/>
		/// </param>
		public ObserveableList (IList aList)
		{
			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			items = aList;
			if (items == null)
				throw new ExceptionObserveableListCreatedWithNullListParameter();
		}
		
		/// <summary>
		/// Destroys ObserveableList 
		/// </summary>
		~ObserveableList()
		{
			queue.Clear();
			queue = null;
			items.Clear();
			items = null;
		}*/

		/// <summary>
		/// Creates ObserveableList
		/// </summary>
		public ObserveableList()
			: base ()
		{
		}

		/// <summary>
		/// Creates Observeable list wit aList as its list storage
		/// </summary>
		/// <param name="aList">
		/// List to use as storage <see cref="IList"/>
		/// </param>
		public ObserveableList (IList aList)
			: base (aList)
		{
		}
	}
}
