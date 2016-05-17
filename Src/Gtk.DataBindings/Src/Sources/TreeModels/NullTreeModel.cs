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
	/// Model which should be specified on null datasources
	/// </summary>
	public class NullTreeModel : QueryImplementor
	{
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
			return (null);
		}

		/// <summary>
		/// Searches for node in Items
		/// </summary>
		public override TreePath PathFromNode (object aNode)
		{
			TreePath tp = new TreePath();
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
			return (false);
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

		private NullTreeModel()
			: base (null)
		{
		}

		public NullTreeModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
