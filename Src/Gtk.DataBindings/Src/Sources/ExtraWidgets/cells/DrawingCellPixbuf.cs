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
using System;
using System.ComponentModel;
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies cell which draws pixbuf
	/// </summary>
	public class DrawingCellPixbuf : DrawingCellContent
	{
		protected Pixbuf pixbuf = null;
		/// <value>
		/// Pixbuf
		/// </value>
		public virtual Pixbuf Pixbuf {
			get { return (pixbuf); }
			set {
				if (pixbuf == value)
					return;
				double w,h,nw,nh = 0;
				GetSize (out w, out h);
				pixbuf = value;
				GetSize (out nw, out nh);
				if ((w != nw) || (h != nh))
					OnPropertyChanged ("Size");					
				OnPropertyChanged ("Pixbuf");
				ResetSize();
			}
		}

		/// <summary>
		/// Loads pixbuf from resource
		/// </summary>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		public void SetFromResource (string aName)
		{
			Pixbuf = (this as IDrawingCell).LoadPixbufFromResourceStore (aName);
		}
		
		/// <summary>
		/// Set pixbuf from file
		/// </summary>
		/// <param name="aFile">
		/// File name <see cref="System.String"/>
		/// </param>
		public void SetFromFile (string aFile)
		{
			Pixbuf = new Pixbuf (aFile);
		}
		
		#region IDrawingCell_Implementation
		
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
			if (pixbuf == null) 
				return;
			aWidth = pixbuf.Width;
			aHeight = pixbuf.Height;
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
			if (pixbuf == null)
				return;
			double w,h = 0;
			GetSize (out w, out h);
			double xdiff = Area.Width - w;
			double ydiff = Area.Height - h;
			Gdk.CairoHelper.SetSourcePixbuf (aArgs.Context, Pixbuf, aArgs.CellArea.X+(xdiff*XPos), aArgs.CellArea.Y+(ydiff*YPos));
			aArgs.Context.Paint(); 
		}

		#endregion IDrawingCell_Implementation
		
		public DrawingCellPixbuf()
		{
			XPos = 0.0;
			YPos = 0.5;
		}
		
		public DrawingCellPixbuf (Pixbuf aPixbuf)
			: this()
		{
			Pixbuf = aPixbuf;
		}
		
		public DrawingCellPixbuf (string aName)
			: this()
		{
			try {
				SetFromResource (aName);
			}
			catch {
				SetFromFile (aName);
			}
		}
	}
}
