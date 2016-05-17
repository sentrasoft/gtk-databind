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

namespace System.Data.Bindings.Collections.Generic
{
	
	public class BaseAVLTreeSort<T,Store> : IAVLTreeSort<T>
	{
		private IAVLTreeSortNode<T> sentinel;
		public IAVLTreeSortNode<T> Sentinel {
			get { return (sentinel); }
		}
		
		private IAVLTreeSortNode<T> deleted;
		public IAVLTreeSortNode<T> Deleted {
			get { return (deleted); }
			set { deleted = value; }
		}
		
		private IAVLTreeSortNode<T> deletedNode;
		public IAVLTreeSortNode<T> DeletedNode {
			get { return (deletedNode); }
			set { deletedNode = value; }
		}
		
		private SortEventArgs<T> args = null;

		private int nodeCount = 0;
		public int NodeCount {
			get { return (nodeCount); }
			set { nodeCount = value; }
		}
		
		public int Count {
			get {
				if (Root == null)
					return (0);
				return (Root.Weight);
			}
		}
		
		private IAVLTreeSortNode<T> root = null;
		public IAVLTreeSortNode<T> Root {
			get { return ((IAVLTreeSortNode<T>) root); }
		}

		private event CompareMethod<T> sortMethod = null;
		
		public int OnSortMethod (T a, T b)
		{
			foreach (CompareMethod<T> method in sortMethod.GetInvocationList()) {
				int compare = method (a, b);
				if (compare != 0)
					return (compare);
			}
			return (0);
		}
		
		private DuplicateHandlingType duplicateHandling = DuplicateHandlingType.CollectDuplicates;
		public DuplicateHandlingType DuplicateHandling {
			get { return (duplicateHandling); }
		}
		
		protected SortEventArgs<T> CreateSortArgs()
		{
			return (new SortEventArgs<T> (this, sortMethod, DuplicateHandling));
		}

		public ITreeSortNode<T> Insert (T aValue)
		{
			if (root == null) {
				if (DuplicateHandling == DuplicateHandlingType.CollectDuplicates)
					root = new MultiInstanceAVLTreeSortNode<T> (aValue, Sentinel);
				else
					root = new SingleInstanceAVLTreeSortNode<T> (aValue, Sentinel);
				NodeCount++;
				return (root);
			}
			else {
				ITreeSortNode<T> newNode = null;
				if (Root.InsertNode (args, ref root, aValue, out newNode) == true)
					return (newNode);
				return (null);
			}
		}
		
		public void Remove (T aValue)
		{
			Remove (aValue, InstanceInformation.AllInstances);
		}
		
		public void Remove (T aValue, InstanceInformation aInstances)
		{
			lock (this) {
				Deleted = null;
				if (((DuplicateHandling == DuplicateHandlingType.CollectDuplicates) &&
				     (aInstances != InstanceInformation.AllInstances)) ||
				    (aInstances == InstanceInformation.ObjectInstance)) {
					IAVLTreeSortNode<T> t = FindAndMark (aValue);
					if (t == null)
						return;
					if (t.InstanceCount > 1)
						t.RemoveValue (aValue);
					else
						Remove (aValue, InstanceInformation.AllInstances);
					return;
				}
				Root.Delete (args, ref root, aValue);
			}
		}
		
		public T Get (int aIdx)
		{
			if ((aIdx < 0) || (aIdx >= Count))
				throw new IndexOutOfRangeException();
			return (Get (Root, aIdx+1));
		}
		
		private T Get (IAVLTreeSortNode<T> aNode, int aIdx)
		{
			if (aIdx <= aNode.LowWeight)
				return (Get (aNode.LowValueChild, aIdx));
			else if (aIdx > (aNode.LowWeight + aNode.InstanceCount))
				return (Get (aNode.HighValueChild, aIdx-(aNode.LowWeight+aNode.InstanceCount)));
			else
				return (aNode[aIdx-aNode.LowWeight-1]);
		}
		
		public IAVLTreeSortNode<T> GetNodeFor (int aIdx)
		{
			if ((aIdx < 0) || (aIdx >= Count))
				throw new IndexOutOfRangeException();
			return (GetNodeFor (Root, aIdx+1));
		}

		private IAVLTreeSortNode<T> GetNodeFor (IAVLTreeSortNode<T> aNode, int aIdx)
		{
			if (aIdx <= aNode.LowWeight)
				return (GetNodeFor (aNode.LowValueChild, aIdx));
			else if (aIdx > (aNode.LowWeight + aNode.InstanceCount))
				return (GetNodeFor (aNode.HighValueChild, aIdx-(aNode.LowWeight+aNode.InstanceCount)));
			else
				return (aNode);
		}
		
		public void Clear()
		{
			while ((Root != null) && (Root.IsSentinel == false))
				Remove (Root.Value, InstanceInformation.AllInstances);
		}
		
		public ITreeSortNode<T> Find (T aValue)
		{
			return (Find (Root, aValue));			
		}
		
		private ITreeSortNode<T> Find (IAVLTreeSortNode<T> aNode, T aValue)
		{
			if (aNode.IsSentinel == true)
				return (null);
			int compare = OnSortMethod (aNode.Value, aValue);
			if (compare > 0)
				return (Find (aNode.LowValueChild, aValue));
			else if (compare < 0)
				return (Find (aNode.HighValueChild, aValue));
			else
				return (aNode);
		}
		
		public IAVLTreeSortNode<T> FindAndMark (T aValue)
		{
			return (FindAndMark (Root, aValue));			
		}
		
		private IAVLTreeSortNode<T> FindAndMark (IAVLTreeSortNode<T> aNode, T aValue)
		{
			if (aNode.IsSentinel == true)
				return (null);
			int compare = OnSortMethod (aNode.Value, aValue);
			IAVLTreeSortNode<T> res = null;
			if (compare > 0)
				res = FindAndMark (aNode.LowValueChild, aValue);
			else if (compare < 0)
				res = FindAndMark (aNode.HighValueChild, aValue);
			else {
				aNode.Weight = -1;
				return (aNode);
			}
			if (res != null)
				aNode.Weight = -1;
			return (res);
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
			return (IndexOf (Root, value, 0));
		}
		
		private int IndexOf (IAVLTreeSortNode<T> aNode, T aValue, int aIdx)
		{
			if (aNode.IsSentinel == true)
				return (-1);
			int compare = OnSortMethod (aNode.Value, aValue);
			if (compare > 0)
				return (IndexOf (aNode.LowValueChild, aValue, aIdx));
			else if (compare < 0)
				return (IndexOf (aNode.HighValueChild, aValue, aIdx + aNode.InstanceCount + aNode.LowWeight));
			else {
				for (int i=0; i<aNode.InstanceCount; i++)
					if (aNode[i].Equals(aValue) == true)
						return (aIdx + i);
				return (-1);
			}
		}
		
		public void Disconnect()
		{
			Clear();
		}
		
		private BaseAVLTreeSort()
		{
		}
		
		internal BaseAVLTreeSort (CompareMethod<T> aSortMethod, DuplicateHandlingType aDuplicateHandling)
		{
			sortMethod = aSortMethod;
			duplicateHandling = aDuplicateHandling;
			args = CreateSortArgs();
			if (DuplicateHandling == DuplicateHandlingType.CollectDuplicates)
				sentinel = new MultiInstanceAVLTreeSortNode<T>();
			else
				sentinel = new SingleInstanceAVLTreeSortNode<T>();
			//sentinel = new Node<T>();
			root = sentinel;
			deleted = null;
		}

		~BaseAVLTreeSort()
		{
			Disconnect();
		}
	}
}
