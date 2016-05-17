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
using System.Data.Bindings.Collections.Generic;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Provides filtered view on list
	/// </summary>
	public class ObservableFilterListView : BaseClientObservableListView, IObservableList, IFiltered
	{
		private List<bool> itemVisibility = new List<bool>();
		private List<int> localIndex = new List<int>();
		
		private bool unsynced = false;

		#region IFiltered
		
		private StrictnessType strictness = StrictnessType.NonStrict;
		/// <value>
		/// Defines if class should act strictly with filter or not
		/// </value>
		public StrictnessType Strictness { 
			get { return (strictness); }
			set { strictness = value; }
		}
		
		private bool isFiltered = false;
		/// <value>
		/// Returns true if filter is specified and list is filtered
		/// </value>
		public bool IsFiltered {
			get { return (isFiltered); }
			private set {
				if (isFiltered == value)
					return;
				isFiltered = value;
				if (isFiltered == false) {
					localIndex.Clear();
					unsynced = true;
				}
				else {
					localIndex.Clear();
					unsynced = true;
				}
			}
		}
		
		private event IsVisibleInFilterEvent isVisibleInFilter = null;
		/// <summary>
		/// Specifies filtering methods
		/// </summary>
		public event IsVisibleInFilterEvent IsVisibleInFilter {
			add { 
				bool b = IsFiltered;
				isVisibleInFilter += value;
				IsFiltered = true;
				if (b != IsFiltered) {
					Refilter();
				}
				else
					FilterMore();
			}
			remove { 
				bool b = IsFiltered;
				isVisibleInFilter -= value; 
				if (b != IsFiltered) {
					IsFiltered = (isVisibleInFilter != null);
					Refilter();
				}
				else
					FilterLess();
			}
		}
		
		protected bool OnIsVisibleInFilter (object aObject) 
		{
			if (IsFiltered == false)
				return (true);
			bool res;
			foreach (Delegate delegatemethod in isVisibleInFilter.GetInvocationList()) {
//			foreach (IsVisibleInFilterEvent delegatemethod in isVisibleInFilter.GetInvocationList()) {
				if (delegatemethod != null) {
					res = (bool) delegatemethod.DynamicInvoke (new object[1] {aObject});
//					res = delegatemethod (aObject);
					if (res == false)
						return (false);
				}
			}
			return (true);
		}

		/// <summary>
		/// Calls to refilter only invisible items
		/// </summary>
		public void FilterLess()
		{
			if (itemVisibility.Count != ParentView.Count) {
				Refilter();
				return;
			}
			for (int i=0; i<ParentView.Count; i++)
				if (itemVisibility[i] == true) {
					itemVisibility[i] = OnIsVisibleInFilter (ParentView[i]);
					if (itemVisibility[i] == false) {
						localIndex.Remove (i);
						OnElementRemoved (new int[1] {i}, ParentView[i]);
					}
				}
		}
		
		/// <summary>
		/// Calls to refilter only visible items
		/// </summary>
		public void FilterMore()
		{
			if (itemVisibility.Count != ParentView.Count) {
				Refilter();
				return;
			}
			for (int i=0; i<ParentView.Count; i++)
				if (itemVisibility[i] == true) {
					itemVisibility[i] = OnIsVisibleInFilter (ParentView[i]);
					if (itemVisibility[i] == false) {
						localIndex.SortedAdd(i);
						OnElementAdded (new int[1] {i});
					}
				}
		}
		
		public void Refilter()
		{
			if (IsFiltered == false)
				return;
			localIndex.Clear();
			if (itemVisibility.Count != ParentView.Count) {
				itemVisibility.Clear();
				for (int i=0; i<ParentView.Count; i++)
					itemVisibility.Add (false);
			}
			
			for (int i=0; i<ParentView.Count; i++) {
				bool b = OnIsVisibleInFilter (ParentView[i]);
				itemVisibility[i] = b;
				if (b == true)
					localIndex.Add (i);
			}
			OnListChanged();
		}
		
		/// <summary>
		/// Calls clear on parent view
		/// </summary>
		public override void ClearParent()
		{
			unsynced = true;
			base.ClearParent();
		}
		
		/// <summary>
		/// Calls clear on master view
		/// </summary>
		public override void ClearMaster()
		{
			unsynced = true;
			base.ClearMaster();
		}

		#endregion IFiltered

		#region IEnumerable
		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			lock (this) {
				lock (ParentView) {
					if (IsFiltered == false) {
						foreach (object o in ParentView)
							yield return (o);
					}
					else {
						for (int i=0; i<localIndex.Count; i++)
							yield return (ParentView[localIndex[i]]);
					}
				}
			}
		}

		#endregion IEnumerable

		#region IList 

		/// <value>
		/// Returns item count, if list is not filtered it returns ParentView item count
		/// </value>
		public int Count {
			get {
				if (IsFiltered == true)
					return (localIndex.Count);
				return (ParentView.Count);
			}
		}
		
		/// <value>
		/// Gets or sets item in list
		/// </value>
		public object this[int aIdx] {
			get {
				if (IsFiltered == false)
					return (ParentView[aIdx]);
				if ((aIdx < 0) || (aIdx >= Count))
					throw new IndexOutOfRangeException (string.Format ("Index {0} is out of bounds. Count={1}", aIdx, Count));
				return (ParentView[localIndex[aIdx]]);
			}
			set {
				if (IsFiltered == false) {
					ParentView[aIdx] = value;
					return;
				}
				ParentView[localIndex[aIdx]] = value;
			}
		}
		
		/// <summary>
		/// Calls ParentView.Add
		/// </summary>
		/// <param name="value">
		/// Value to be added <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Position <see cref="System.Int32"/>
		/// </returns>
		/// <remarks>
		/// Returned position is relative to actual list where it has happened,
		/// users of IListEvents are strongly incouraged to use events instead
		/// </remarks>
		public int Add (object value)
		{
			if (IsFiltered == true) {
				bool valid = OnIsVisibleInFilter (value);
				if (valid == false) {
					switch (Strictness) {
					case StrictnessType.RefuseAction:
						return (-1);
					case StrictnessType.ThrowException:
						throw new OutOfBoundsException ("Adding object out of filter constraints");
					}
				}
			}
			return (ParentView.Add (value));
		}
		
		/// <summary>
		/// Clears items in its self, which means it removes only items
		/// present in this filter
		/// </summary>
		public void Clear ()
		{
			if (IsFiltered == true) {
				if (Count == 0)
					return;
				for (int i=Count-1; i>=0; i--)
					ParentView.RemoveAt (localIndex[i]);
				return;
			}
			else
				ClearParent();
		}
		
		/// <summary>
		/// Checks if view contains specified object
		/// </summary>
		/// <param name="value">
		/// Searched object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if yes, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (object value)
		{
			if (IsFiltered == true) {
				if (OnIsVisibleInFilter(value) == false)
					return (false);
				for (int i=0; i<Count; i++)
					if (this[i] == value)
						return (true);
				return (false);
			}
			return (ParentView.Contains(value));
		}
		
		/// <summary>
		/// Returns index of specified item
		/// </summary>
		/// <param name="value">
		/// Searched object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		public int IndexOf (object value)
		{
			if (IsFiltered == false)
				return (ParentView.IndexOf (value));
			
			for (int i=0; i<Count; i++)
				if (this[i] == value)
					return (i);
			return (-1);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aIndex">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="value">
		/// A <see cref="System.Object"/>
		/// </param>
		public void Insert (int aIndex, object value)
		{
			if (IsFiltered == true) {
				bool valid = OnIsVisibleInFilter (value);
				if (valid == false) {
					switch (Strictness) {
					case StrictnessType.RefuseAction:
						return;
					case StrictnessType.ThrowException:
						throw new OutOfBoundsException ("Inserting object out of filter constraints");
					}
				}
				ParentView.Insert (localIndex[aIndex], value);
			}
			else
				ParentView.Insert (aIndex, value);
		}
		
		public bool IsFixedSize {
			get { return (ParentView.IsFixedSize); }
		}
		
		public bool IsReadOnly {
			get { return (ParentView.IsReadOnly); }
		}
		
		public void RemoveAt (int aIdx)
		{
			if (IsFiltered == true)
				ParentView.RemoveAt (localIndex[aIdx]);
			else
				ParentView.RemoveAt (aIdx);
		}
		
		public void Remove (object value)
		{
			if (IsFiltered == true) {
				bool valid = OnIsVisibleInFilter (value);
				if (valid == false) {
					switch (Strictness) {
					case StrictnessType.RefuseAction:
						return;
					case StrictnessType.ThrowException:
						throw new OutOfBoundsException ("Removing object out of filter constraints");
					}
				}
			}
			ParentView.Remove (value);
		}
		
		#endregion IList 

		#region ICollection 
		
		public void CopyTo (Array aArray, int aIndex)
		{
			if (IsFiltered == true) {
				object[] arr = new object [Count-aIndex];
				for (int i=aIndex; i<Count; i++)
					arr[i-aIndex] = this[i];
				arr.CopyTo (aArray, 0);
			}
			else
				ParentView.CopyTo (aArray, aIndex);
		}
		
		public bool IsSynchronized {
			get { return (ParentView.IsSynchronized); }
		}
		
		public object SyncRoot {
			get { return (ParentView.SyncRoot); }
		}

		#endregion ICollection 

		#region IObservableList 
		
		public IList CreateList ()
		{
			throw new NotSupportedException ("Filter list can't create lists");
		}
		
		internal PropertyChangedEventHandler ListItemPropertyChangedMethod = null;
		
		internal void ListItemsPropertyChanged (object aSender, PropertyChangedEventArgs e)
		{
		}
		
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
		
		public object this [int[] aIdx] {
			get {
				if ((aIdx == null) || (aIdx.Length == 0))
					throw new NullReferenceException ("Hierarchical index (int[]) can't be null or empty");
				
				if (IsFiltered == false)
					if (TypeValidator.IsCompatible(ParentView.GetType(), typeof(IObservableList)) == true)
						return ((ParentView as IObservableList)[aIdx]);
					else
						return (HierarchicalList.Get (ParentView, aIdx));
				
				if ((aIdx[0] < 0) || (aIdx[0] >= Count))
					throw new IndexOutOfRangeException(string.Format ("Index {0} is out of range", aIdx.PathToString()));

				return (HierarchicalList.Get (this, aIdx));
			}
		}

		#endregion IObservableList 

		protected override void MasterListChanged (object aList)
		{
			unsynced = true;
			Refilter();
		}

		protected override void ElementsSortedInMaster (object aObject, int[] aIdx)
		{
			unsynced = true;
			Refilter();
		}

		protected override void ElementAddedIntoMaster (object aList, int[] aIdx)
		{
			if (IsFiltered == false) {
				OnElementAdded (aIdx);
				OnPropertyChanged ("Count");
				return;
			}
			
			// Correct local index
			object obj = HierarchicalList.Get (ParentView, aIdx);
			bool valid = OnIsVisibleInFilter (obj);
			if (aIdx.Length == 1) {
				itemVisibility.SafeInsert (aIdx[0], valid);
				if (valid == true)
					localIndex.SortedAdd (aIdx[0]);
			}

			// Handle messaging
			int[] idx = aIdx.CopyPath();
			idx[0] = localIndex.IndexOf (aIdx[0]);
			if (aIdx.Length > 1) {
				if (OnIsVisibleInFilter(obj) == false)
					return;
				if (idx[0] != -1)
					OnElementAdded (idx);
				idx = null;
				return;
			}
			if (idx[0] > -1) {
				for (int i=0; i<localIndex.Count; i++)
					if (localIndex[i] > aIdx[0])
						localIndex[i]++;
				OnElementAdded (idx);
				OnPropertyChanged ("Count");
			}
			idx = null;
		}

		protected override void ElementChangedInMaster (object aList, int[] aIdx)
		{
			if (IsFiltered == false) {
				OnElementChanged (aIdx);
				return;
			}
			
			// Check if level is higher
			if (aIdx.Length > 1) {
				if (itemVisibility[aIdx[0]] == false)
					return;
				int[] lidx = aIdx.CopyPath();
				lidx[0] = localIndex.IndexOf (aIdx[0]);
				OnElementChanged (lidx);
				lidx = null;
				return;
			}
			
			// Correct local index
			bool prevState = itemVisibility[aIdx[0]];
			object obj = HierarchicalList.Get (ParentView, aIdx);
			bool valid = OnIsVisibleInFilter (obj);
			itemVisibility[aIdx[0]] = valid;
			if ((prevState == valid) && (valid == false))
				return;

			int[] idx = aIdx.CopyPath();
			if ((prevState == valid) && (valid == true)) {
				idx[0] = localIndex.IndexOf (aIdx[0]);
				OnElementChanged (idx);
				return;
			}
			
			if (valid == true) {
				idx[0] = localIndex.SortedAdd (aIdx[0]);
				OnElementAdded (idx);
				OnPropertyChanged ("Count");
			}
			else {
				idx[0] = localIndex.IndexOf (aIdx[0]);
				localIndex.Remove (aIdx[0]);
				OnElementRemoved (idx, obj);
				OnPropertyChanged ("Count");
			}
			idx = null;
		}

		protected override void ElementRemovedFromMaster (object aList, int[] aIdx, object aObject)
		{
			if (IsFiltered == false) {
				OnElementRemoved (aIdx, aObject);
				OnPropertyChanged ("Count");
				return;
			}
			if (itemVisibility[aIdx[0]] == false) {
				for (int i=0; i<localIndex.Count; i++)
					if (localIndex[i] >= aIdx[0])
						localIndex[i]--;
				itemVisibility.RemoveAt(aIdx[0]);
				return;
			}
			int[] idx = aIdx.CopyPath();
			idx[0] = localIndex.IndexOf (aIdx[0]);
			if (aIdx.Length > 1) {
				OnElementRemoved (idx, aObject);
				return;
			}
			localIndex.Remove (aIdx[0]);
			// correct indexes
			for (int i=0; i<localIndex.Count; i++)
				if (localIndex[i] >= aIdx[0])
					localIndex[i]--;
			OnElementRemoved (idx, aObject);
			OnPropertyChanged ("Count");
		}

		public override void Disconnect ()
		{
			isVisibleInFilter = null;
			base.Disconnect ();
		}

		private ObservableFilterListView()
			: base (null)
		{
		}
		
		public ObservableFilterListView (IList aParentList)
			: base (aParentList)
		{
		}
	}
}
