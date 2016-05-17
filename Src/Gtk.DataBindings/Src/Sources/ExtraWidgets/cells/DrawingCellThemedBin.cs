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
	public class DrawingCellThemedBin : DrawingCellBin, ICustomGtkState
	{
		private SideCut sideCut = SideCut.None;
		/// <value>
		/// Specifies arrow type
		/// </value>
		public SideCut SideCut {
			get { return (sideCut); }
			set { 
				if (sideCut == value)
					return;
				sideCut = value;
				OnPropertyChanged ("ButtonSide");
			}
		}
		
		private Style style = null;
		/// <value>
		/// Specifies style used to draw the cell
		/// </value>
		public Style Style {
			get {
				if (style == null)
					style = GetStyle();
				return (style);
			}
		}

		public void ResolveStyle()
		{
			if (style == null)
				style = GetStyle();
		}

		public void FreeStyle()
		{
			style.Dispose();
			style = null;
		}
		
		public StateType OwnerState {
			get { 
				if (Master != null)
					if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true)
						return (((Gtk.Widget) Master).State);
				return (CustomState); 
			}
		}
		
		private StateType customState = StateType.Normal;
		public StateType CustomState {
			get { return (customState); }
			set {
				if (customState == value)
					return;
				customState = value;
				OnPropertyChanged ("CustomState");
			}
		}
		
		private ValueResolveMethod stateResolving = ValueResolveMethod.FromOwner;
		public ValueResolveMethod StateResolving {
			get { return (stateResolving); }
			set {
				if (stateResolving == value)
					return;
				stateResolving = value;
				OnPropertyChanged ("State");
			}
		}
		
		public StateType State {
			get { 
				if (StateResolving == ValueResolveMethod.FromOwner)
					return (OwnerState);
				return (CustomState);
			}
		}
		
		/// <summary>
		/// Resolves style used to draw the cell
		/// </summary>
		/// <returns>
		/// Style <see cref="Style"/>
		/// </returns>
		protected virtual Style GetStyle()
		{
			throw new NotImplementedException ("GetStyle must be overriden in derived classes");
		}

		/// <summary>
		/// Calculates Children areas
		/// </summary>
		/// <param name="aRect">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		public override void DoCalculateCellAreas (CellRectangle aRect)
		{
//System.Console.WriteLine("aRect: {0}", aRect);
			CellRectangle pr = GetPaintableArea();
//			Area = aRect;
//			System.Console.WriteLine("SideCut: {0}", SideCut);
//			System.Console.WriteLine("Area: {0}", Area);
//			System.Console.WriteLine("PaintableArea: {0}", pr);
//			Cairo.Rectangle r = DrawingCellHelper.GetChildAreaByStyle (Style, pr, SideCut);
			base.DoCalculateCellAreas (pr);
			Area.CopyFrom (aRect);
		}

/*		protected override CellRectangle GetPaintableArea()
		{
			CellRectangle rct = new CellRectangle (Area.X+Padding, Area.Y+Padding, Area.Width-(Padding*2), Area.Height-(Padding*2));
			int x,y,r,b;
			x = y = r = b = 0;
			if (Style != null)
//				DrawingCellHelper.GetSideBorderThickness (Style, SideCut, out x, out y, out r, out b);
				DrawingCellHelper.GetBorderThickness (Style, SideCut, out x, out y, out r, out b);
			FreeStyle();
			rct.X += x;
			rct.Y += y;
			rct.Width -= r+x;
			rct.Height -= b+y;
			System.Console.WriteLine("x,y,r,b: {0},{1},{2},{3}", x,y,r,b);
//			return (new Cairo.Rectangle (rct.X, rct.Y, rct.Width-(r+x), rct.Height-(b+y)));
			return (rct);
		}*/
		
		protected override CellRectangle GetChildArea()
		{
			CellRectangle rct = base.GetChildArea();
				//new CellRectangle (Area.X+Padding, Area.Y+Padding, Area.Width-(Padding*2), Area.Height-(Padding*2));
			int x,y,r,b;
			x = y = r = b = 0;
			ResolveStyle();
			DrawingCellHelper.GetBorderThickness (Style, SideCut, out x, out y, out r, out b);
			FreeStyle();
			rct.X += x;
			rct.Y += y;
			rct.Width -= r+x;
			rct.Height -= b+y;
//			System.Console.WriteLine("x,y,r,b: {0},{1},{2},{3}", x,y,r,b);
//			return (new Cairo.Rectangle (rct.X, rct.Y, rct.Width-(r+x), rct.Height-(b+y)));
			return (rct);
		}
		
		/// <summary>
		/// Calculates background drawing rect, rectangle can reside out of clipping area
		/// </summary>
		/// <param name="aArea">
		/// Area <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <returns>
		/// Result rectangle <see cref="Cairo.Rectangle"/>
		/// </returns>
		public override CellRectangle CalculateBackgroundRect (CellRectangle aArea)
		{
			int x,y,r,b;
			x = y = r = b = 0;
			ResolveStyle();
			DrawingCellHelper.GetCutSideBorderThickness (Style, SideCut, out x, out y, out r, out b);
			FreeStyle();
			return (new CellRectangle (aArea.X-x, aArea.Y-y, aArea.Width+r+x, aArea.Height+b+y));
		}
		
		/// <summary>
		/// Resolves size needed for cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public override void GetCellSize (out double aWidth, out double aHeight)
		{
			base.GetCellSize (out aWidth, out aHeight);
			int xThick, yThick, rThick, bThick;
			xThick = yThick = rThick = bThick = 0;
			ResolveStyle();
			DrawingCellHelper.GetBorderThickness (Style, SideCut, out xThick, out yThick, out rThick, out bThick);
			FreeStyle();
			aWidth += xThick+rThick;
			aHeight += yThick+bThick;
		}
		
		public DrawingCellThemedBin()
			: base()
		{
		}
		
		public DrawingCellThemedBin (IDrawingCell aCell)
			: base (aCell)
		{
		}
	}
}
