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
	
	public class AVLTreeSortNode<T,Store> : IAVLTreeSortNode<T>
	{
		private const int SENTINEL_ID = -99999;
		
		private int level;
		public int Level {
			get { return (level); }
			set { 
				if (level == SENTINEL_ID)
					return;
				level = value; }
		}
		
		protected IAVLTreeSortNode<T> lowValueChild = null;
		public IAVLTreeSortNode<T> LowValueChild {
			get { return (lowValueChild); }
			set {
				if (lowValueChild  == value)
					return;
				lowValueChild = value;
			}
		}
	
		protected IAVLTreeSortNode<T> highValueChild = null;
		public IAVLTreeSortNode<T> HighValueChild {
			get { return (highValueChild); }
			set {
				if (highValueChild  == value)
					return;
				highValueChild = value;
			}
		}
	
		public int LowWeight {
			get { return (LowValueChild.Weight); }
		}
		
		public int HighWeight {
			get { return (HighValueChild.Weight); }
		}
		
		protected int weight = 0;
		public int Weight {
			get { 
				if (IsSentinel == true)
					return (0);
				if (weight == -1)
					weight = GetWeight();
				return (weight); 
			}
			set { weight = value; }
		}

		private int GetWeight()
		{
			if (IsSentinel == true)
				return (0);
			return (InstanceCount + LowWeight + HighWeight);
		}
		
		public bool IsSentinel {
			get { return ((Value == null) && (Level == SENTINEL_ID)); }
		}
		
		public virtual int InstanceCount {
			get { throw new NotImplementedException(); }
		}
		
		protected Store valueStore = default(Store);
		public Store ValueStore {
			get { return (valueStore); }
		}
		
		public virtual T Value {
			get { throw new NotImplementedException(); }
		}

		public virtual T this [int aIdx] {
			get { throw new NotImplementedException(); }
		}

		public virtual void AddValue (T aValue)
		{
			throw new NotImplementedException();
		}
		
		public virtual void RemoveValue (T aValue)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSingleInstance {
			get { return (typeof(Store) == typeof(T)); }
		}
		
		public int NodeCount {
			get {
				int res = 1;
				if ((LowValueChild != null) && (lowValueChild.IsSentinel == false))
					res += LowValueChild.NodeCount;
				if ((HighValueChild != null) && (highValueChild.IsSentinel == false))
					res += HighValueChild.NodeCount;
				return (res);
			}
		}
		
		public virtual T[] GetInstances()
		{
			throw new NotImplementedException();
		}
		
		private void Skew (ref IAVLTreeSortNode<T> aNode)
		{
			if (aNode.Level == aNode.HighValueChild.Level) {
				// rotate low
				
				IAVLTreeSortNode<T> high = aNode.HighValueChild;
				aNode.HighValueChild.Weight = -1;
				
				aNode.HighValueChild = high.LowValueChild;
				aNode.HighValueChild.Weight = -1;
				
				high.LowValueChild = aNode;
				high.LowValueChild.Weight = -1;
				
				aNode = high;
				aNode.Weight = -1;
			}
		}
	
		private void Split(ref IAVLTreeSortNode<T> aNode)
		{
			if (aNode.LowValueChild.LowValueChild.Level == aNode.Level) {
				// rotate high
				IAVLTreeSortNode<T> low = aNode.LowValueChild;
				aNode.LowValueChild.Weight = -1;
				
				aNode.LowValueChild = low.HighValueChild;
				aNode.LowValueChild.Weight = -1;
				
				low.HighValueChild = aNode;
				low.HighValueChild.Weight = -1;
				
				aNode = low;
				aNode.Weight = -1;
				
				aNode.Level++;
			}
		}
	
		public bool InsertNode (SortEventArgs<T> aArgs, ref IAVLTreeSortNode<T> aNode, T value, out ITreeSortNode<T> aNewNode)
		{
			aNewNode = null;
			if (aNode == aArgs.Tree.Sentinel) {
				aNode = CreateNode (value, aArgs.Tree.Sentinel);
				aNewNode = aNode;
				return (true);
			}
			
			int compare = -aArgs.OnCompare (value, aNode.Value);
			if (compare < 0) {
				if (InsertNode (aArgs, ref (aNode as AVLTreeSortNode<T,Store>).highValueChild, value, out aNewNode) == false)
					return (false);
				(aNode as AVLTreeSortNode<T,Store>).HighValueChild.Weight = -1;
			} 
			else if (compare > 0) {
				if (InsertNode (aArgs, ref (aNode as AVLTreeSortNode<T,Store>).lowValueChild, value, out aNewNode) == false)
					return (false);
				(aNode as AVLTreeSortNode<T,Store>).LowValueChild.Weight = -1;
			} 
			else {
				switch (aArgs.DuplicateHandling) {
				case DuplicateHandlingType.CollectDuplicates:
					aNode.AddValue (value);
					aNewNode = aNode;
					return (true);
				case DuplicateHandlingType.ThrowException:
					throw new DuplicateItemException();
				default:
					return (false);
				}	
				return (false);
			}

			Weight = -1;

			Skew (ref aNode);
			Split (ref aNode);
	
			return (true);
		}
	
		public bool Delete (SortEventArgs<T> aArgs, ref IAVLTreeSortNode<T> aNode, T value)
		{
			if (aNode == aArgs.Tree.Sentinel)
				return (aArgs.Tree.Deleted != null);
	
			int compare = -aArgs.OnCompare (value, aNode.Value);
			if (compare < 0) {
				if (Delete(aArgs, ref (aNode as AVLTreeSortNode<T,Store>).highValueChild, value) == false)
					return (false);
				(aNode as AVLTreeSortNode<T,Store>).HighValueChild.Weight = -1;
			} 
			else {
				if (compare == 0)
					aArgs.Tree.Deleted = aNode;
				if (Delete(aArgs, ref (aNode as AVLTreeSortNode<T,Store>).lowValueChild, value) == false)
					return (false);
				(aNode as AVLTreeSortNode<T,Store>).LowValueChild.Weight = -1;
			}
	
			if (aArgs.Tree.Deleted != null) {
				aArgs.Tree.DeletedNode = aArgs.Tree.Deleted;
				aArgs.Tree.Deleted = null;
				aNode = aNode.LowValueChild;
				aNode.Weight = -1;
				aNode.LowValueChild.Weight = -1;
			} 
			else if ((aNode.HighValueChild.Level < aNode.Level - 1) || (aNode.LowValueChild.Level < aNode.Level - 1)) {
				--aNode.Level;
				if (aNode.LowValueChild.Level > aNode.Level)
					aNode.LowValueChild.Level = aNode.Level;
				Skew(ref aNode);
				Skew(ref (aNode as AVLTreeSortNode<T,Store>).lowValueChild);
				Skew(ref (aNode.LowValueChild as AVLTreeSortNode<T,Store>).lowValueChild);
				Split(ref aNode);
				Split(ref (aNode as AVLTreeSortNode<T,Store>).highValueChild);
			}
	
			return (true);
		}
		
		public virtual IAVLTreeSortNode<T> CreateNode (T aValue, IAVLTreeSortNode<T> aSentinel)
		{
			throw new NotImplementedException();
		}
		
		public string DebugNode (string aPrefix)
		{
			if (IsSentinel == true)
				return (aPrefix + "[SENTINEL]");
			else
				return (string.Format (aPrefix + "{0} :{1}\n{2}\n{3}", Value, Weight,
				                       LowValueChild.DebugNode(aPrefix + "  "),
				                       HighValueChild.DebugNode(aPrefix + "  ")));
		}
		
		protected virtual void InitStore()
		{
		}
		
		public virtual void Disconnect()
		{
			lowValueChild = null;
			highValueChild = null;
		}
		
		// constuctor for the sentinel node
		internal AVLTreeSortNode()
		{
			level = SENTINEL_ID;
			lowValueChild = this;
			highValueChild = this;
			InitStore();
		}

		// constuctor for regular nodes (that all start life as leaf nodes)
		internal AVLTreeSortNode (T aValue, IAVLTreeSortNode<T> aSentinel)
		{
			level = 1;
			weight = -1;
			lowValueChild = aSentinel;
			highValueChild = aSentinel;
			InitStore();
			AddValue (aValue);
		}
		
		~AVLTreeSortNode()
		{
			Disconnect();
		}
	}
}
