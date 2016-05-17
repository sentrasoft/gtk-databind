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
	/// Provides sorted view of master list
	/// </summary>	
	public class ObservableSortListView : BaseClientObservableListView, IObservableList
	{
		private List<object> localIndex = new List<object>();
		private AVLTreeSort<object> items = null;
		
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
					foreach (object o in items)
						yield return (o);
				}
			}
		}

		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerable"/>
		/// </returns>
		public IEnumerable Distinct()
		{
			lock (this) {
				lock (ParentView) {
					foreach (object o in items.Distinct())
						yield return (o);
				}
			}
		}

		#endregion IEnumerable

		#region IList 

		/// <value>
		/// Returns item count, if list is not filtered it returns ParentView item count
		/// </value>
		public int Count {
			get { return (items.Count);	}
		}
		
		/// <value>
		/// Gets or sets item in list
		/// </value>
		public object this[int aIdx] {
			get { return (items[aIdx]); }
			set { 
				if ((aIdx < 0) || (aIdx >= Count))
					throw new OutOfBoundsException();
				
				int idx = TranslateIndex (aIdx);
				if (idx > -1)
					ParentView[idx] = value;
				else
					throw new ItemNotFoundException();
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
			return (ParentView.Add (value));
		}
		
		/// <summary>
		/// Clears items in its self, which means it removes only items
		/// present in this filter
		/// </summary>
		public void Clear ()
		{
			ParentView.Clear();
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
			return (items.Contains (value));
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
		public bool ContainsKey (object value)
		{
			return (items.ContainsKey (value));
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
			return (items.IndexOf (value));
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
			ParentView.RemoveAt (TranslateIndex (aIdx));
		}
		
		public void Remove (object value)
		{
			ParentView.Remove (value);
		}
		
		#endregion IList 

		#region ICollection 
		
		public void CopyTo (Array aArray, int aIndex)
		{
			object[] arr = new object [Count-aIndex];
			for (int i=aIndex; i<Count; i++)
				arr[i-aIndex] = this[i];
			arr.CopyTo (aArray, 0);
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
				
				return (HierarchicalList.Get (this, aIdx));
			}
		}

		#endregion IObservableList 

		public void Resort()
		{
			items.Clear();
			localIndex.Clear();
			lock (ParentView) {
				for (int i=0; i<ParentView.Count; i++) {
					localIndex.Add (ParentView[i]);
					items.Insert (ParentView[i]);
				}
			}
		}
		
		private int TranslateIndex (int aIndex)
		{
			return (items.IndexOf (localIndex[aIndex]));
		}
		
		protected override void MasterListChanged (object aList)
		{
			Resort();
			OnListChanged();
		}

		protected override void ElementsSortedInMaster (object aObject, int[] aIdx)
		{
			Resort();
			OnListChanged();
		}

		protected override void ElementAddedIntoMaster (object aList, int[] aIdx)
		{
			int[] idx = aIdx.CopyPath();
			idx[0] = TranslateIndex (aIdx[0]);
			object o = HierarchicalList.Get (ParentView, aIdx);
			if (aIdx.Length > 1) {
				if (idx[0] > -1)
					OnElementAdded (idx);
			}
			else if ((idx[0] > -1) && (o != null)) {
				items.Insert (o);
				if (idx[0] >= localIndex.Count)
					localIndex.Add (o);
				else
					localIndex.Insert (aIdx[0], o);
				OnElementAdded (idx);
				OnPropertyChanged ("Count");
			}
			idx = null;
		}

		protected override void ElementChangedInMaster (object aList, int[] aIdx)
		{
			int[] idx = aIdx.CopyPath();
			idx[0] = TranslateIndex (aIdx[0]);
			object o = HierarchicalList.Get (ParentView, aIdx);
			// Check if level is higher
			if (aIdx.Length > 1) {
				OnElementChanged (idx);
				idx = null;
				return;
			}

			items.Remove (localIndex[aIdx[0]], InstanceInformation.ObjectInstance);
			items.Insert (o);
			localIndex[aIdx[0]] = o;
			idx[0] = items.IndexOf (o);
			OnElementChanged (idx);
			idx = null;
		}

		protected override void ElementRemovedFromMaster (object aList, int[] aIdx, object aObject)
		{
			int[] idx = aIdx.CopyPath();
			idx[0] = TranslateIndex (aIdx[0]);
			if (aIdx.Length > 1) {
				OnElementRemoved (idx, aObject);
				return;
			}
			localIndex.RemoveAt (aIdx[0]);
			OnElementRemoved (idx, aObject);
			OnPropertyChanged ("Count");
		}

		public override void Disconnect ()
		{
			items.Disconnect();
			base.Disconnect ();
		}

		private ObservableSortListView()
			: base (null)
		{
		}
		
		public ObservableSortListView (IList aParentList, CompareMethod<object> aSortMethod, DuplicateHandlingType aDuplicateHandling)
			: base (aParentList)
		{
			items = new AVLTreeSort<object> (aSortMethod, aDuplicateHandling);
		}
	}
}
