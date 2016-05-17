// DbObservableList.cs - IList approach to DataTable and DataView providing databinding events
//
// Author: Tadej <tadej@arsis.net>
//
// Copyright (c) 2009 tadey.
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
using System.Data;
using System.Data.Bindings;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Provides IList access to DataTable or DataView
	/// </summary>
	public class DbObservableList : BaseNotifyPropertyChanged, IDbObservableList, IDisconnectable
	{
		#region PRIVATE
		private object master = null;
		#endregion PRIVATE

		/// <value>
		/// Returns reference to table
		/// </value>
		public DataTable Table {
			get { 
				return (master is DataTable) ? (master as DataTable) : (master as DataView).Table;
			}
		}
		
		/// <value>
		/// Returns reference to table
		/// </value>
		public DataView DefaultView {
			get { 
				return (master is DataTable) ? (master as DataTable).DefaultView : (master as DataView);
			}
		}
		
		/// <value>
		/// Returns row count in wrapped datatable or dataview
		/// </value>
		public int Count {
			get { return (master is DataTable) ? (master as DataTable).Rows.Count : (master as DataView).Count; }
		}
		
		public int IndexOf(object aValue)
		{
			if (master is DataView) {
				lock (master) {
					lock (Table) {
						DataView view = (master as DataView);
						for (int i=0; i<Count; i++)
							if (aValue == view[i].Row)
								return (i);
					}
				}
				return (-1);
			}
			return (Table.Rows.IndexOf(aValue as DataRow));
		}
		
		public void AcceptChanges()
		{
			Table.AcceptChanges();
		}
		
		public void RejectChanges()
		{
			Table.RejectChanges();
		}
		
		#region IEnumerable implementation
		
		public IEnumerator GetEnumerator()
		{
			lock (master) {
				lock (Table) { //isn't Table already locked as it's calling master anyway .. ?
					System.Console.WriteLine("[{0}] Get enumerator", master.GetType().ToString());
					if (master is DataTable)
						foreach (DataRow row in (master as DataTable).Rows)
							yield return (row);
					else 
						foreach (DataRowView row_view in (master as DataView))
							yield return (row_view.Row);
				}
			}
		}

		#endregion

		#region IDisconnectable implementation
		
		/// <summary>
		/// Disconnects everything for GC
		/// </summary>
		public void Disconnect()
		{
			Table.RowChanged -= new DataRowChangeEventHandler(Table_RowChanged);
			Table.RowChanging -= new DataRowChangeEventHandler(Table_RowChanging);
			Table.RowDeleting -= Table_RowDeleting;
			Table.RowDeleted -= Table_RowDeleted;
			/* if (master is DataView)
				(master as DataView).ListChanged -= View_ListChanged; */
		}
		
		#endregion
		
		protected void Connect()
		{
			Table.RowChanging += new DataRowChangeEventHandler(Table_RowChanging);
			Table.RowChanged += new DataRowChangeEventHandler(Table_RowChanged);
			Table.RowDeleting += Table_RowDeleting;
			Table.RowDeleted += Table_RowDeleted;
			if (master is DataView)
				(master as DataView).ListChanged += View_ListChanged;
		}

		private void View_ListChanged(object sender, ListChangedEventArgs e)
		{
			//we should only be here if our master is DataView
			Console.WriteLine("{3}; View_ListChanged; e.ListChangedType={0}; newIndex: {1}; oldIndex: {2}", e.ListChangedType, e.NewIndex, e.OldIndex, this.GetHashCode());
			if (e.ListChangedType == ListChangedType.Reset)
				OnListChanged();
			else if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
			}
			else if (e.ListChangedType == ListChangedType.ItemMoved)
				OnElementsSorted();
			else if (e.ListChangedType == ListChangedType.ItemAdded)
				OnElementAdded(e.NewIndex);
		}

		private Hashtable indexes = new Hashtable();
		private Hashtable previous_states = new Hashtable();
		
		private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
		{
			System.Console.WriteLine("{1}; Table_RowChanging: e.Row.RowState={0}", e.Row.RowState, this.GetHashCode());
			int hash = e.Row.GetHashCode(); //TODO check whether or not a situation can arise where the Row won't exists (and thus won't let us calculate hash) or that the hash would change ..
			if (previous_states.ContainsKey(hash))
				previous_states[hash] = e.Row.RowState;
			else
				previous_states.Add(hash, e.Row.RowState);

			if (e.Action == DataRowAction.Commit)
			{
				if (e.Row.RowState == DataRowState.Deleted)
				{
					int row_index = IndexOf(e.Row);
					if (indexes.ContainsKey(hash))
						indexes[hash] = row_index;
					else
						indexes.Add (hash, row_index);
				}
			}
			else if (e.Action == DataRowAction.Rollback)
			{
				if (e.Row.RowState == DataRowState.Added) //it's currently added, but it'll be removed in just a sec, because we're doing a rollback
				{
					int row_index = IndexOf(e.Row);
					if (indexes.ContainsKey(hash))
						indexes[hash] = row_index;
					else
						indexes.Add(hash, row_index);
				}
				else if (e.Row.RowState == DataRowState.Deleted) //shows up in RowChanged as Unchanged
				{
				}
				else if (e.Row.RowState == DataRowState.Modified) //shows up in RowChanged as Unchanged
				{
				}
			}
		}
		
		private void Table_RowChanged (object sender, DataRowChangeEventArgs e)
		{
			System.Console.WriteLine("{1}; Table_RowChanged: e.Action={0}", e.Action, this.GetHashCode());
			int hash = e.Row.GetHashCode();
			if (e.Action == DataRowAction.Add && master is DataTable)
				OnElementAdded (IndexOf(e.Row));
			else if (e.Action == DataRowAction.Change)
				OnElementChanged (IndexOf(e.Row));
			else if (e.Action == DataRowAction.Rollback)
			{
				System.Console.WriteLine("{2}; Rollback: state: {0}; index: {1}", e.Row.RowState, IndexOf(e.Row), this.GetHashCode());
				if (e.Row.RowState == DataRowState.Added)
					OnElementRemoved (IndexOf(e.Row), e.Row);
				else if (e.Row.RowState == DataRowState.Modified)
					OnElementChanged (IndexOf(e.Row));
				else if (e.Row.RowState == DataRowState.Deleted)
					OnElementAdded (IndexOf(e.Row));
				else if (e.Row.RowState == DataRowState.Detached)
				{
					int index = (indexes[hash] == null) ? -1 : (int)indexes[hash];
					if (index > -1)
					{
						OnElementRemoved((int)indexes[hash], e.Row);
						indexes.Remove(hash);
					}
				}
				else if (e.Row.RowState == DataRowState.Unchanged)
				{
					if ((DataRowState)previous_states[hash] == DataRowState.Deleted)
					{
					}
					else if ((DataRowState)previous_states[hash] == DataRowState.Modified)
						OnElementChanged(IndexOf(e.Row));
				}
			}
			else if (e.Action == DataRowAction.Commit)
			{
				if (e.Row.RowState == DataRowState.Detached) //this is (hopefully) only emitted with item delete; should only happen with DataTable
				{
					int index = (indexes[hash] == null) ? -1 : (int)indexes[hash];
					if (index > -1)
					{
						OnElementRemoved((int)indexes[hash], e.Row);
						indexes.Remove(hash);
					}
				}
			}
		}
		
		private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
		{
			System.Console.WriteLine("{1}; Table_RowDeleting: e.Row.RowState={0}", e.Row.RowState, this.GetHashCode());
			if (e.Row.RowState == DataRowState.Added || e.Row.RowState == DataRowState.Modified || e.Row.RowState == DataRowState.Unchanged)
			{
				int hash = e.Row.GetHashCode();
				int row_index = IndexOf(e.Row);
				if (indexes.ContainsKey(hash))
					indexes[hash] = row_index;
				else
					indexes.Add(hash, row_index);
			}
		}

		private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
		{
			System.Console.WriteLine("{1}; Table_RowDeleted: e.Row.RowState={0}", e.Row.RowState, this.GetHashCode());
			if (e.Row.RowState == DataRowState.Detached || e.Row.RowState == DataRowState.Deleted)
			{
				int key = e.Row.GetHashCode();
				int index = (indexes[key] == null) ? -1 : (int)indexes[key];
				if (index > -1)
				{
					if (master is DataTable)
						OnElementChanged ((int)indexes[key]);
					else
						OnElementRemoved ((int)indexes[key], e.Row);
					indexes.Remove (key);
				}
			}
		}
		
		#region IDbObservableList implementation
				
		public IList CreateList()
		{
			throw new NullReferenceException ("DbObservableList needs to be created from existing objects");
		}
		
		internal void ListItemsPropertyChanged (object aSender, PropertyChangedEventArgs e)
		{
			// Already handled in RowChanged handling, 
/*			if (aSender == null)
				return;
			int i = IndexOf (aSender);
			if (i != -1)
				OnElementChanged (i);*/
		}

