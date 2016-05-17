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
using Cairo;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class DrawingCellFrame : DrawingCell
	{
		private int borderWidth = 1;
		public int BorderWidth {
			get { return (borderWidth); }
			set { borderWidth = value; }
		}
		
		private int spacing = 0;
		public int Spacing {
			get { return (spacing); }
			set { spacing = value; }
		}
		
		private bool framed = false;
		public bool Frame {
			get { return (framed); }
			set { framed = value; }
		}

		private bool paintFrame = false;
		public bool PaintFrame {
			get { return (paintFrame); }
			set { paintFrame = value; }
		}
		
		private Cairo.Color frameColor = new Cairo.Color (0, 0, 0);
		public Cairo.Color FrameColor {
			get { return (frameColor); }
			set { frameColor = value; }
		}

		private bool paintBackground = false;
		public bool PaintBackground {
			get { return (paintBackground); }
			set { paintBackground = value; }
		}
		
		private Cairo.Color backgroundColor = new Cairo.Color (1, 1, 1);
		public Cairo.Color BackgroundColor {
			get { return (backgroundColor); }
			set { backgroundColor = value; }
		}

		/// <summary>
		/// Paints border on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public virtual void PaintBorder (CellExposeEventArgs aArgs)
		{
			CellRectangle r = aArgs.CellArea.Copy();
			if (PaintFrame == true)
				r.Grow(-1);
			r.DrawPath (aArgs.Context);
//			aArgs.Context.Rectangle (r);
			aArgs.Context.Color = FrameColor;
			aArgs.Context.LineWidth = BorderWidth;
			if (PaintFrame == true) {
				if (PaintBackground == true)
					aArgs.Context.StrokePreserve();
				else
					aArgs.Context.Stroke();
			}
			aArgs.Context.Color = BackgroundColor;
			if (PaintBackground == true)
				aArgs.Context.Fill();
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
			PaintBorder (aArgs);
		}

		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public override void GetSize (out double aWidth, out double aHeight)
		{
			aWidth = (BorderWidth * 2) + (Spacing * 2);
			aHeight = (BorderWidth * 2) + (Spacing * 2);
		}

		public DrawingCellFrame()
		{
		}

		public DrawingCellFrame (bool aPaintFrame, bool aPaintBackground)
		{
			paintBackground = aPaintBackground;
			paintFrame = aPaintFrame;
		}
	}
}
