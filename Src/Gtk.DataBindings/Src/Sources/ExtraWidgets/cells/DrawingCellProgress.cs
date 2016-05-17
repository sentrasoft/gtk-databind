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
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class DrawingCellProgress : DrawingCellContent
	{		
		private double progress = 0.5;
		/// <value>
		/// Specifies progress
		/// </value>
		public double Progress {
			get { return (progress); }
			set {
				if (progress == value)
					return;
				progress = value;
				OnPropertyChanged ("Progress");
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
			aWidth = 100;
			aHeight = 15;
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		[ToDo ("Check why adding +2 is needed")]
		public override void Paint (CellExposeEventArgs aArgs)
		{
			if (Area.IsInsideArea(aArgs.ClippingArea) == false)
				return;
			Style style = Rc.GetStyle (ChameleonTemplates.Button);
			Widget wdg = (Widget) Master;
			Gdk.Rectangle rect = aArgs.CellArea.CopyToGdkRectangle();
			rect.Height += 2;
			if (this.ResolveState() == StateType.Insensitive) {
				Style.PaintBox (style, aArgs.Drawable, StateType.Insensitive, ShadowType.Out, rect, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				rect.Width = rect.Width-System.Convert.ToInt32(rect.Width*Progress);
				Style.PaintBox (style, aArgs.Drawable, StateType.Insensitive, ShadowType.Out, rect, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
			}
			else {
				Style.PaintBox (style, aArgs.Drawable, StateType.Normal, ShadowType.Out, rect, 
				                wdg, "box", rect.X, rect.Y, rect.Width, rect.Height);
				rect.Width = rect.Width-System.Convert.ToInt32(rect.Width*Progress);
				Style.PaintBox (style, aArgs.Drawable, StateType.Selected, ShadowType.Out, rect, 
				                wdg, "bar", rect.X, rect.Y, rect.Width, rect.Height);
			}
			style.Dispose();
		}
	}
}
