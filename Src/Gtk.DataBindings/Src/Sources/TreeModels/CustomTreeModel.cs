using System;
using GLib;
using Gtk;
using System.Collections;
using System.Runtime.InteropServices;

namespace Gtk.DataBindings
{
	public class CustomTreeModel : GLib.Object, TreeModelImplementor 
	{
		public TreeModelFlags Flags {
			get { return (GetFlags()); }
		}

		public virtual TreeModelFlags GetFlags()
		{
			throw new System.NotImplementedException();
		}
		
		public int NColumns {
			get { return (GetNColumns()); }
		}

		public virtual int GetNColumns() 
		{
			throw new System.NotImplementedException();
		}
		
		public virtual object GetNodeAtPath (TreePath path)
		{
			throw new System.NotImplementedException();
		}

		public virtual GLib.GType GetColumnType (int aColumn)
		{
			throw new System.NotImplementedException();
		}

		public TreeIter IterFromNode (object aNode)
		{
			GCHandle gch = GCHandle.Alloc (aNode);
			TreeIter result = TreeIter.Zero;
			result.UserData = (IntPtr) gch;
			return (result);
		}

		private GCHandle NullHandle = GCHandle.Alloc(null);
		public object NodeFromIter (TreeIter aIter)
		{
			if ((int) aIter.UserData == 0)
				return (NullHandle);
//				return (GCHandle.Alloc(null));
			GCHandle gch = (GCHandle) aIter.UserData;
			return (gch.Target);
		}

		public virtual TreePath PathFromNode (object aNode)
		{
			throw new NotImplementedException();
		}

		public virtual bool GetIter (out TreeIter aIter, TreePath aPath)
		{
			if (aPath == null)
				throw new ArgumentNullException ("aPath");

			aIter = TreeIter.Zero;

			object node = GetNodeAtPath (aPath);
			if (node == null)
				return (false);

			aIter = IterFromNode (node);
			return (true);
		}

		public virtual TreePath GetPath (TreeIter aIter)
		{
			object node = NodeFromIter (aIter);
			if (node == null) 
				throw new ArgumentException ("aIter");

			return (PathFromNode (node));
		}


		public virtual void GetValue (TreeIter aIter, int aColumn, ref GLib.Value value)
		{
			throw new System.NotImplementedException();
		}

		public virtual void SetValue (TreeIter aIter, int aColumn, object value)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IterNext (ref TreeIter aIter)
		{
			throw new System.NotImplementedException();
		}

		public virtual int ChildCount (object aNode)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IterChildren (out TreeIter aChild, TreeIter aParent)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IterHasChild (TreeIter aIter)
		{
			if (Flags == TreeModelFlags.ListOnly)
				return (false);
			
			throw new System.NotImplementedException();
		}

		public virtual int IterNChildren (TreeIter aIter)
		{			
			throw new System.NotImplementedException();
		}

		public virtual bool IterNthChild (out TreeIter aChild, TreeIter aParent, int n)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IterParent (out TreeIter aParent, TreeIter aChild)
		{
			if (Flags == TreeModelFlags.ListOnly) {
				aParent = TreeIter.Zero;
				return (false);
			}
			throw new System.NotImplementedException();
		}

		public virtual void RefNode (TreeIter aIter)
		{
		}

		public virtual void UnrefNode (TreeIter aIter)
		{
		}

		public virtual void Disconnect()
		{
		}
		
		public CustomTreeModel ()
		{
		}
	}
}
