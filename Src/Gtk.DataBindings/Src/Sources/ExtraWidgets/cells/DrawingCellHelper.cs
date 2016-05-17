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
	/// Provides static methods
	/// </summary>
	public static class DrawingCellHelper
	{
		/// <summary>
		/// Calculates container size
		/// </summary>
		/// <param name="aWidth">
		/// Requested width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Requested height <see cref="System.Double"/>
		/// </param>
		public static void HBoxGetSize (DrawingCellBox aBox, out double aWidth, out double aHeight)
		{
			aWidth = 0;
			aHeight = 0;
			if (aBox.VisibleCount == 0)
				return;
			double w,h,cw = w = h = 0;
			foreach (IDrawingCell dc in aBox.Cells) {
				if (dc.IsVisible == false)
					continue;
				dc.GetCellSize (out w, out h);
				if (cw < w)
					cw = w;
				aWidth += w;
				if (aHeight < h)
					aHeight = h;
			}
			if (aBox.VisibleCount > 1)
				if (aBox.Homogeneous == true)
					aWidth += ((aBox.VisibleCount*cw) + ((aBox.VisibleCount-1) * aBox.Spacing));
				else
					aWidth += ((aBox.VisibleCount-1) * aBox.Spacing);
//			aWidth = aWidth + (aBox.Padding * 2);
//			aHeight = aHeight + (aBox.Padding * 2);
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Bounding rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		public static void HBoxCalculateCellAreas (DrawingCellBox aBox, CellRectangle aRect)
		{
			CellRectangle rect = aRect.Copy();
			rect.Shrink (aBox.Padding);
			double start = rect.X;
			double end = System.Convert.ToInt32 (rect.Width+rect.X);
			double w,h = 0;
			double cellTop = rect.Y;
			double cellHeight = System.Convert.ToInt32 (rect.Height);

			if (aBox.Homogeneous == true) {
				double cw, ch = cw = 0;
				for (int i=0; i<aBox.Count; i++) {
					if (aBox.Cells[i].IsVisible == false) {
						aBox.Cells[i].Area.Set (0, 0, 0, 0);
						continue;
					}
					aBox.Cells[i].GetCellSize (out w, out h);
					if (w > cw)
						cw = w;
					if (h > ch)
						ch = h;
				}
				int j = 0;
				for (int i=0; i<aBox.Count; i++)
					if (aBox.Cells[i].IsVisible == true) {
						aBox.Cells[i].Area.Set (j*cw, cellTop, cw, cellHeight);
						j++;
					}
				return;			
			}
			
			IDrawingCell expandedCell = null;
			for (int i=0; i<aBox.Count; i++) {
				if (aBox.Cells[i].IsVisible == false) {
					aBox.Cells[i].Area.Set (0, 0, 0, 0);
					continue;
				}
				if (aBox.Cells[i].Expanded == true) {
					expandedCell = aBox.Cells[i];
					break;
				}
				aBox.Cells[i].GetCellSize (out w, out h);
				aBox.Cells[i].Area.Set (start, cellTop, w, cellHeight);
				start += w + aBox.Spacing;
			}
			if (expandedCell != null) {
				for (int i=aBox.Count-1; i>-1; i--) {
					if (aBox.Cells[i].IsVisible == false) {
						aBox.Cells[i].Area.Set (0, 0, 0, 0);
						continue;
					}
					if (aBox.Cells[i].Expanded == true)
						break;
					aBox.Cells[i].GetCellSize (out w, out h);
					aBox.Cells[i].Area.Set (end-w, cellTop, w, cellHeight);
					end -= w + aBox.Spacing;
				}
				if (expandedCell.IsVisible == true)
					expandedCell.Area.Set (start, cellTop, end-start, cellHeight);
				else
					expandedCell.Area.Set (0, 0, 0, 0);
			}
			aBox.RecalcChildren();
		}

		/// <summary>
		/// Calculates container size
		/// </summary>
		/// <param name="aWidth">
		/// Requested width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Requested height <see cref="System.Double"/>
		/// </param>
		public static void VBoxGetSize (DrawingCellBox aBox, out double aWidth, out double aHeight)
		{
			aWidth = 0;
			aHeight = 0;
			if (aBox.VisibleCount == 0)
				return;
			double w,h,ch = w = h = 0;
			foreach (IDrawingCell dc in aBox.Cells) {
				if (dc.IsVisible == false)
					continue;
				if (ch < h)
					ch = h;
				dc.GetCellSize (out w, out h);
				aHeight += h;
				if (aWidth < w)
					aWidth = w;
			}
			if (aBox.VisibleCount > 1)
				if (aBox.Homogeneous == true)
					aHeight += (aBox.VisibleCount*ch) + ((aBox.VisibleCount-1) * aBox.Spacing);
				else
					aHeight += (aBox.VisibleCount-1) * aBox.Spacing;
//			aWidth = aWidth + (Padding * 2);
//			aHeight = aHeight + (Padding * 2);
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Bounding rectangle <see cref="CellRectangle"/>
		/// </param>
		public static void VBoxCalculateCellAreas (DrawingCellBox aBox, CellRectangle aRect)
		{
			CellRectangle rect = aRect.Copy();
			rect.Shrink (aBox.Padding);
			double start = rect.Y;
			double end = System.Convert.ToInt32 (rect.Height+rect.Y);
			double w,h = 0;
			double cellLeft = rect.X;
			double cellWidth = System.Convert.ToInt32 (rect.Width);

			if (aBox.Homogeneous == true) {
				double cw, ch = cw = 0;
				for (int i=0; i<aBox.Count; i++) {
					if (aBox.Cells[i].IsVisible == false) {
						aBox.Cells[i].Area.Set (0, 0, 0, 0);
						continue;
					}
					aBox.Cells[i].GetCellSize (out w, out h);
					if (w > cw)
						cw = w;
					if (h > ch)
						ch = h;
				}
				int j = 0;
				for (int i=0; i<aBox.Count; i++)
					if (aBox.Cells[i].IsVisible == true) {
						aBox.Cells[i].Area.Set (cellLeft, j*ch, cellWidth, ch);
						j++;
					}
				return;			
			}
			
			IDrawingCell expandedCell = null;
			for (int i=0; i<aBox.Count; i++) {
				if (aBox.Cells[i].IsVisible == false) {
					aBox.Cells[i].Area.Set (0, 0, 0, 0);
					continue;
				}
				if (aBox.Cells[i].Expanded == true) {
					expandedCell = aBox.Cells[i];
					break;
				}
				aBox.Cells[i].GetCellSize (out w, out h);
				aBox.Cells[i].Area.Set (cellLeft, start, cellWidth, h);
				start += h + aBox.Spacing;
			}
			if (expandedCell != null) {
				for (int i=aBox.Count-1; i>-1; i--) {
					if (aBox.Cells[i].IsVisible == false) {
						aBox.Cells[i].Area.Set (0, 0, 0, 0);
						continue;
					}
					if (aBox.Cells[i].Expanded == true)
						break;
					aBox.Cells[i].GetCellSize (out w, out h);
					aBox.Cells[i].Area.Set (cellLeft, end-h, cellWidth, h);
					end -= h + aBox.Spacing;
				}
				if (expandedCell.IsVisible == true)
					expandedCell.Area.Set (cellLeft, start, cellWidth, end-start);
				else
					expandedCell.Area.Set (0, 0, 0, 0);
			}
			aBox.RecalcChildren();
		}

		/// <summary>
		/// Calculates container size
		/// </summary>
		/// <param name="aWidth">
		/// Requested width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Requested height <see cref="System.Double"/>
		/// </param>
		public static void BinGetSize (DrawingCellBox aBox, out double aWidth, out double aHeight)
		{
			aWidth = 0;
			aHeight = 0;
			if ((aBox.Count == 0) || (aBox.Cells[0].Visible == false))
				return;
			aBox.Cells[0].GetCellSize (out aWidth, out aHeight);
			aWidth += (aBox.Padding * 2);
			aHeight += (aBox.Padding * 2);
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Bounding rectangle <see cref="CellRectangle"/>
		/// </param>
		public static void BinCalculateCellAreas (DrawingCellBox aBox, CellRectangle aRect)
		{
			if (aBox.Count == 0)
				return;
			if (aBox.Cells[0].Visible == false) {
				aBox.Cells[0].Area.Set (0, 0, 0, 0);
				return;
			}
			aBox.Cells[0].Area.CopyFrom (aRect);
			aBox.Cells[0].Area.Shrink (aBox.Padding);
			aBox.RecalcChildren();
		}
		
		public static void GetCutSideBorderThickness (Style aStyle, SideCut aCut, out int aLeft, out int aTop, out int aRight, out int aBottom)
		{
			aLeft = aRight = aTop = aBottom = 0;
			if ((aCut & SideCut.Left) == SideCut.Left)
			    aLeft = aStyle.XThickness;
			if ((aCut & SideCut.Right) == SideCut.Right)
			    aRight = aStyle.XThickness;
			if ((aCut & SideCut.Top) == SideCut.Top)
			    aTop = aStyle.YThickness;
			if ((aCut & SideCut.Bottom) == SideCut.Bottom)
			    aBottom = aStyle.YThickness;
		}
		
		public static void GetBorderThickness (Style aStyle, SideCut aCut, out int aLeft, out int aTop, out int aRight, out int aBottom)
		{
			aLeft = aRight = aTop = aBottom = 0;
			if ((aCut & SideCut.Left) != SideCut.Left)
			    aLeft = aStyle.XThickness;
			if ((aCut & SideCut.Right) != SideCut.Right)
			    aRight = aStyle.XThickness;
			if ((aCut & SideCut.Top) != SideCut.Top)
			    aTop = aStyle.YThickness;
			if ((aCut & SideCut.Bottom) != SideCut.Bottom)
			    aBottom = aStyle.YThickness;
		}
		
		public static Cairo.Rectangle GetDrawAreaByStyle (Style aStyle, Cairo.Rectangle aArea, SideCut aCut)
		{
			return (ExpandAreaByStyle (aStyle, aArea, aCut, -1));
		}
		
		public static Cairo.Rectangle GetChildAreaByStyle (Style aStyle, Cairo.Rectangle aArea, SideCut aCut)
		{
			return (ExpandAreaByStyle (aStyle, aArea, aCut, 1));
		}
		
		private static Cairo.Rectangle ExpandAreaByStyle (Style aStyle, Cairo.Rectangle aArea, SideCut aCut, int aFactor)
		{
			double areaX = aArea.X;
			double areaY = aArea.Y; 
			double areaWidth = aArea.Width;
			double areaHeight = aArea.Height;

			int xThick = (aStyle.XThickness * aFactor);
			int yThick = (aStyle.YThickness * aFactor);
			
			if ((aCut & SideCut.Left) == SideCut.Left) {
				areaX -= xThick;
				areaWidth += xThick;
			}
			if ((aCut & SideCut.Right) == SideCut.Right)
				areaWidth +=xThick;
			if ((aCut & SideCut.Top) == SideCut.Top) {
				areaY -= yThick;
				areaHeight += yThick;
			}
			if ((aCut & SideCut.Bottom) == SideCut.Bottom)
				areaHeight += yThick;
			
			return (new Cairo.Rectangle (areaX, areaY, areaWidth, areaHeight));
		}
	}
}
