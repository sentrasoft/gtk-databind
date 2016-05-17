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
	/// Specifies drawing cell which acts as container and draws like button
	/// </summary>
	public class DrawingCellEntry : DrawingCellThemedBin
	{
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public override void PaintBackground (CellExposeEventArgs aArgs)
		{
			Widget wdg = (Widget) Master;
			
			ResolveStyle();
			System.Console.WriteLine("ENTRY");
			System.Console.WriteLine(Master.GetType());
			System.Console.WriteLine(aArgs.Drawable.GetType());
			//aArgs.CellArea.Grow(1);
			Gdk.Rectangle rect = aArgs.CellArea.CopyToGdkRectangle();
			Gdk.Rectangle clip = aArgs.ClippingArea.CopyToGdkRectangle();
			if (aArgs.WidgetInRenderer == true) {
				System.Console.WriteLine("Paint entry flat");
				aArgs.Context.Color = Style.Foregrounds[(int) StateType.Selected].GetCairoColor();
				aArgs.CellArea.DrawPath (aArgs.Context);
				aArgs.Context.Fill();
			}
			else if (wdg.IsSensitive() == false) {
				Gtk.Style.PaintFlatBox (Style, aArgs.Drawable, StateType.Insensitive, 
				                       ShadowType.In, rect, wdg, "entry_bg", 
				                       rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow(Style, aArgs.Drawable, StateType.Insensitive, 
				                      ShadowType.In, rect, wdg, "entry", 
			    	                  rect.X, rect.Y, rect.Width, rect.Height);
			}
			else if (MasterIsFocused == true) {				
				Gtk.Style.PaintFlatBox (Style, aArgs.Drawable, State, 
				                       ShadowType.In, rect, wdg, "entry_bg", 
				                       rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintFocus (Style, aArgs.Drawable, State, clip, wdg, "entry", 
				                      rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow (Style, aArgs.Drawable, State, 
				                       ShadowType.In, rect, wdg, "entry", 
				                       rect.X, rect.Y, rect.Width, rect.Height);
			}
			else {
				Gtk.Style.PaintFlatBox (Style, aArgs.Drawable, State, 
				                       ShadowType.In, rect, wdg, "entry_bg", 
				                       rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow(Style, aArgs.Drawable, State, 
				                      ShadowType.In, rect, wdg, "entry", 
			    	                  rect.X, rect.Y, rect.Width, rect.Height);
			}
			System.Console.WriteLine("END ENTRY");
			//aArgs.CellArea.Grow(-1);
			FreeStyle();
		}

		/// <summary>
		/// Resolves style used for drawing this container
		/// </summary>
		/// <returns>
		/// Style <see cref="Style"/>
		/// </returns>
		protected override Style GetStyle ()
		{
			return (Rc.GetStyle (ChameleonTemplates.Entry));
		}
		
		public DrawingCellEntry()
			: base()
		{
		}
		
		public DrawingCellEntry (IDrawingCell aCell)
			: base (aCell)
		{
		}
	}
}
