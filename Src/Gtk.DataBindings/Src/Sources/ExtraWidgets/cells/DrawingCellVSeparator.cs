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
	/// Cell specifying vertical separator
	/// </summary>
	public class DrawingCellVSeparator : DrawingCellContent
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
			aWidth = ChameleonTemplates.VSeparator.Requisition.Width;
			aHeight = ChameleonTemplates.VSeparator.Requisition.Height;
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
			Style style = Rc.GetStyle (ChameleonTemplates.VSeparator);
			Widget wdg = (Widget) Master;
			Style.PaintVline (style, aArgs.Drawable, StateType.Normal, aArgs.CellArea.CopyToGdkRectangle(), 
			                  wdg, "hseparator", System.Convert.ToInt32(aArgs.CellArea.Y), 
			                  System.Convert.ToInt32(aArgs.CellArea.Y+aArgs.CellArea.Height), 
			                  System.Convert.ToInt32(aArgs.CellArea.X)+1);
			style.Dispose();
		}
	}
}