//		internal PropertyChangedEventHandler ListItemPropertyChangedMethod = null;

		public PropertyChangedEventHandler GetDefaultNotifyPropertyChangedHandler()
		{
//			return (ListItemPropertyChangedMethod);
			return (null);
		}
		
		/// <value>
		/// Returns row at specified index
		/// </value>
		/// <remarks>
		/// Only single index is allowed here as it doesn't support hierarchy
		/// </remarks>
		public object this[int[] aIdx] {
			get {
				if (aIdx == null)
					throw new NullReferenceException(string.Format("DbObservableList[int[] aIdx=null]", aIdx));
				if (aIdx.Length > 1)
					throw new IndexOutOfRangeException(string.Format("DbObservableList[int aIdx={0}]", aIdx.PathToString()));
				return (this[(aIdx as int[])[0]]);
			}
		}

		#endregion

		#region IListEvents implementation
		
		private event ElementAddedInListObjectEvent elementAdded = null;
		public event ElementAddedInListObjectEvent ElementAdded {
			add { elementAdded += value; }
			remove { elementAdded -= value; }
		}
		
		protected void OnElementAdded (int aIdx)
		{
			System.Console.WriteLine("{1}; OnElementAdded: {0}", aIdx, this.GetHashCode());
			if (aIdx < 0) return;
			OnPropertyChanged("Items");
			int[] idx = new int[1];
			idx[0] = aIdx;
			if (elementAdded != null)
				elementAdded (this, idx);
		}
		
		private event ElementChangedInListObjectEvent elementChanged = null;
		public event ElementChangedInListObjectEvent ElementChanged {
			add { elementChanged += value; }
			remove { elementChanged -= value; }
		}
		
		protected void OnElementChanged (int aIdx)
		{
			System.Console.WriteLine("{1}; OnElementChanged: {0}", aIdx, this.GetHashCode());
			if (aIdx < 0) return;
			OnPropertyChanged("Items");
			int[] idx = new int[1];
			idx[0] = aIdx;
			if (elementChanged != null)
				elementChanged (this, idx);
			if (master is DataView && (master as DataView).Sort.Length > 0)
				OnElementsSorted();
		}
		
		private event ElementRemovedFromListObjectEvent elementRemoved = null;
		public event ElementRemovedFromListObjectEvent ElementRemoved {
			add { elementRemoved += value; }
			remove { elementRemoved -= value; }
		}
		
		protected void OnElementRemoved (int aIdx, object aObject)
		{
			System.Console.WriteLine("{1}; OnElementRemoved: {0}", aIdx, this.GetHashCode());
			if (aIdx < 0) return;
			OnPropertyChanged("Items");
			int[] idx = new int[1];
			idx[0] = aIdx;
			if (elementRemoved != null)
				elementRemoved (this, idx, aObject);
		}
		
		private event ElementsInListSortedEvent elementsSorted = null;
		public event ElementsInListSortedEvent ElementsSorted {
			add { elementsSorted += value; }
			remove { elementsSorted -= value; }
		}
		
		protected void OnElementsSorted()
		{
			System.Console.WriteLine("{0}; OnElementsSorted", this.GetHashCode());
			OnPropertyChanged("Items");
			if (elementsSorted != null)
				elementsSorted (this, null);
		}
		
		private event ListChangedEvent listChanged = null;
		public event ListChangedEvent ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}

		protected void OnListChanged()
		{
			System.Console.WriteLine("{0}; OnListChanged", this.GetHashCode());
			OnPropertyChanged("Items");
			if (listChanged != null)
				listChanged (this);
		}
		
		#endregion

		/// <summary>
		/// To be removed - only for testing
		/// </summary>
		public void Refresh()
		{
			OnListChanged();
		}
		
		#region IList Members

		public int Add (object value)
		{
			if (value == null)
				return (-1);
			if (TypeValidator.IsCompatible(value.GetType(), typeof(DataRow)) == false)	
				throw new NotSupportedException ("DbObservableList Add: value is of wrong type");
			Table.Rows.Add(value as DataRow);
			return (Count); //should be the index of our new row
		}

		public void Clear()
		{
			Table.Rows.Clear();
		}

		public bool Contains (object value)
		{
			if (TypeValidator.IsCompatible(value.GetType(), typeof(DataRow)) == false)	
				throw new NotSupportedException ("DbObservableList Contains: value is of wrong type");
			return (Table.Rows.Contains (value));
		}

		public void Insert (int aIndex, object value)
		{
			if (TypeValidator.IsCompatible(value.GetType(), typeof(DataRow)) == false)	
				throw new NotSupportedException ("DbObservableList Insert: value is of wrong type");
			if ((aIndex < 0) || (aIndex >= Count))
				throw new IndexOutOfRangeException (string.Format("DbObservableList Insert: index is {0}", aIndex));
			Table.Rows.InsertAt (value as DataRow, aIndex);
		}

		public bool IsFixedSize {
			get { return (false); }
		}

		public bool IsReadOnly {
			get { return (Table.Rows.IsReadOnly); }
		}

		public void Remove(object value)
		{
			if (TypeValidator.IsCompatible(value.GetType(), typeof(DataRow)) == false)	
				throw new NotSupportedException ("DbObservableList Remove: value is of wrong type");
			Table.Rows.Remove (value as DataRow);
		}

		public void RemoveAt(int aIndex)
		{
			if ((aIndex < 0) || (aIndex >= Count))
				throw new IndexOutOfRangeException(string.Format("DbObservableList RemoveAt: index is {0}", aIndex));
			Table.Rows.RemoveAt (aIndex);
		}

		public object this[int aIdx] {
			get {
				if ((aIdx < 0) || (aIdx >= Count))
					throw new IndexOutOfRangeException(string.Format("DbObservableList[int aIdx={0}]", aIdx));
				return ((master is DataTable) ? Table.Rows[aIdx] : (master as DataView)[aIdx].Row);
			}
			set { throw new NotImplementedException ("Not implemented"); }
		}

		#endregion

		#region ICollection Members

		public void CopyTo (Array aArray, int aIndex)
		{
			Table.Rows.CopyTo (aArray, aIndex);
		}

		public bool IsSynchronized {
			get { return (Table.Rows.IsSynchronized); }
		}

		public object SyncRoot {
			get { return (Table.Rows.SyncRoot); }
		}

		#endregion

		public DbObservableList (DataTable aTable)
		{
//			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			if (aTable == null)
				throw new NullReferenceException ("NULL table assigned to DbObservableList");
			master = aTable;

			Connect();
		}
		
		public DbObservableList (DataView aView)
		{
//			ListItemPropertyChangedMethod = new PropertyChangedEventHandler (ListItemsPropertyChanged);
			if (aView == null)
				throw new Exception ("NULL view assigned to DbObservableList");
			master = aView;

			Connect();
		}
		
		~DbObservableList()
		{
			Disconnect();
		}
	}
}