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
	/// Cell specifying horizontal separator
	/// </summary>
	public class DrawingCellHSeparator : DrawingCellContent
	{
		/// <summary>
		/// Calculates size needed for this cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public override void GetSize (out double aWidth, out double aHeight)
		{
			aWidth = ChameleonTemplates.HSeparator.Requisition.Width;
			aHeight = ChameleonTemplates.HSeparator.Requisition.Height;
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public override void Paint (CellExposeEventArgs aArgs)
		{
			if (Area.IsInsideArea(aArgs.ClippingArea) == false)
				return;
			Style style = Rc.GetStyle (ChameleonTemplates.HSeparator);

			Widget wdg = (Widget) Master;
			Style.PaintHline (style, aArgs.Drawable, StateType.Normal, aArgs.CellArea.CopyToGdkRectangle(), 
			                  wdg, "vseparator", System.Convert.ToInt32(aArgs.CellArea.X), 
			                  System.Convert.ToInt32(aArgs.CellArea.X+aArgs.CellArea.Width), 
			                  System.Convert.ToInt32(aArgs.CellArea.Y));
			style.Dispose();
		}
	}
}
