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
	
	public class MultiInstanceAVLTreeSortNode<T> : AVLTreeSortNode<T, T[]>
	{
		public override T Value {
			get { 
				if (valueStore == null)
					return (default(T));
				return (valueStore[0]); 
			}
		}

		public override T this [int aIdx] {
			get { 
				if ((aIdx < 0) || (aIdx >= InstanceCount))
					throw new IndexOutOfRangeException();
				return (ValueStore[aIdx]);
			}
		}

		public override int InstanceCount {
			get {
				if (IsSentinel == true)
					return (0);
				if (valueStore == null)
					return (0);
				return (valueStore.Length);
			}
		}

		public override void AddValue (T aValue)
		{
			bool just = false;
			if (valueStore == null) {
				valueStore = new T[1];
				just = true;
			}
			if (((InstanceCount+1) > valueStore.Length) && (just == false)){
				T[] newValues = new T[InstanceCount+1];
				valueStore.CopyTo (newValues, 0);
				newValues[InstanceCount] = aValue;
				for (int i=0; i<valueStore.Length; i++)
					valueStore[i] = default(T);
				valueStore = newValues;
			}
			else 
				valueStore[InstanceCount-1] = aValue;
			weight++;
		}
		
		public override void RemoveValue (T aValue)
		{
			int j=-1;
			for (int i=0; i<valueStore.Length; i++) {
				if (valueStore[i].Equals(aValue) == true) {
					j = i;
					valueStore[i] = default(T);
					weight = -1;
					break;
				}
			}
			if (j != -1) {
				if (valueStore.Length == 1) {
					valueStore[0] = default(T);
					valueStore = null;
				}
				T[] newValues = new T[valueStore.Length-1];
				for (int i=0; i<j; i++)
					newValues[i] = valueStore[i];
				for (int i=j; i<newValues.Length-1; i++)
					newValues[i] = valueStore[i+1];
				T[] tmp = valueStore;
				valueStore = newValues;
				for (int i=0; i<tmp.Length; i++)
					tmp[i] = default(T);
				tmp = null;
			}
		}
		
		public override T[] GetInstances()
		{
			T[] res = new T[InstanceCount];
			for (int i=0; i<InstanceCount; i++)
				res[i] = valueStore[i];
			return (res);
		}
		
		public override IAVLTreeSortNode<T> CreateNode (T aValue, IAVLTreeSortNode<T> aSentinel)
		{
			return (new MultiInstanceAVLTreeSortNode<T> (aValue, aSentinel));
		}

		protected override void InitStore ()
		{
			base.InitStore ();
			valueStore = null;
		}

		public override void Disconnect ()
		{
			for (int i=0; i<InstanceCount; i++)
				valueStore[i] = default(T);
			valueStore = null;
			base.Disconnect ();
		}

		internal MultiInstanceAVLTreeSortNode ()
			: base ()
		{
		}
		
		internal MultiInstanceAVLTreeSortNode (T aValue, IAVLTreeSortNode<T> aSentinel)
			: base (aValue, aSentinel)
		{
		}
		
		~MultiInstanceAVLTreeSortNode()
		{
			if (valueStore == null)
				return;
			for (int i=0; i<valueStore.Length; i++)
				valueStore[i] = default(T);
			valueStore = null;
		}
	}
}
