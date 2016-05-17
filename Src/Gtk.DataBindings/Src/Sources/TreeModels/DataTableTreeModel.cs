using System;
using System.Data;
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Defines query implementor specific to handle DataTable
	/// </summary>
	[QueryModel (typeof(System.Data.DataTable))]
	public class DataTableTreeModel : QueryImplementor
	{
		private System.Data.DataTable items = null;
		/// <summary>
		/// Reference to DataTable
		/// </summary>
		public System.Data.DataTable Items {
			get { 
				if (items == null)
					if (DataSource != null)
						if (TypeValidator.IsCompatible(DataSource.GetType(), typeof(System.Data.DataTable)) == true)
							items = DataSource as System.Data.DataTable;
				return (items);
			}
		}

		#region QUERY_IMPLEMENTOR
		/// <summary>
		/// Returns flags, in this case ListOnly
		/// </summary>
		public override TreeModelFlags GetFlags()
		{
			return (TreeModelFlags.ListOnly);
		}
		
		/// <summary>
		/// Returns node at specified path
		/// </summary>
		public override object GetNodeAtPath (TreePath aPath)
		{
			if (Items == null)
				return (null);
			if (aPath.Indices.Length == 0)
				return (null);
			return (Items.Rows[aPath.Indices[0]]);
		}

		/// <summary>
		/// Searches for node in Items
		/// </summary>
		public override TreePath PathFromNode (object aNode)
		{
			TreePath tp = new TreePath();
			if ((aNode == null) || (Items == null) || (Items.Rows.Count == 0))
				return (tp);

			int idx = Items.Rows.IndexOf(aNode as DataRow);
			if (idx >= 0)
				tp.AppendIndex (idx);
			return (tp);
		}

		/// <summary>
		/// Returns next iter
		/// </summary>
		/// <param name="aIter">
		/// Iterator <see cref="TreeIter"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public override bool IterNext (ref TreeIter aIter)
		{
			object node = NodeFromIter (aIter);
			if ((node == null) || (Items == null) || (Items.Rows.Count == 0))
				return (false);
			int idx = Items.Rows.IndexOf(node as DataRow);
			if ((idx < 0) || ((idx+1) >= Items.Rows.Count))
				return (false);
			aIter = IterFromNode (Items.Rows[idx+1]);
			return (true);
		}

		/// <summary>
		/// Returns child count from node
		/// </summary>
		/// <param name="aNode">
		/// Node <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Child count <see cref="System.Int32"/>
		/// </returns>
		public override int ChildCount (object aNode)
		{
			return (0);
		}

		/// <summary>
		/// Returns first iterator in specified parent
		/// </summary>
		/// <param name="aChild">
		/// Child iterator <see cref="TreeIter"/>
		/// </param>
		/// <param name="aParent">
		/// Parent <see cref="TreeIter"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool IterChildren (out TreeIter aChild, TreeIter aParent)
		{
			aChild = TreeIter.Zero;
			if ((Items == null) || (Items.Rows.Count == 0))
			    return (false);
			if (aParent.UserData == IntPtr.Zero) {
				aChild = IterFromNode (Items.Rows[0]);
				return (true);
			}				
			return (false);
		}

		/// <summary>
		/// Checks if iterator has child or not
		/// </summary>
		/// <param name="aIter">
		/// Iterator <see cref="TreeIter"/>
		/// </param>
		/// <returns>
		/// true if children are present, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool IterHasChild (TreeIter aIter)
		{
			return (false);
		}

		/// <summary>
		/// Returns child count from iterator
		/// </summary>
		/// <param name="aIter">
		/// Iterator <see cref="TreeIter"/>
		/// </param>
		/// <returns>
		/// Child count <see cref="System.Int32"/>
		/// </returns>
		public override int IterNChildren (TreeIter aIter)
		{
			if ((Items == null) || (Items.Rows.Count == 0))
				return (0);
			if (aIter.UserData == IntPtr.Zero)
				return (Items.Rows.Count);
			return (0);
		}

		/// <summary>
		/// Returns n-th child for specified parent
		/// </summary>
		/// <param name="aChild">
		/// N-th child node <see cref="TreeIter"/>
		/// </param>
		/// <param name="aParent">
		/// Parent node <see cref="TreeIter"/>
		/// </param>
		/// <param name="n">
		/// Index of specifie node <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool IterNthChild (out TreeIter aChild, TreeIter aParent, int n)
		{
			aChild = TreeIter.Zero;

			if ((Items == null) || (Items.Rows.Count == 0))
				return (false);
			
			if (aParent.UserData == IntPtr.Zero) {
				if (Items.Rows.Count <= n)
					return (false);
				aChild = IterFromNode (Items.Rows[n]);
				return (true);
			}
			return (false);
		}

		/// <summary>
		/// Returns parent for iterator
		/// </summary>
		/// <param name="aParent">
		/// Parent node <see cref="TreeIter"/>
		/// </param>
		/// <param name="aChild">
		/// Iterator <see cref="TreeIter"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool IterParent (out TreeIter aParent, TreeIter aChild)
		{
			aParent = TreeIter.Zero;
			return (false);
		}

		/// <summary>
		/// Checks if item is deleted, classes like data table support rows
		/// wit state=deleted
		/// </summary>
		/// <param name="aObject">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if deleted, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool IsItemDeleted (object aObject)
		{
			if (aObject == null)
				return (true);
			
			return ((aObject as DataRow).RowState == DataRowState.Deleted);
		}
		
		#endregion QUERY_IMPLEMENTOR

		/// <summary>
		/// Disconnects references
		/// </summary>
		public override void Disconnect ()
		{
			items = null;
			base.Disconnect ();
		}

		public DataTableTreeModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
			HasDeletedItems = true;
		}
	}
}
