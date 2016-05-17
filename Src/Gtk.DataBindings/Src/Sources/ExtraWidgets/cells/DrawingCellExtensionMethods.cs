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
using System;
using System.Data.Bindings;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Provides extension methods for DrawingCell types
	/// </summary>
	public static class DrawingCellExtensionMethods
	{
		/// <summary>
		/// Checks if cell area is inside of clipping area
		/// </summary>
		/// <param name="aCell">
		/// A <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aClippingArea">
		/// Cllipping rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <returns>
		/// true if area intersects with clipping area, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsInsideArea (this IDrawingCell aCell, Cairo.Rectangle aClippingArea)
		{
			if (aCell.Area.X > (aClippingArea.X+aClippingArea.Width))
				return (false);
			if ((aCell.Area.X+aCell.Area.Width) < aClippingArea.X)
				return (false);
			if (aCell.Area.Y > (aClippingArea.Y+aClippingArea.Height))
				return (false);
			if ((aCell.Area.Y+aCell.Area.Height) < aClippingArea.Y)
				return (false);
			return (true);
		}
		
		public static Gdk.Pixbuf GetAsPixbuf (this IDrawingCell aCell)
		{
			return (aCell.GetAsPixbuf (true));
		}
		
		public static Gdk.Pixbuf GetAsPixbuf (this IDrawingCell aCell, bool aFramed)
		{
			int fw = (aFramed == true) ? 1 : 0;
			Cairo.ImageSurface surface = new Cairo.ImageSurface (Cairo.Format.RGB24, System.Convert.ToInt32(aCell.Area.Width+2), System.Convert.ToInt32(aCell.Area.Height+2));
			Cairo.Context context = new Cairo.Context (surface);
			CellRectangle rect = new CellRectangle (0, 0, aCell.Area.Width+(fw*2), aCell.Area.Height+(fw*2));
			context.Color = new Cairo.Color (1, 1, 1);
			rect.DrawPath (context);
//			context.Rectangle (rect);
			context.FillPreserve();
			context.Color = new Cairo.Color (0, 0, 0);
			if (aFramed == true) {
				context.Stroke();
				rect = new CellRectangle (1, 1, aCell.Area.Width+1, aCell.Area.Height+1);
			}
			CellRectangle fake = new CellRectangle (0, 0, 9999999, 9999999);
			CellExposeEventArgs args = new CellExposeEventArgs (null, context, null, fake, rect);
			args.NeedsRecalculation = true;
			aCell.Paint (args);
			
			Gdk.Pixbuf px = surface.GetAsPixbuf();
			
			((IDisposable) context.Target).Dispose();                               
			((IDisposable) context).Dispose();
			((IDisposable) surface).Dispose();
			return (px);
		}
		
		public static Gdk.Pixbuf GetAsPixbuf (this Cairo.ImageSurface aSurface)
		{
			Gdk.Pixbuf px = BasicUtilities.CairoConvertToPixbuf (aSurface);
			return (px);
		}
		
		public static StateType ResolveState (this IDrawingCell aCell)
		{
			if (aCell.MasterIsSensitive() == false)
				return (StateType.Insensitive);
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(IGtkState)) == true)
				return ((aCell as IGtkState).State);
			if (TypeValidator.IsCompatible(aCell.Owner.GetType(), typeof(IDrawingCell)) == true)
				return ((aCell.Owner as IDrawingCell).ResolveState());
			if (TypeValidator.IsCompatible(aCell.Owner.GetType(), typeof(Gtk.Widget)) == true)
				return ((aCell.Owner as Gtk.Widget).State);
			return (StateType.Normal);
		}
		
		public static bool MasterIsGtkWidget (this IDrawingCell aCell)
		{
			return (TypeValidator.IsCompatible(aCell.Master.GetType(), typeof(Gtk.Widget)) == true);
		}
		
		public static bool MasterIsSensitive (this IDrawingCell aCell)
		{
			if (aCell.MasterIsGtkWidget() == true)
				return ((aCell.Master as Gtk.Widget).IsSensitive());
			return (true);
		}
		
		public static IActivatable GetActivatableCell (this IDrawingCell aCell)
		{
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(IActivatable)) == true)
				return (aCell as IActivatable);
			if (aCell.Owner == null)
				return (null);
			if (TypeValidator.IsCompatible(aCell.Owner.GetType(), typeof(IDrawingCell)) == true)
				return ((aCell.Owner as IDrawingCell).GetActivatableCell());
			return (null);
		}
		
		public static void SetGtkState (this IDrawingCell aCell, StateType aState)
		{
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(ICustomGtkState)) == true) {
				if ((aCell as ICustomGtkState).StateResolving == ValueResolveMethod.Manual)
					(aCell as ICustomGtkState).CustomState = aState;
				else
					return;
				return;
			}
			if (aCell.Owner != null)
				if (TypeValidator.IsCompatible(aCell.Owner.GetType(), typeof(IDrawingCell)) == true)
					(aCell.Owner as IDrawingCell).SetGtkState (aState);
		}
	}
}
