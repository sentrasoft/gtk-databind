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
	
	public class SingleInstanceAVLTreeSortNode<T> : AVLTreeSortNode<T, T>
	{
		public override T Value {
			get { return (valueStore); }
		}

		public override T this [int aIdx] {
			get { 
				if (aIdx != 0)
					throw new IndexOutOfRangeException();
				return (valueStore);
			}
		}		
		
		public override int InstanceCount {
			get { 
				if (IsSentinel == true)
					return (0);
				return (1); 
			}
		}
		
		public override void AddValue (T aValue)
		{
			if (InstanceCount > 0)
				throw new Exception ("Instances > 1???");
			valueStore = aValue;
			weight++;
		}
		
		public virtual void RemoveValue (T aValue)
		{
			valueStore = default(T);
			weight--;
		}
		
		public override T[] GetInstances()
		{
			T[] res = new T[1];
			res[0] = Value;
			return (res);
		}
		
		public override IAVLTreeSortNode<T> CreateNode (T aValue, IAVLTreeSortNode<T> aSentinel)
		{
			return (new SingleInstanceAVLTreeSortNode<T> (aValue, aSentinel));
		}
		
		public override void Disconnect()
		{
			valueStore = default(T);
			base.Disconnect();
		}
		
		internal SingleInstanceAVLTreeSortNode ()
			: base ()
		{
		}
		
		internal SingleInstanceAVLTreeSortNode (T aValue, IAVLTreeSortNode<T> aSentinel)
			: base (aValue, aSentinel)
		{
		}
	}
}
