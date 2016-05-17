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

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides shell for implementation of linear list query models
	/// </summary>
	public class HierarchicalShellQueryModel : QueryImplementor
	{
		private TreeIter masterIter = TreeIter.Zero;

		#region Private_Methods

		/// <summary>
		/// Returns master item count
		/// </summary>
		/// <returns>
		/// Item count <see cref="System.Int32"/>
		/// </returns>
		private int GetMasterItemChildCount()
		{
			return (GetItemChildCount (DataSource));
		}
		
		/// <summary>
		/// Resolves item in master list at specified index, all checking
		/// </summary>
		/// <param name="aIndex">
		/// Index of node <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object at index or null <see cref="System.Object"/>
		/// </returns>
		private object GetMasterItemAtIndex (int aIndex)
		{
			int[] idx = new int[1] {aIndex};
			return (GetItemAtIndex (idx));
		}

		/// <summary>
		/// Resolves index of specified items, this method must be overriden in derivatives
		/// </summary>
		/// <param name="aNode">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		private int GetMasterItemIndex (object aNode)
		{
			int[] idx = GetItemIndex (aNode, false);
			if ((idx != null) && (idx.Length != 1))
				return (-1);
			return (idx[0]);
		}
		
		#endregion Private_Methods
		
		#region Methods_Needing_Override
		
		/// <summary>
		/// Resolves item at specified index, all checking
		/// </summary>
		/// <param name="aIndex">
		/// Index of node <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object at index or null <see cref="System.Object"/>
		/// </returns>
		protected virtual object GetItemAtIndex (int[] aIndex)
		{
			throw new NotImplementedException ("GetItemAtIndex must be implemented in derivatives\n" +
			                                   "  method should return item at specified index");
		}
		
		/// <summary>
		/// Resolves index of specified items, this method must be overriden in derivatives
		/// </summary>
		/// <param name="aNode">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <param name="aDeepSearch">
		/// Specifies if search should go deeper into hierarchy <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		protected virtual int[] GetItemIndex (object aNode, bool aDeepSearch)
		{
			throw new NotImplementedException ("GetItemIndex must be implemented in derivatives\n" +
			                                   "  method should return zero based position of specified node\n" +
			                                   "  or null if item doesn't exists");
		}
		
		/// <summary>
		/// Returns child count for specified item
		/// </summary>
		/// <param name="aNode">
		/// Item <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Number of child items <see cref="System.Int32"/>
		/// </returns>
		protected virtual int GetItemChildCount (object aNode)
		{
			throw new NotImplementedException ("GetItemChildCount must be implemented in derivatives\n" +
			                                   "  method should return number of items in specified item\n" +
			                                   "  or number of items in datasource if node specified was the\n" +
			                                   "  same as DataSource");
		}
		
		/// <summary>
		/// Resolves n-th item for specified node
		/// </summary>
		/// <param name="aNode">
		/// Node <see cref="System.Object"/>
		/// </param>
		/// <param name="aIndex">
		/// Index of node <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object at index or null <see cref="System.Object"/>
		/// </returns>
		protected virtual object GetItemChildAtIndex (object aNode, int aIndex)
		{
			throw new NotImplementedException ("GetItemAtIndex must be implemented in derivatives\n" +
			                                   "  method should return item at specified index");
		}
		
		#endregion Methods_Needing_Override

		#region Optional_Methods_To_Override
		
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
			
			return (false);
		}
		
		#endregion Optional_Methods_To_Override
		
		#region QUERY_IMPLEMENTOR

		public override TreeModelFlags GetFlags()
		{
			return (TreeModelFlags.ItersPersist);
		}
		
		public override object GetNodeAtPath (TreePath aPath)
		{
			if (DataSource == null)
				return (null);
			if (aPath.Indices.Length == 0)
				return (null);
			if (RespectHierarchy == true)
				return (GetItemAtIndex (aPath.Indices));
			return (GetMasterItemAtIndex (aPath.Indices[0]));
		}

		public override TreePath PathFromNode (object aNode)
		{
			TreePath tp = new TreePath();
			if ((aNode == null) || (DataSource == null) || (GetMasterItemChildCount() == 0))
				return (tp);

			if (RespectHierarchy == false) {
				int i = GetMasterItemIndex (aNode);
				if (i > -1)
					tp.AppendIndex (i);
				return (tp);
			}
			int[] idx = GetItemIndex (aNode, true);
			if (idx != null)
				foreach (int j in idx)
					tp.AppendIndex (j);
			return (tp);
		}

		public override bool IterNext (ref TreeIter aIter)
		{
			object node = NodeFromIter (aIter);
			if ((node == null) || (DataSource == null) || (GetMasterItemChildCount() == 0))
				return (false);
			if (RespectHierarchy == false) {
				int idx = GetMasterItemIndex (node) + 1;
				if ((idx < 0) || (idx >= GetMasterItemChildCount()))
					return (false);
				
				aIter = IterFromNode (GetMasterItemAtIndex (idx));
				return (true);
			}
			else {
				int[] path = GetItemIndex (node, true);
				if (path == null)
					return (false);
				path[path.Length-1] += 1;
				object o = GetItemAtIndex (path);
				if (o == null)
					return (false);
				aIter = IterFromNode (o);
				return (true);
			}
		}

		public override int ChildCount (object aNode)
		{
			if (RespectHierarchy == true)
				return (GetItemChildCount (aNode));
			return (0);
		}

		public override bool IterChildren (out TreeIter aChild, TreeIter aParent)
		{
			aChild = TreeIter.Zero;
			if ((DataSource == null) || (GetMasterItemChildCount() == 0))
			    return (false);
			if (aParent.UserData == IntPtr.Zero) {
				aChild = IterFromNode (GetMasterItemAtIndex (0));
				return (true);
			}
			object node = NodeFromIter (aParent);
			if ((node == null) || (ChildCount(node) <= 0))
				return (false);
			if (RespectHierarchy == true)
				if (GetItemChildCount(node) > 0) {
					aChild = IterFromNode (GetItemChildAtIndex(node, 0));
					return (true);
				}
			
			return (false);
		}

		public override bool IterHasChild (TreeIter aIter)
		{
			if (RespectHierarchy == false)
				return (false);
			
			object node = NodeFromIter (aIter);
			if ((node == null) || (DataSource == null) || (GetMasterItemChildCount() == 0) || (ChildCount(node) <= 0))
				return (false);

			return (true);
		}

		public override int IterNChildren (TreeIter aIter)
		{
			if ((DataSource == null) || (GetMasterItemChildCount() == 0))
				return (0);
			if (aIter.UserData == IntPtr.Zero)
				return (GetMasterItemChildCount());
			if (RespectHierarchy == false)
				return (0);
			object node = NodeFromIter (aIter);
			if (node == null)
				return (0);

			return (ChildCount (node));
		}

		public override bool IterNthChild (out TreeIter aChild, TreeIter aParent, int n)
		{
			aChild = TreeIter.Zero;

			if ((DataSource == null) || (GetMasterItemChildCount() == 0))
				return (false);
			
			if (aParent.UserData == IntPtr.Zero) {
				if (GetMasterItemChildCount() <= n)
					return (false);
				aChild = IterFromNode (GetMasterItemAtIndex (n));
				return (true);
			}
			
			if (RespectHierarchy == true) {
				object node = NodeFromIter (aParent);
				if ((node == null) || (ChildCount(node) <= n))
					return (false);
				aChild = IterFromNode (GetItemChildAtIndex (node, n));
				return (true);
			}
			return (false);
		}

		public override bool IterParent (out TreeIter aParent, TreeIter aChild)
		{
			aParent = TreeIter.Zero;
			object node = NodeFromIter (aChild);
			if (node == null)
				return (false);
			if (RespectHierarchy == true) {
				TreePath tp = PathFromNode (node);
				if ((tp.Indices == null) || (tp.Indices.Length <= 1))
					return (false);
				if (tp.Up() == false)
					return (false);
				object o = GetItemAtIndex (tp.Indices);
				if (o != null) {
					aParent = IterFromNode(o);
					return (true);
				}
			}
			return (false);
		}
		#endregion QUERY_IMPLEMENTOR

		public HierarchicalShellQueryModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
