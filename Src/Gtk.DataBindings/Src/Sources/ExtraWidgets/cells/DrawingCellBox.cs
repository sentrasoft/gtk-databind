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
	/// Specifies container cell
	/// </summary>
	public class DrawingCellBox : DrawingCell
	{
		/// <summary>
		/// Checks if cell is visible or not
		/// </summary>
		public override bool IsVisible {
			get { return ((base.IsVisible == true) && (VisibleCount > 0)); }
		}
		
		private bool homogeneous = false;
		/// <value>
		/// Specifies that cell sizes are equal
		/// </value>
		public bool Homogeneous {
			get { return (homogeneous); }
			set { 
				if (homogeneous == value)
					return;
				homogeneous = value;
				OnPropertyChanged ("Homogeneous");
			}
		}
		
		private int spacing = 0;
		/// <value>
		/// Spacing between cells
		/// </value>
		public int Spacing {
			get { return (spacing); }
			set { spacing = value; }
		}

		/// <value>
		/// Cell count
		/// </value>
		public int Count {
			get { return (cells.Count); }
		}
		
		private DrawingCellCollection cells = new DrawingCellCollection();
		/// <value>
		/// Access to cells
		/// </value>
		public DrawingCellCollection Cells {
			get { return (cells); }
		}

		/// <value>
		/// Returns number of visible cells
		/// </value>
		public int VisibleCount {
			get {
				if (Count == 0)
					return (0);
				return (Cells.VisibleCount);
			}
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		protected virtual void PaintCells (CellExposeEventArgs aArgs)
		{
			for (int i=0; i<Count; i++) {
				if (Cells[i].IsVisible == false)
					continue;
				if (Cells[i].Area.IsInsideArea(aArgs.ClippingArea) == false)
					continue;
/*				if (aArgs.ExposeEvent != null) {
					Cairo.Rectangle rct = new Cairo.Rectangle (aArgs.ExposeEvent.Area.X, aArgs.ExposeEvent.Area.Y, aArgs.ExposeEvent.Area.Width, aArgs.ExposeEvent.Area.Height);
					aArgs.Context.Rectangle (rct);
					aArgs.Context.Clip();
				}
				aArgs.Context.Rectangle (Cells[i].Area);
				aArgs.Context.Clip();*/
//				Cairo.Rectangle origArea = aArgs.CellArea;
				double[] buff = aArgs.CellArea.Store();
				
//				aArgs.CellArea = Cells[i].Area;
				aArgs.CellArea.CopyFrom (Cells[i].Area);
				Cells[i].Paint (aArgs); //(evnt, aContext, aClippingArea, Cells[i].Area);
//				aArgs.CellArea = origArea;
				aArgs.CellArea.Restore (buff);
				buff = null;
//				aArgs.Context.ResetClip();
			}
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public virtual void PaintBackground (CellExposeEventArgs aArgs)
		{
		}

		private CellRectangle origClip = new CellRectangle();
		private CellRectangle origArea = new CellRectangle();
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
//			Cairo.Rectangle origClip = aArgs.ClippingArea;
//			Cairo.Rectangle origArea = aArgs.CellArea;
			
			origClip.CopyFrom(aArgs.ClippingArea);
			origArea.CopyFrom(aArgs.CellArea);
//			double[] origClip = aArgs.ClippingArea.Store();
//			double[] origArea = aArgs.CellArea.Store();
			
			// Paint background
//			Cairo.Rectangle fakeArea = CalculateBackgroundRect (aArgs.CellArea);
			CellRectangle fakeArea = CalculateBackgroundRect (aArgs.CellArea);
//			aArgs.Context.Rectangle (aArgs.CellArea);
//			aArgs.Context.Clip();
			aArgs.CellArea.Clip (aArgs.Context);
			if (aArgs.ExposeEvent != null) {
//				Cairo.Rectangle rct = new Cairo.Rectangle (aArgs.ExposeEvent.Area.X, aArgs.ExposeEvent.Area.Y, aArgs.ExposeEvent.Area.Width, aArgs.ExposeEvent.Area.Height);
				CellRectangle rct = new CellRectangle (aArgs.ExposeEvent.Area.X, aArgs.ExposeEvent.Area.Y, aArgs.ExposeEvent.Area.Width, aArgs.ExposeEvent.Area.Height);
//				aArgs.Context.Rectangle (rct);
//				aArgs.Context.Clip();
				rct.Clip (aArgs.Context);
				rct = null;
			}
			CellRectangle fa = aArgs.CellArea;
//			aArgs.CellArea = fakeArea;
			aArgs.CellArea = fakeArea;
			PaintBackground (aArgs); //.ExposeEvent, aContext, aArea, fakeArea);
			aArgs.CellArea = fa;
			aArgs.Context.ResetClip();
			
			// Paint cells
//			aArgs.CellArea = aArgs.CellArea.CopyAndShrink (Padding);
			aArgs.CellArea.Shrink (Padding);
			if (aArgs.NeedsRecalculation == true)
				if ((aArgs.ExposeEvent == null) || (TypeValidator.IsCompatible(Owner.GetType(), typeof(IDrawingCell)) == false)) {
					DoCalculateCellAreas (aArgs.CellArea);
					aArgs.ForceRecalculation = false;
				}
			if (aArgs.ForceRecalculation == true) {
				DoCalculateCellAreas (aArgs.CellArea);
				aArgs.ForceRecalculation = false;
			}
			PaintCells (aArgs);
			aArgs.CellArea.Shrink (-Padding);
			
			aArgs.ClippingArea.CopyFrom(origClip);
			aArgs.CellArea.CopyFrom(origArea);
//			aArgs.ClippingArea.Restore(origClip);
//			aArgs.CellArea.Restore(origArea);
		}

		internal override void ResetSize()
		{
			foreach (DrawingCell cell in Cells) {
				if ((cell.SizeWidth != -1) || (cell.SizeHeight != -1))
					cell.ResetSize();
			}
			base.ResetSize();
		}
		
		internal override void HierarchyChanged()
		{
			foreach (DrawingCell dc in Cells)
				dc.HierarchyChanged();
			base.HierarchyChanged();
		}
		
		/// <summary>
		/// Calculates background drawing rect, rectangle can reside out of clipping area
		/// </summary>
		/// <param name="aArea">
		/// Area <see cref="CellRectangle"/>
		/// </param>
		/// <returns>
		/// Result rectangle <see cref="CellRectangle"/>
		/// </returns>
		public virtual CellRectangle CalculateBackgroundRect (CellRectangle aArea)
		{
			return (aArea);
		}
		
		/// <summary>
		/// Forces recalc of boxed children
		/// </summary>
		public void RecalcChildren()
		{
//			foreach (IDrawingCell cell in Cells)
			for (int i=0; i<Count; i++)// IDrawingCell cell in Cells)
				if (TypeValidator.IsCompatible(Cells[i].GetType(), typeof(DrawingCellBox)) == true)
					(Cells[i] as DrawingCellBox).DoCalculateCellAreas ((Cells[i] as DrawingCellBox).Area);
		}
		
		protected virtual CellRectangle GetChildArea()
		{
			return (new CellRectangle (Area.X+Padding, Area.Y+Padding, Area.Width-(Padding*2), Area.Height-(Padding*2)));
		}
		
		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Area occupied by widget <see cref="CellRectangle"/>
		/// </param>
		public virtual void DoCalculateCellAreas (CellRectangle aRect)
		{
			Area.CopyFrom(aRect);
			CellRectangle childrect = GetChildArea();
			CalculateCellAreas (childrect);
			childrect = null;
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Area occupied by widget <see cref="CellRectangle"/>
		/// </param>
		protected virtual void CalculateCellAreas (CellRectangle aRect)
		{
			throw new NotImplementedException ("CalculateCellAreas needs to be implemented");
		}

		/// <summary>
		/// Packs cell into container
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <remarks>
		/// if cell already exists, exception is thrown
		/// </remarks>
		public virtual void Pack (IDrawingCell aCell)
		{
			if (aCell == null)
				return;
			if (Count > 0)
				throw new NotSupportedException ("Pack method only supports one cell");
			PackStart (aCell, true);
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(DrawingCellContent)) == true) {
				(aCell as DrawingCellContent).XPos = 0.5;
				(aCell as DrawingCellContent).YPos = 0.5;
			}
		}
		
		/// <summary>
		/// Packs cell as first
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public virtual void PackStart (IDrawingCell aCell, bool aExpanded)
		{
			if (aCell == null)
				return;
			if (aExpanded == true)
				foreach (IDrawingCell dc in cells)
					if (dc.Expanded == true)
						throw new NotSupportedException ("Two expanded cells are not supported");
			cells.PackStart (aCell);
			aCell.Expanded = aExpanded;		
			aCell.Owner = this;
		}
		
		/// <summary>
		/// Packs cell as last
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public virtual void PackEnd (IDrawingCell aCell, bool aExpanded)
		{
			if (aCell == null)
				return;
			if (aExpanded == true)
				foreach (IDrawingCell dc in cells)
					if (dc.Expanded == true)
						throw new NotSupportedException ("Two expanded cells are not supported");
			cells.PackEnd (aCell);
			aCell.Expanded = aExpanded;		
			aCell.Owner = this;
		}

		/// <summary>
		/// Removes cell from collection
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		public virtual void Remove (IDrawingCell aCell)
		{
			cells.Remove (aCell);
		}
		
		/// <summary>
		/// Returns cell which takes place on specified coordinates
		/// </summary>
		/// <param name="aX">
		/// X <see cref="System.Double"/>
		/// </param>
		/// <param name="aY">
		/// Y <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// Cell at specified coordinates <see cref="IDrawingCell"/>
		/// </returns>
		public override IDrawingCell CellAtCoordinates (double aX, double aY)
		{
			IDrawingCell c = base.CellAtCoordinates (aX, aY);
			if (c == null)
				return (null);
			foreach (IDrawingCell cell in Cells) {
				if (cell.IsVisible == false)
					continue;
				c = cell.CellAtCoordinates(aX, aY);
				if (c != null)
					return (c);
			}
			return (null);
		}
	}
}
