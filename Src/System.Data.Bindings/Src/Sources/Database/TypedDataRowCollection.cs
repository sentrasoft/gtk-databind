
using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Data.Bindings.Generic
{
	/// <summary>
	/// Provides type safe access to DataTable Rows. Every method here
	/// is just type safe redirect to Rows
	/// </summary>
	public class TypedDataRowCollection<T> : InternalDataCollectionBase, IEnumerable<T>
		where T : DataRow
	{
		private WeakReference wrRows = null;
		
		/// <value>
		/// Reference to original rows
		/// </value>
		protected DataRowCollection rows {
			get { return ((DataRowCollection) wrRows.Target); }
		}
		
		/// <value>
		/// Returns row count
		/// </value>
		public override int Count { 
			get { return (rows.Count); }
		}
		
		/// <value>
		/// Returns DataRow at specified index
		/// </value>
		public T this [int index] { 
			get { return (rows[index] as T); }
		}
			
		/// <summary>
		/// Adds new row based on specified values
		/// </summary>
		/// <param name="values">
		/// Values used for row creation <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Reference to new DataRow <see cref="T"/>
		/// </returns>
		public T Add (params object[] values)
		{
			return (rows.Add(values) as T);
		}
		
		/// <summary>
		/// Adds new row
		/// </summary>
		/// <param name="row">
		/// Row <see cref="T"/>
		/// </param>
		public void Add (T row)
		{
			rows.Add (row);
		}
		
		/// <summary>
		/// Clears the row list
		/// </summary>
		public void Clear ()
		{
			rows.Clear();
		}
		
		/// <summary>
		/// Checks if row containing key exists
		/// </summary>
		/// <param name="key">
		/// Key <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if exists, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (object key)
		{
			return (Contains (key));
		}

		/// <summary>
		/// Checks if row containing keys exists
		/// </summary>
		/// <param name="keys">
		/// Keys <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if exists, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (object[] keys)
		{
			return (Contains (keys));
		}
		
		/// <summary>
		/// Copies Row collection to specified array
		/// </summary>
		/// <param name="array">
		/// Destination <see cref="Array"/>
		/// </param>
		/// <param name="index">
		/// Start index <see cref="System.Int32"/>
		/// </param>
		public override void CopyTo (Array array, int index)
		{
			rows.CopyTo (array, index);
		}
		
		/// <summary>
		/// Copies Row collection to specified array
		/// </summary>
		/// <param name="array">
		/// Destination <see cref="Array"/>
		/// </param>
		/// <param name="index">
		/// Start index <see cref="System.Int32"/>
		/// </param>
		public void CopyTo (T[] array, int index)
		{
			rows.CopyTo ((array as DataRow[]), index);
		}

		/// <summary>
		/// Searches for row containing key
		/// </summary>
		/// <param name="key">
		/// Searched key <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Reference if found, null if not <see cref="T"/>
		/// </returns>
		public T Find (object key)
		{
			return (Find (key) as T);
		}
		
		/// <summary>
		/// Searches for row containing keys
		/// </summary>
		/// <param name="key">
		/// Searched keys <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Reference if found, null if not <see cref="T"/>
		/// </returns>
		public DataRow Find (object[] keys)
		{
			return (Find (keys) as T);
		}
		
		/// <summary>
		/// Returns row enumerator
		/// </summary>
		/// <returns>
		/// Row enumerator <see cref="IEnumerator"/>
		/// </returns>
		public override IEnumerator GetEnumerator ()
		{
			return (rows.GetEnumerator());
		}
		
		/// <summary>
		/// Returns row enumerator
		/// </summary>
		/// <returns>
		/// Row enumerator <see cref="IEnumerator"/>
		/// </returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return ((IEnumerator<T>) rows.GetEnumerator());
		}
		
		/// <summary>
		/// Returns row position
		/// </summary>
		/// <param name="row">
		/// Searched row <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index of found row or -1 <see cref="System.Int32"/>
		/// </returns>
		public int IndexOf (T row)
		{
			return (rows.IndexOf (row));
		}
		
		/// <summary>
		/// Inserts row at specified index
		/// </summary>
		/// <param name="row">
		/// Row which will be inserted <see cref="T"/>
		/// </param>
		/// <param name="pos">
		/// Row position <see cref="System.Int32"/>
		/// </param>
		public void InsertAt (T row, int pos)
		{
			rows.InsertAt (row, pos);
		}
		
		/// <summary>
		/// Removes row from collection
		/// </summary>
		/// <param name="row">
		/// Row <see cref="T"/>
		/// </param>
		public void Remove (T row)
		{
			rows.Remove (row);
		}
		
		/// <summary>
		/// Removes row at specified index from collection
		/// </summary>
		/// <param name="index">
		/// Index of the row <see cref="System.Int32"/>
		/// </param>
		public void RemoveAt (int index)
		{
			rows.RemoveAt (index);
		}
		
		public TypedDataRowCollection (DataRowCollection aRows)
			: base ()
		{
			wrRows = new WeakReference (aRows);
		}
	}
}
