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
	/// 
	/// </summary>
	public class DrawingCellArrow : DrawingCellContent
	{
		private ArrowType arrowType = ArrowType.Down;
		/// <value>
		/// Specifies arrow type
		/// </value>
		public ArrowType ArrowType {
			get { return (arrowType); }
			set { 
				if (arrowType == value)
					return;
				arrowType = value;
				OnPropertyChanged ("ArrowType");
			}
		}
		
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
			aWidth = ChameleonTemplates.Arrow.Requisition.Width;
			aHeight = ChameleonTemplates.Arrow.Requisition.Height;
//			System.Console.WriteLine("arrow:{0}x{1}", aWidth, aHeight);
		}
		
		public virtual void MaxArrowSize (out double x, out double y)
		{
			x = ChameleonTemplates.Arrow.Requisition.Width;
			y = ChameleonTemplates.Arrow.Requisition.Height;
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
/*			System.Console.WriteLine("Master: {0}", ((Gtk.Widget) Master).Allocation);
			System.Console.WriteLine("Owner: {0}", ((IDrawingCell) Owner).Area);
			System.Console.WriteLine("Arrow area: {0}", Area);
			System.Console.WriteLine("Cell area: {0}", aArgs.CellArea);*/
			object wdg = Master;
			if (wdg is Gtk.Widget) {
				Style style = Rc.GetStyle (ChameleonTemplates.Arrow);
				double x, y;
				MaxArrowSize (out x, out y);
				CellRectangle r = aArgs.CellArea.Copy();
				if (r.Height > r.Width)
					while (x<r.Width)
						r.Shrink(1);
				else
					while (y<r.Height)
						r.Shrink(1);
//				Gdk.Rectangle rect = aArgs.CellArea.CopyToGdkRectangle();
				Gdk.Rectangle rect = r.CopyToGdkRectangle();
//				System.Console.WriteLine(rect);
				Style.PaintArrow (style, aArgs.Drawable, this.ResolveState(), ShadowType.In, rect, 
			                      (wdg is Gtk.Widget) ? (Gtk.Widget) wdg : null, "arrow", ArrowType, true, rect.X, rect.Y, rect.Width, rect.Height);
				style.Dispose();
				r = null;
			}
			else {
//				System.Console.WriteLine("Arrow master={0}", Master.GetType());
			}
		}
	}
}
