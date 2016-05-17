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
	public class DrawingCellGdkColorFrame : DrawingCellContent
	{
		private DrawingCellFrame frame = new DrawingCellFrame();

		public bool FrameVisible {
			get { return ((BorderVisible == true) || (FillVisible == true)); }
			set {
				BorderVisible = value;
				FillVisible = value; 
			}
		}
		
		public bool BorderVisible {
			get { return (frame.Frame == true); }
			set { 
				if (frame.Frame == value)
					return;
				frame.PaintFrame = value; 
				OnPropertyChanged ("BorderVisible");
			}
		}

		public Cairo.Color BorderColor {
			get { return (frame.FrameColor); }
			set {
				if (frame.FrameColor.Equals(value) == true)
					return;
				frame.FrameColor = value;
				OnPropertyChanged ("BorderColor");
			}
		}
		
		public bool FillVisible {
			get { return (frame.PaintBackground == true); }
			set { 
				if (frame.PaintBackground == value)
					return;
				frame.PaintBackground = value; 
				OnPropertyChanged ("BorderVisible");
			}
		}

		public Cairo.Color FillColor {
			get { return (frame.BackgroundColor); }
			set {
				if (frame.BackgroundColor.Equals(value) == true)
					return;
				frame.BackgroundColor = value;
				OnPropertyChanged ("FillColor");
			}
		}

		public int BorderWidth {
			get { return (frame.BorderWidth); }
			set {
				if (frame.BorderWidth == value)
					return;
				frame.BorderWidth = value;
				OnPropertyChanged ("BorderWidth");
			}
		}
		
		public override Cairo.Color Color {
			get { return (FillColor); }
			set { 
				if (Color.Equals(value) == true)
					return;
				FillColor = value; 
				OnPropertyChanged ("Color");
			}
		}
		
		public Gdk.Color GdkColor {
			get { 
				return (new Gdk.Color (System.Convert.ToByte(Color.R*255),
				                       System.Convert.ToByte(Color.G*255),
				                       System.Convert.ToByte(Color.B*255)));
			}
			set {
				Color = new Cairo.Color (System.Convert.ToDouble(value.Red/255.0),
				                         System.Convert.ToDouble(value.Green/255.0),
				                         System.Convert.ToDouble(value.Blue/255.0));
				OnPropertyChanged ("GdkColor");
			}
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
//			CellRectangle r = aArgs.CellArea.Copy;
			aArgs.CellArea.Shrink (1);
				//= aArgs.CellArea.CopyAndGrow (-3);
			frame.Area.CopyFrom (aArgs.CellArea);
			frame.Paint (aArgs);
//			aArgs.CellArea = r;
			aArgs.CellArea.Grow (1);
		}

		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public override void GetSize (out double aWidth, out double aHeight)
		{
			aWidth = 15;
			aHeight = 15;
		}

		public DrawingCellGdkColorFrame()
		{
			BorderWidth = 1;
			FillVisible = true;
			BorderVisible = true;
		}

		public DrawingCellGdkColorFrame (Gdk.Color aColor)
			: this()
		{
			GdkColor = aColor;
		}

		public DrawingCellGdkColorFrame (Cairo.Color aColor)
			: this()
		{
			Color = aColor;
		}
	}
}
