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
	public class DrawingCellContent : DrawingCell
	{
		private Cairo.Color color = new Cairo.Color (0, 0, 0);
		public virtual Cairo.Color Color {
			get { return (color); }
			set {
				if (color.Equals(value) == true)
					return;
				color = value;
				OnPropertyChanged ("Color");
			}
		}
		
		private double xPos = 0;
		public double XPos {
			get { return (xPos); }
			set {
				if (xPos == value)
					return;
				xPos = value;
				OnPropertyChanged ("XPos");
			}
		}
		
		private double yPos = 0;
		public double YPos {
			get { return (yPos); }
			set {
				if (yPos == value)
					return;
				yPos = value;
				OnPropertyChanged ("YPos");
			}
		}

//		public override void Paint (Cairo.Context aContext, Cairo.Rectangle aArea)
//		{
//			if (FrameVisible == true)
//				frame.Paint (aContext, aArea);
//		}

//		public override void GetSize (out int aWidth, out int aHeight)
//		{
//			if (BorderVisible == true)
//				aWidth = aHeight = 4;
//		}

		public DrawingCellContent()
		{
//			FrameVisible = false;
		}
	}
}
