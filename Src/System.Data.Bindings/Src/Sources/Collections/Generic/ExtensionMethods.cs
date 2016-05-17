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
using System.Collections.Generic;

namespace System.Data.Bindings.Collections.Generic
{
	/// <summary>
	/// Provides extension methods to generic list classes
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Provides easy string representation of list
		/// </summary>
		/// <param name="aList">
		/// List <see cref="List"/>
		/// </param>
		/// <param name="aDelimiter">
		/// Delimiter <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		public static string AsDelimited<T> (this List<T> aList, string aDelimiter)
		{
			List<string> strings = new List<string>();
			foreach (T data in aList)
				strings.Add (data.ToString());
			string res = string.Join (aDelimiter, strings.ToArray());
			strings.Clear();
			strings = null;
			return (res);
		}

		/// <summary>
		/// Safely inserts value. If index is out of range, then Add is called
		/// </summary>
		/// <param name="aList">
		/// List <see cref="List"/>
		/// </param>
		/// <param name="aAtIndex">
		/// Index where insertion should go <see cref="System.Int32"/>
		/// </param>
		/// <param name="value">
		/// Value <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index of new item <see cref="System.Int32"/>
		/// </returns>
		public static int SafeInsert<T> (this List<T> aList, int aAtIndex, T value)
		{
			if ((aAtIndex < 0) || (aAtIndex >= aList.Count)) {
				aList.Add (value);
				return (aList.Count);
			}
			aList.Insert (aAtIndex, value);
			return (aAtIndex);
		}
		
		/// <summary>
		/// Adds element into list by sorting
		/// </summary>
		/// <param name="aList">
		/// List of elements <see cref="List"/>
		/// </param>
		/// <param name="aData">
		/// Data to be inserted <see cref="T"/>
		/// </param>
		/// <returns>
		/// Index where data was inserted <see cref="System.Int32"/>
		/// </returns>
		/// <remarks>
		/// This method is threating list as sorted
		/// </remarks>
		public static int SortedAdd<T> (this List<T> aList, T aData) where T: IComparable
		{
			if ((aList.Count == 0) || ((aList[aList.Count-1] as IComparable).CompareTo(aData) <= 0)) {
				aList.Add (aData);
				return (aList.Count-1);
			}
			for (int i=0; i<aList.Count; i++)
				if ((aList[i] as IComparable).CompareTo(aData) >= 0) {
					aList.Insert (i, aData);
					return (i);
				}
			return (-1);
		}
	}
}
