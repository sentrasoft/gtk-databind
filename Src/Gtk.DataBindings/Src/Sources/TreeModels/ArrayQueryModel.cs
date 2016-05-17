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

namespace Gtk.DataBindings
{
	/// <summary>
	/// Query model that handles arrays
	/// </summary>
	public class ArrayQueryModel : LinearListShellQueryModel
	{
		/// <summary>
		/// Gets object at specified index
		/// </summary>
		/// <param name="aIndex">
		/// Index <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object at index <see cref="System.Object"/>
		/// </returns>
		protected override object GetItemAtIndex (int aIndex)
		{
			if ((aIndex < 0) || (aIndex >= GetItemCount()))
			    return (null);
			if (DataSource != null) {
				if (DataSource.GetType().IsArray == true) {
					object[] ds = (object[]) DataSource;
					return (ds[aIndex]);
				}
			}
			return (null);
		}

		/// <summary>
		/// Returns count of items
		/// </summary>
		/// <returns>
		/// Item count <see cref="System.Int32"/>
		/// </returns>
		protected override int GetItemCount()
		{
			if (DataSource != null) {
				if (DataSource.GetType().IsArray == true) {
					object[] ds = (object[]) DataSource;
					return (ds.Length);
				}
			}
			return (0);
		}

		/// <summary>
		/// Searches array for specified object
		/// </summary>
		/// <param name="aNode">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Index of object <see cref="System.Int32"/>
		/// </returns>
		protected override int GetItemIndex (object aNode)
		{
			if (DataSource != null) {
				if (DataSource.GetType().IsArray == true) {
					object[] ds = (object[]) DataSource;
					for (int i=0; i<ds.Length; i++)
						if (ds[i] == aNode)
							return (i);
				}
			}
			return (-1);
		}
		
		public ArrayQueryModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
