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

using Cairo;
using Gdk;
using Gtk;
using System;
using System.ComponentModel;
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies cell which draws pixbuf
	/// </summary>
	public class DrawingCellText : DrawingCellContent
	{
		double lastW, lastH = -1;
		string lastStr = "";
		
		private string text = null;
		/// <value>
		/// Text which specifies Pango markup language
		/// </value>
		public string Text {
			get { return (text); }
			set {
				if (text == value)
					return;
				double w,h,nw,nh = 0;
				GetSize (out w, out h);
				text = value;
				GetSize (out nw, out nh);
				if ((w != nw) || (h != nh))
					OnPropertyChanged ("Size");
				text = value;
				OnPropertyChanged ("Text");
				if ((sizeText != null) || (sizeText != ""))
					ResetSize();
			}
		}

		/// <value>
		/// Display value of text
		/// </value>
		public virtual string DisplayText {
			get { return (Text); }
		}
		
		private string sizeText = "";
		/// <value>
		/// Text used to measure size of the cell
		/// </value>
		public string SizeText {
			get { 
				if ((sizeText == null) || (sizeText == ""))
					return (DisplayText);
				return (sizeText);
			}
			set {
				if (sizeText == value)
					return;
				sizeText = value;
				OnPropertyChanged ("Size");
				OnPropertyChanged ("Text");
				ResetSize();
			}
		}
		
		private int xSize, ySize;
		
		#region IDrawingCell_Implementation

		protected Pango.Layout SetLayout()
		{
			Pango.Layout layout = Layout;
			layout.Wrap = Pango.WrapMode.Word;
			layout.Alignment = Pango.Alignment.Left;
			layout.FontDescription = FontDescription;
			return (layout);
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
			aWidth = 0;
			aHeight = 0;
			Pango.Context pContext = PangoContext;
			if (((SizeText == "") && (text == null) || (text == "")) || (pContext == null))
				return;
			if (lastStr == SizeText)
				if ((lastH != -1) && (lastW != -1)) {
					aWidth = lastW;
					aHeight = lastH;
					return;
				}
				
			Pango.Layout layout = SetLayout();
			int w,h=0;
			layout.SetMarkup ("Mj");
			Pango.Rectangle r1;
			Pango.Rectangle r2;
			layout.GetPixelExtents (out r1, out r2);
			xSize = r1.Width;
			h = ySize = r1.Height;
			w = 0;
			
			if ((SizeText != null) && (SizeText != "")) {
				layout.SetMarkup (SizeText);
				layout.GetPixelSize (out w, out h);
			}
			aWidth = w;
			aHeight = h;
			layout.SetMarkup ("");
			FreeLayout (layout);
		}

		private void FreeLayout (Pango.Layout aLayout)
		{
//			if (aLayout == null)
//				return;
//			aLayout.Context.Dispose();
//			aLayout.FontDescription.Dispose();
//			aLayout.Dispose();
//			PangoContext.Dispose();
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
			Pango.Context pContext = PangoContext;
			if ((DisplayText == "") || (pContext == null))
				return;
			Pango.Layout layout = SetLayout();
			layout.Width = Pango.Units.FromPixels (System.Convert.ToInt32 (aArgs.CellArea.Width));

			layout.SetMarkup (DisplayText);
			int width, height;
			layout.GetPixelSize (out width, out height);
			double xdiff = Area.Width - width;
			double ydiff = Area.Height - ySize;
			aArgs.Context.Color = Color;
			aArgs.Context.MoveTo (aArgs.CellArea.X+(xdiff*XPos), aArgs.CellArea.Y+(ydiff*YPos));
			
			Pango.CairoHelper.ShowLayout (aArgs.Context, layout);
			layout.SetMarkup ("");
			FreeLayout (layout);
		}

		#endregion IDrawingCell_Implementation
		
		public DrawingCellText()
		{
			XPos = 0.0;
			YPos = 0.5;
		}
		
		public DrawingCellText (string aText)
			: this()
		{
			text = aText;
		}
		
		public DrawingCellText (string aText, string aSizeText)
			: this (aText)
		{
			sizeText = aSizeText;
		}
		
		~DrawingCellText()
		{
		}
	}
}
