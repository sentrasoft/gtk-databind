//TODO Handle System.ComponentModel.IBindingList
//  - invokes event ListChangedEventHandler ListChanged
//
//			[Serializable]
//			public delegate void ListChangedEventHandler(
//			   object sender,
//			   ListChangedEventArgs e
//			);

using System;
using System.Data.Bindings;
using System.Data.Bindings.Cached;
using System.Data.Bindings.Collections;
using GLib;
using Gtk;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Defines query implementor specific to handle DataTable
	/// </summary>
	[ToDo ("Handle IBindingList too?")]
	public class IListTreeModel : QueryImplementor
	{
		/// <summary>
		/// Reference to IList
		/// </summary>
		public IList Items {
			get { 
				if (DataSource != null)
					if (DataSource is IList)
						return (DataSource as IList);
				return (null);
			}
		}

		#region QUERY_IMPLEMENTOR
		public override TreeModelFlags GetFlags()
		{
			if (RespectHierarchy == true)
				return (TreeModelFlags.ItersPersist);
			return (TreeModelFlags.ListOnly);
		}
		
		public override object GetNodeAtPath (TreePath aPath)
		{
			if (Items == null)
				return (null);
			if (aPath.Indices.Length == 0)
				return (null);
			if (RespectHierarchy == true)
				return (HierarchicalList.Get(Items, aPath.Indices));
			return (Items[aPath.Indices[0]]);
		}

		public override TreePath PathFromNode (object aNode)
		{
			TreePath tp = new TreePath();
			if ((aNode == null) || (Items == null) || (Items.Count == 0))
				return (tp);

			if (RespectHierarchy == false) {
				int i = Items.IndexOf(aNode);
				if (i > -1)
					tp.AppendIndex (i);
				return (tp);
			}
			int[] idx = HierarchicalList.IndexOf(Items, aNode);
			if (idx != null)
				foreach (int j in idx)
					tp.AppendIndex (j);
			return (tp);
		}

		public override bool IterNext (ref TreeIter aIter)
		{
			object node = NodeFromIter (aIter);
			if ((node == null) || (Items == null) || (Items.Count == 0))
				return (false);
			if (RespectHierarchy == false) {
				int idx = Items.IndexOf(node) + 1;
				if ((idx < 0) || (idx >= Items.Count))
					return (false);
				
				aIter = IterFromNode (Items[idx]);
				return (true);
			}
			else {
				int[] path = HierarchicalList.IndexOf(Items, node);
				if (path == null)
					return (false);
				path[path.Length-1] += 1;
				object o = HierarchicalList.Get (Items, path);
				if (o == null)
					return (false);
				aIter = IterFromNode (o);
				return (true);
			}
		}

		public override int ChildCount (object aNode)
		{
			if (RespectHierarchy == true)
				if (aNode is IList)
					return ((aNode as IList).Count);
			return (0);
		}

		public override bool IterChildren (out TreeIter aChild, TreeIter aParent)
		{
			aChild = TreeIter.Zero;
			if ((Items == null) || (Items.Count == 0))
			    return (false);
			if (aParent.UserData == IntPtr.Zero) {
				aChild = IterFromNode (Items[0]);
				return (true);
			}
			object node = NodeFromIter (aParent);
			if ((node == null) || (ChildCount(node) <= 0))
				return (false);
			if (RespectHierarchy == true)
				if (node is IList)
			    	if ((node as IList).Count > 0) {
						aChild = IterFromNode ((node as IList)[0]);
						return (true);
					}	                      
				
			return (false);
		}

		public override bool IterHasChild (TreeIter aIter)
		{
			if (RespectHierarchy == false)
				return (false);
			
			object node = NodeFromIter (aIter);
			if ((node == null) || (Items == null) || (Items.Count == 0) || (ChildCount(node) <= 0))
				return (false);

			return (true);
		}

		public override int IterNChildren (TreeIter aIter)
		{
			if ((Items == null) || (Items.Count == 0))
				return (0);
			if (aIter.UserData == IntPtr.Zero)
				return (Items.Count);
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

			if ((Items == null) || (Items.Count == 0))
				return (false);
			
			if (aParent.UserData == IntPtr.Zero) {
				if (Items.Count <= n)
					return (false);
				aChild = IterFromNode (Items[n]);
				return (true);
			}
			
			if (RespectHierarchy == true) {
				object node = NodeFromIter (aParent);
				if ((node == null) || (ChildCount(node) <= n))
					return (false);
/*				if (node is IList)
					if ((node as IList).Count < n) {
						aChild = IterFromNode (Items[n]);
						return (true);
					}*/
				if (node is IList) {
//					if ((node as IList).Count < n) {
					aChild = IterFromNode ((node as IList)[n]);
					return (true);
				}
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
				object o = HierarchicalList.Get(Items, tp.Indices);
				if (o != null) {
					aParent = IterFromNode(o);
					return (true);
				}
			}
			return (false);
		}
		#endregion QUERY_IMPLEMENTOR

		public IListTreeModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
			if (Items != null)
				if (TypeValidator.IsCompatible(Items.GetType(), typeof(DbObservableList)) == true)
					HasDeletedItems = true;
		}
	}
}
