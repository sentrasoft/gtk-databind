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

namespace System.Data.Bindings.Collections.Generic
{
	
	public class AVLTreeSort<T> : IEnumerable
	{
		private IAVLTreeSort<T> tree = null;
		
		private event CompareMethod<T> sortMethod = null;
		
		public int Count {
			get {
				if (Root == null)
					return (0);
				return (tree.Root.Weight);
			}
		}
		
		protected IAVLTreeSortNode<T> Root {
			get { return (tree.Root); }
		}

		public T this [int aIdx] {
			get { return (tree.Get (aIdx)); }
		}

		private DuplicateHandlingType duplicateHandling = DuplicateHandlingType.CollectDuplicates;
		public DuplicateHandlingType DuplicateHandling {
			get { return (duplicateHandling); }
		}

		public virtual IEnumerator GetEnumerator()
		{
			lock (this) {
				for (int i=0; i<Count; i++)
					yield return (this[i]);
			}
		}
		
		public virtual IEnumerable Distinct()
		{
			IAVLTreeSortNode<T> curr;
			IAVLTreeSortNode<T> last = null;
			lock (this) {
				for (int i=0; i<Count; i++) {
					curr = tree.GetNodeFor (i);
					if ((curr != last) && (curr != null)) {
						yield return (curr.Value);
					}
					last = curr;
				}
			}
		}
		
		public ITreeSortNode<T> Insert (T aValue)
		{
			lock (this) {
				return (tree.Insert (aValue));
			}
		}
		
		/// <summary>
		/// Returns index of specified item
		/// </summary>
		/// <param name="value">
		/// Searched object <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index or -1 if not found <see cref="System.Int32"/>
		/// </returns>
		public int IndexOf (T value)
		{
			lock (this) {
				return (tree.IndexOf (value));
			}
		}
		
		private T[] _GetKeyInstances (T aValue)
		{
			ITreeSortNode<T> n = tree.Find (aValue);
			if (n == null)
				return (null);
			return (n.GetInstances());
		}
		
		public T[] GetKeyInstances (T aValue)
		{
			return (_GetKeyInstances (aValue));
		}
		
		public void RemoveKey (T aValue)
		{
			Remove (aValue, InstanceInformation.AllInstances);
		}

		public void Remove (T aValue)
		{
			Remove (aValue, InstanceInformation.ObjectInstance);
		}
		
		public void Remove (T aValue, InstanceInformation aInstances)
		{
			lock (this) {
				tree.Remove (aValue, aInstances);
				if (tree.DeletedNode != null) {
					tree.DeletedNode.Disconnect();
					tree.DeletedNode = null;
				}
			}
		}
		
		public bool Contains (T aValue)
		{
			lock (this) {
				T[] res = _GetKeyInstances (aValue);
				if ((res != null) && (res.Length > 0)) {
					bool found = false;
					for (int i=0; i<res.Length; i++) {
						if (res[i].Equals(aValue))
							found = true;
						res[i] = default(T);
					}
					res = null;
					return (found);
				}
				return (false);
			}
		}
		
		public bool ContainsKey (T aValue)
		{
			lock (this) {
				T[] res = _GetKeyInstances (aValue);
				if ((res != null) && (res.Length > 0)) {
					for (int i=0; i<res.Length; i++)
						res[i] = default(T);
					res = null;
					return (true);
				}
				return (false);
			}
		}

		public string Debug (string aPreffix)
		{
			lock (this) {
				return (tree.Root.DebugNode (aPreffix));
			}
		}

		public void Clear()
		{
			lock (this) {
				tree.Clear();
			}
		}
		
		public void Disconnect()
		{
			tree.Disconnect();
			tree = null;
		}
		
		private AVLTreeSort()
		{
		}
		
		public AVLTreeSort (CompareMethod<T> aSortMethod, DuplicateHandlingType aDuplicateHandling)
		{
			sortMethod = aSortMethod;
			duplicateHandling = aDuplicateHandling;
			switch (DuplicateHandling) {
			case DuplicateHandlingType.CollectDuplicates:
				tree = new BaseAVLTreeSort<T,T[]> (aSortMethod, aDuplicateHandling);
				break;
			case DuplicateHandlingType.ThrowException:
			case DuplicateHandlingType.Drop:
				tree = new BaseAVLTreeSort<T,T> (aSortMethod, aDuplicateHandling);
				break;
			}
		}
		
		~AVLTreeSort()
		{
			tree.Disconnect();
		}
	}
}
