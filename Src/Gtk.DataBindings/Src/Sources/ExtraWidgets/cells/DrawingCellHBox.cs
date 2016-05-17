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
	/// Specifies horizontally packed cell container
	/// </summary>
	public class DrawingCellHBox : DrawingCellPaintedBox
	{
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
			DrawingCellHelper.HBoxGetSize (this, out aWidth, out aHeight);
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Bounding rectangle <see cref="CellRectangle"/>
		/// </param>
		protected override void CalculateCellAreas (CellRectangle aRect)
		{
			DrawingCellHelper.HBoxCalculateCellAreas (this, aRect);
		}
	}
}
