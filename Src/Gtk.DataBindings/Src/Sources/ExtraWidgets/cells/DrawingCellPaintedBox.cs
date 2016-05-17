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
	public class DrawingCellPaintedBox : DrawingCellBox
	{
		private DrawingCellFrame frame = new DrawingCellFrame();
		
		/// <value>
		/// Specifies background color
		/// </value>
		public Cairo.Color BackgroundColor {
			get { return (frame.BackgroundColor); }
			set {
				if (frame.BackgroundColor.IsEqualColor(value) == true)
					return;
				frame.BackgroundColor = value;
				OnPropertyChanged ("BackgroundColor");
			}
		}
		
		/// <value>
		/// Specifies background color
		/// </value>
		public Cairo.Color FrameColor {
			get { return (frame.FrameColor); }
			set {
				if (frame.FrameColor.IsEqualColor(value) == true)
					return;
				frame.FrameColor = value;
				OnPropertyChanged ("FrameColor");
			}
		}

		/// <value>
		/// Specifies if background should be painted or not
		/// </value>
		public bool BackgroundPainted {
			get { return (frame.PaintBackground); }
			set { 
				if (frame.PaintBackground == value)
					return;
				frame.PaintBackground = value;
				OnPropertyChanged ("BackgroundPainted");
			}
		}
		
		/// <value>
		/// Specifies if frame should be painted or not
		/// </value>
		public bool FramePainted {
			get { return (frame.PaintFrame); }
			set { 
				if (frame.PaintFrame == value)
					return;
				frame.PaintFrame = value;
				OnPropertyChanged ("FramePainted");
			}
		}
		
		/// <value>
		/// Specifies frame line width
		/// </value>
		public int FrameWidth {
			get { return (frame.BorderWidth); }
			set {
				if (frame.BorderWidth == value)
					return;
				frame.BorderWidth = value;
				OnPropertyChanged ("FrameWidth");
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
			if ((BackgroundPainted == true) || (FramePainted == true))
				frame.Paint (aArgs);
			base.Paint (aArgs);
		}
	}
}
