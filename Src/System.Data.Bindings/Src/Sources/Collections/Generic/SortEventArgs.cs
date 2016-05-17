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
	public class SortEventArgs<T> : EventArgs, IDisconnectable
	{
		private event CompareMethod<T> method = null;
		
		private IAVLTreeSort<T> tree = null;
		public IAVLTreeSort<T> Tree {
			get { return (tree); }
		}

		private DuplicateHandlingType duplicateHandling = DuplicateHandlingType.CollectDuplicates;
		public DuplicateHandlingType DuplicateHandling {
			get { return (duplicateHandling); }
		}
		
		public int OnCompare (T a, T b)
		{
			return (method (a, b));
		}
		
		public void Disconnect()
		{
			tree = null;
			method = null;
		}
		
		private SortEventArgs()
		{
		}
		
		public SortEventArgs (IAVLTreeSort<T> aTree, CompareMethod<T> aMethod, DuplicateHandlingType aDuplicates)
		{
			tree = aTree;
			method = aMethod;
			duplicateHandling = aDuplicates;
		}
	}
}
