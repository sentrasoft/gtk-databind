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

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies drawing cell which contains one single cell
	/// </summary>
	public class DrawingCellBin : DrawingCellBox
	{
		/// <value>
		/// Cell inside this container
		/// </value>
		public IDrawingCell Cell {
			get { 
				if (Count == 0)
					return (null);
				return (Cells[0]);
			}
		}
		
		/// <summary>
		/// Packs cell as first
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public override void PackStart (IDrawingCell aCell, bool aExpanded)
		{
			if (Count > 0)
				throw new NotSupportedException ("DrawingCellBin only supports one cell");
			base.PackStart (aCell, aExpanded);
		}
		
		/// <summary>
		/// Packs cell as last
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public override void PackEnd (IDrawingCell aCell, bool aExpanded)
		{
			if (Count > 0)
				throw new NotSupportedException ("DrawingCellBin only supports one cell");
			base.PackEnd (aCell, aExpanded);
		}
		
		/// <summary>
		/// Calculates container size
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public override void GetSize (out double aWidth, out double aHeight)
		{
			DrawingCellHelper.BinGetSize (this, out aWidth, out aHeight);
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Bounding rectangle <see cref="CellRectangle"/>
		/// </param>
		protected override void CalculateCellAreas (CellRectangle aRect)
		{
			DrawingCellHelper.BinCalculateCellAreas (this, aRect);
		}
		
		public DrawingCellBin()
			: base()
		{
		}
		
		public DrawingCellBin (IDrawingCell aCell)
			: base()
		{
			Pack (aCell);
		}
	}
}
