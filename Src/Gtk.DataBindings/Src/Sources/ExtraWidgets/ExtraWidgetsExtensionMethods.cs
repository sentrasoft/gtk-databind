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
	/// Publishes methods which are usefull for ExtraWidgets and beyond
	/// </summary>
	public static class ExtraWidgetsExtensionMethods
	{
		public static Gdk.Pixbuf CreateAlphaCopy (this Gdk.Pixbuf aPixbuf)
		{
			if (aPixbuf == null)
				return (null);
			return (aPixbuf.CreateAlphaCopy (60));
		}
		
		public static Gdk.Pixbuf CreateAlphaCopy (this Gdk.Pixbuf aPixbuf, byte aMaxAlpha)
		{
			if (aPixbuf == null)
				return (null);
			return (BasicUtilities.CreateAlphaPixbuf (aPixbuf, aMaxAlpha));
		}
		
		public static Gdk.Pixbuf CreatePrelightCopy (this Gdk.Pixbuf aPixbuf)
		{
			if (aPixbuf == null)
				return (null);
			return (aPixbuf.CreatePrelightCopy (30));
		}
		
		public static Gdk.Pixbuf CreatePrelightCopy (this Gdk.Pixbuf aPixbuf, byte aShift)
		{
			if (aPixbuf == null)
				return (null);
			return (BasicUtilities.ColorShiftPixbuf (aPixbuf, aShift));
		}
		
		public static bool Activate (this IDrawingCell aCell)
		{
			if (aCell == null)
				return (false);
			IActivatable act = aCell.GetActivatableCell();
			if (act != null) {
				act.Activate();
				return (true);
			}
			return (false);
		}
		
		public static double Right (this Cairo.Rectangle aRectangle)
		{
			return (aRectangle.X+aRectangle.Width);
		}
		
		public static double Bottom (this Cairo.Rectangle aRectangle)
		{
			return (aRectangle.Y+aRectangle.Height);
		}

		public static bool IsEmpty (this Cairo.Rectangle aRectangle)
		{
			return ((aRectangle.Width <= 0) || (aRectangle.Height <= 0));
		}
		
		public static bool Overlaps (this Cairo.Rectangle aRect, Cairo.Rectangle aWith)
		{
			if ((aRect.IsEmpty() == true) || (aWith.IsEmpty() == true))
				return (false);
			return (aRect.Intersection(aWith).IsEmpty() == false);
		}
		
		public static Cairo.Rectangle Intersection (this Cairo.Rectangle aRect, Cairo.Rectangle aWith)
		{
			if (aRect.Overlaps(aWith) == false)
				return (new Cairo.Rectangle (0, 0, -1, -1));
			return (new Cairo.Rectangle (0, 0, -1, -1));
		}
		
		public static Gdk.Rectangle Copy (this Gdk.Rectangle aRect)
		{
			return (new Gdk.Rectangle (aRect.X, aRect.Y, aRect.Width, aRect.Height));
		}
		
		public static Gdk.Rectangle CopyToGdkRectangle (this Cairo.Rectangle aRect)
		{
			return (new Gdk.Rectangle (System.Convert.ToInt32 (aRect.X),
			                           System.Convert.ToInt32 (aRect.Y),
			                           System.Convert.ToInt32 (aRect.Width),
			                           System.Convert.ToInt32 (aRect.Height)));
		}
		
		public static bool IsSensitive (this Gtk.Widget aWidget)
		{
			if (aWidget.Sensitive == false)
				return (false);
			Gtk.Widget wdg = aWidget.Parent;
			while (wdg != null) {
				if (wdg.Sensitive == false) {
					wdg = null;
					return (false);
				}
				wdg = wdg.Parent;
			}
			wdg = null;
			return (true);
		}
		
		/// <summary>
		/// Converts Gdk Color to Cairo Color
		/// </summary>
		/// <param name="aColor">
		/// Color <see cref="Gdk.Color"/>
		/// </param>
		/// <returns>
		/// Cairo color <see cref="Cairo.Color"/>
		/// </returns>
		public static Cairo.Color GetCairoColor (this Gdk.Color aColor)
		{
			return (new Cairo.Color (System.Convert.ToDouble(aColor.Red)/(255*255),
			                         System.Convert.ToDouble(aColor.Green)/(255*255),
			                         System.Convert.ToDouble(aColor.Blue)/(255*255)));
		}
		
		/// <summary>
		/// Converts Cairo Color to Gdk Color
		/// </summary>
		/// <param name="aColor">
		/// Color <see cref="Gdk.Color"/>
		/// </param>
		/// <returns>
		/// Gdk color <see cref="Cairo.Color"/>
		/// </returns>
		public static Gdk.Color GetGdkColor (this Cairo.Color aColor)
		{
			return (new Gdk.Color (System.Convert.ToByte(aColor.R*255),
			                       System.Convert.ToByte(aColor.G*255),
			                       System.Convert.ToByte(aColor.B*255)));
		}

		public static string GetAsString(this Cairo.Color aColor)
		{
			return (string.Format ("Cairo.Color[{0},{1},{2}-alpha={3}", aColor.R, aColor.G, aColor.B, aColor.A));
		}
		
		public static string GetAsString(this Gdk.Color aColor)
		{
			return (string.Format ("Gdk.Color[{0},{1},{2}-pixels={3}", aColor.Red, aColor.Green, aColor.Blue, aColor.Pixel));
		}
		
		/// <summary>
		/// Compares colors if they are equal
		/// </summary>
		/// <param name="aColor">
		/// Original color <see cref="Gdk.Color"/>
		/// </param>
		/// <param name="aCompareTo">
		/// Compared to <see cref="Gdk.Color"/>
		/// </param>
		/// <returns>
		/// true if equal false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsEqualColor (this Gdk.Color aColor, Gdk.Color aCompareTo)
		{
			return ((aColor.Red == aCompareTo.Red) && (aColor.Green == aCompareTo.Green) && (aColor.Blue == aCompareTo.Blue));
		}
		
		/// <summary>
		/// Compares colors if they are equal
		/// </summary>
		/// <param name="aColor">
		/// Original color <see cref="Cairo.Color"/>
		/// </param>
		/// <param name="aCompareTo">
		/// Compared to <see cref="Cairo.Color"/>
		/// </param>
		/// <returns>
		/// true if equal false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsEqualColor (this Cairo.Color aColor, Cairo.Color aCompareTo)
		{
			return ((aColor.R == aCompareTo.R) && (aColor.G == aCompareTo.G) && (aColor.B == aCompareTo.B) && (aColor.A == aCompareTo.A));
		}
		
		/// <summary>
		/// Compares colors if they are equal
		/// </summary>
		/// <param name="aColor">
		/// Original color <see cref="Gdk.Color"/>
		/// </param>
		/// <param name="aCompareTo">
		/// Compared to <see cref="Cairo.Color"/>
		/// </param>
		/// <returns>
		/// true if equal false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsEqualColor (this Gdk.Color aColor, Cairo.Color aCompareTo)
		{
			return (IsEqualColor(aColor, aCompareTo.GetGdkColor()));
		}
		
		/// <summary>
		/// Compares colors if they are equal
		/// </summary>
		/// <param name="aColor">
		/// Original color <see cref="Cairo.Color"/>
		/// </param>
		/// <param name="aCompareTo">
		/// Compared to <see cref="Gdk.Color"/>
		/// </param>
		/// <returns>
		/// true if equal false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsEqualColor (this Cairo.Color aColor, Gdk.Color aCompareTo)
		{
			return (IsEqualColor(aColor, aCompareTo.GetCairoColor()));
		}
		
		/// <summary>
		/// Creates copy of Cairo rectangle
		/// </summary>
		/// <param name="aRectangle">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <returns>
		/// Copy <see cref="Cairo.Rectangle"/>
		/// </returns>
		public static Cairo.Rectangle Copy (this Cairo.Rectangle aRectangle)
		{
			return (new Cairo.Rectangle (aRectangle.X, aRectangle.Y, aRectangle.Width, aRectangle.Height));
		}
		
		/// <summary>
		/// Creates copy of Cairo rectangle and grows it by specified number
		/// </summary>
		/// <param name="aRectangle">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <param name="aGrowFor">
		/// Growth size <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// New rectangle <see cref="Cairo.Rectangle"/>
		/// </returns>
		public static Cairo.Rectangle CopyAndGrow (this Cairo.Rectangle aRectangle, double aGrowFor)
		{
			return (new Cairo.Rectangle (aRectangle.X-aGrowFor, aRectangle.Y-aGrowFor, 
			                             aRectangle.Width+(aGrowFor*2), aRectangle.Height+(aGrowFor*2)));
		}
		
		/// <summary>
		/// Creates copy of Cairo rectangle and shrinks it by specified number
		/// </summary>
		/// <param name="aRectangle">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <param name="aGrowFor">
		/// Shrink size <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// New rectangle <see cref="Cairo.Rectangle"/>
		/// </returns>
		public static Cairo.Rectangle CopyAndShrink (this Cairo.Rectangle aRectangle, double aGrowFor)
		{
			return (aRectangle.CopyAndGrow (-aGrowFor));
		}
		
		/// <summary>
		/// Loads pixbuf resource from store, if specified pixbuf is not already loaded in
		/// store, then this methods loads it from list of resources and registers it
		/// </summary>
		/// <param name="aWidget">
		/// Extension method per widget <see cref="Gtk.Widget"/>
		/// </param>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Result pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		public static Gdk.Pixbuf LoadPixbufFromResourceStore (this Gtk.Widget aWidget, string aName)
		{
			if (PictureResourceStore.Get(aName) != null)
				return ((Gdk.Pixbuf) PictureResourceStore.Get(aName));
			Gdk.Pixbuf pix = new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource (aName));
			if (pix != null)
				PictureResourceStore.Add (aName, pix);
			return (pix);
		}

		/// <summary>
		/// Loads pixbuf resource from store, if specified pixbuf is not already loaded in
		/// store, then this methods loads it from list of resources and registers it
		/// </summary>
		/// <param name="aWidget">
		/// Extension method per drawing cell <see cref="Gtk.Widget"/>
		/// </param>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Result pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		public static Gdk.Pixbuf LoadPixbufFromResourceStore (this IDrawingCell aCell, string aName)
		{
			if (PictureResourceStore.Get(aName) != null)
				return ((Gdk.Pixbuf) PictureResourceStore.Get(aName));
			Gdk.Pixbuf pix = new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource (aName));
			if (pix != null)
				PictureResourceStore.Add (aName, pix);
			return (pix);
		}

		/// <summary>
		/// Loads pixbuf resource from store, if specified pixbuf is not already loaded in
		/// store, then this methods loads it from list of resources and registers it
		/// </summary>
		/// <param name="aWidget">
		/// Extension method per cairo context <see cref="Gtk.Widget"/>
		/// </param>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Result pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		public static Gdk.Pixbuf LoadPixbufFromResourceStore (this Cairo.Context aCell, string aName)
		{
			if (PictureResourceStore.Get(aName) != null)
				return ((Gdk.Pixbuf) PictureResourceStore.Get(aName));
			Gdk.Pixbuf pix = new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource (aName));
			if (pix != null)
				PictureResourceStore.Add (aName, pix);
			return (pix);
		}

		/// <summary>
		/// Loads pixbuf resource from store, if specified pixbuf is not already loaded in
		/// store, then this methods loads it from list of resources and registers it
		/// </summary>
		/// <param name="aWidget">
		/// Extension method per cairo surface <see cref="Gtk.Widget"/>
		/// </param>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Result pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		public static Gdk.Pixbuf LoadPixbufFromResourceStore (this Cairo.Surface aCell, string aName)
		{
			if (PictureResourceStore.Get(aName) != null)
				return ((Gdk.Pixbuf) PictureResourceStore.Get(aName));
			Gdk.Pixbuf pix = new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource (aName));
			if (pix != null)
				PictureResourceStore.Add (aName, pix);
			return (pix);
		}
	}
}
