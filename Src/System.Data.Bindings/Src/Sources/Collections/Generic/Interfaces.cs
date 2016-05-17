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
	public delegate int CompareMethod<T> (T a, T b);

	public interface ITreeSortNode<T> : IDisconnectable
	{
		int InstanceCount { get; }
		T Value { get; }
		T this [int aIdx] { get; }
		void AddValue (T aValue);
		void RemoveValue (T aValue);		
		T[] GetInstances();
	}

	public interface IAVLTreeSortNode<T> : ITreeSortNode<T>
	{
		IAVLTreeSortNode<T> LowValueChild { get; set; }
		IAVLTreeSortNode<T> HighValueChild { get; set; }
		int Weight { get; set; }
		int LowWeight { get; }
		int HighWeight { get; }
		int NodeCount { get; }
		int Level { get; set; }
		bool IsSentinel { get; }
		bool InsertNode (SortEventArgs<T> aArgs, ref IAVLTreeSortNode<T> aNode, T value, out ITreeSortNode<T> aNewNode);
		bool Delete (SortEventArgs<T> aArgs, ref IAVLTreeSortNode<T> aNode, T value);
		string DebugNode (string aPrefix);
		IAVLTreeSortNode<T> CreateNode (T aValue, IAVLTreeSortNode<T> aSentinel);
	}

	public interface IAVLTreeSort<T> : IDisconnectable
	{
		IAVLTreeSortNode<T> Root { get; }
		IAVLTreeSortNode<T> Sentinel { get; }
		IAVLTreeSortNode<T> Deleted { get; set; }
		IAVLTreeSortNode<T> DeletedNode { get; set; }
		DuplicateHandlingType DuplicateHandling { get; }
		T Get (int aIdx);
		IAVLTreeSortNode<T> GetNodeFor (int aIdx);
		int IndexOf (T value);
		ITreeSortNode<T> Find (T aValue);		
		ITreeSortNode<T> Insert (T aValue);
		void Remove (T aValue);
		void Remove (T aValue, InstanceInformation aInstances);
		void Clear();
	}
}
