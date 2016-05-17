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
	/// Provides shell for implementing query models for linear lists
	/// </summary>	
	/// <remarks>
	/// The only things one has to implement is overrides for methods
	///       protected virtual object GetItemAtIndex (int aIndex)
	///       protected virtual int GetItemIndex (object aNode)
	///       protected virtual int GetDataSourceItemCount()
	/// and optional method
	///       public override bool IsItemDeleted (object aObject)
	/// and query model is already handled correctly
	/// </remarks>
	public class LinearListShellQueryModel : QueryImplementor
	{
		private TreeIter masterIter = TreeIter.Zero;

		#region Methods_Needing_Override
		
		/// <summary>
		/// Returns master item count
		/// </summary>
		/// <returns>
		/// Item count <see cref="System.Int32"/>
		/// </returns>
		protected virtual int GetItemCount()
		{
			throw new NotImplementedException ("GetItemCount must be implemented in derivatives\n" +
			                                   "  method should return number of items in specified datasource");
		}
		
		/// <summary>
		/// Resolves item at specified index, all checking
		/// </summary>
		/// <param name="aIndex">
		/// Index of node <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object at index or null <see cref="System.Object"/>
		/// </returns>
		protected virtual object GetItemAtIndex (int aIndex)
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
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		protected virtual int GetItemIndex (object aNode)
		{
			throw new NotImplementedException ("GetItemIndex must be implemented in derivatives\n" +
			                                   "  method should return zero based position of specified node\n" +
			                                   "  or -1 if item doesn't exists");
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
			if (DataSource == null)
				return (null);
			if (aPath.Indices.Length == 0)
				return (null);
			return (GetItemAtIndex (aPath.Indices[0]));
		}

		/// <summary>
		/// Searches for node in Items
		/// </summary>
		public override TreePath PathFromNode (object aNode)
		{
			TreePath tp = new TreePath();
			if ((aNode == null) || (DataSource == null) || (GetItemCount() == 0))
				return (tp);

			int idx = GetItemIndex (aNode);
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
			if ((node == null) || (DataSource == null) || (GetItemCount() == 0))
				return (false);
			int idx = GetItemIndex (node);
			if ((idx < 0) || ((idx+1) >= GetItemCount()))
				return (false);
			aIter = IterFromNode (GetItemAtIndex (idx+1));
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
			if ((DataSource == null) || (GetItemCount() == 0))
			    return (false);
			if (aParent.UserData == IntPtr.Zero) {
				aChild = IterFromNode (GetItemAtIndex (0));
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
			if ((DataSource == null) || (GetItemCount() == 0))
				return (0);
			if (aIter.UserData == IntPtr.Zero)
				return (GetItemCount());
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

			if ((DataSource == null) || (GetItemCount() == 0))
				return (false);
			
			if (aParent.UserData == IntPtr.Zero) {
				if (GetItemCount() <= n)
					return (false);
				aChild = IterFromNode (GetItemAtIndex (n));
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
		
		#endregion QUERY_IMPLEMENTOR

		/// <summary>
		/// Disconnects references
		/// </summary>
		public override void Disconnect ()
		{
			base.Disconnect ();
		}

		public LinearListShellQueryModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
