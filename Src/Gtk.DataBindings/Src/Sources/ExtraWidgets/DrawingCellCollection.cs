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
using System.Collections.Generic;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Provides collection of drawing cells
	/// </summary>	
	public class DrawingCellCollection : IEnumerable
	{
		private List<IDrawingCell> cells = new List<IDrawingCell>();

		/// <value>
		/// Returns cell count
		/// </value>
		public int Count {
			get { return (cells.Count); }
		}
		
		/// <value>
		/// Returns count of the visible items
		/// </value>
		public int VisibleCount {
			get {
				if (cells.Count == 0)
					return (0);
				int j=0;
				for (int i=0; i<Count; i++)
					if (this[i].Visible == true)
						j++;
				return (j);
			}
		}
		
		/// <value>
		/// Returns Cell at specified index
		/// </value>
		public IDrawingCell this [int aIndex] {
			get { 
				if ((aIndex < 0) || (aIndex >= Count))
					throw new IndexOutOfRangeException (string.Format ("Cell index {0} is out of range 0-{1}", aIndex, Count));
				return (cells[aIndex]);
			}
		}
		
		/// <summary>
		/// Packs new cell at end of the list
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		public void PackEnd (IDrawingCell aCell)
		{
			if ((aCell == null) || (cells.IndexOf(aCell) > -1))
				return;
			cells.Add (aCell);
		}
		
		/// <summary>
		/// Packs new cell at start of the list
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		public void PackStart (IDrawingCell aCell)
		{
			if (Count == 0) {
				PackEnd (aCell);
				return;
			}
			if ((aCell == null) || (cells.IndexOf(aCell) > -1))
				return;
			cells.Insert (0, aCell);
		}
		
		#region IEnumerable implementation
		
		/// <summary>
		/// Returns enumerator
		/// </summary>
		/// <returns>
		/// Enumerator <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return (cells.GetEnumerator());
		}
		
		/// <summary>
		/// Removes cell from collection
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		public void Remove (IDrawingCell aCell)
		{
			if (cells.IndexOf(aCell) < 0)
				return;
			cells.Remove (aCell);
		}
		
		#endregion
	}
}
