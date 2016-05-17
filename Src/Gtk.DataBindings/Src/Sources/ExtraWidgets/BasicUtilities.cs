//BasicUtilities.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 12:01 AM 1/19/2009
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of version 2 of the Lesser GNU General 
//Public License as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA 02111-1307, USA.

using System;
using Gdk;
using Gtk;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Methods needed to perform basic tasks
	/// </summary>
	public static class BasicUtilities
	{
		public static class FocusPopupGrab
		{
			private static uint CURRENT_TIME = 0; 
			
			public static void GrabWindow (Gtk.Window window)
			{
				window.GrabFocus();
				
				Grab.Add(window);
				Gdk.GrabStatus grabbed = Gdk.Pointer.Grab (window.GdkWindow, true, Gdk.EventMask.ButtonPressMask
													                | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask, 
				                                           null, null, CURRENT_TIME);
	
				if (grabbed == Gdk.GrabStatus.Success) {
					grabbed = Gdk.Keyboard.Grab(window.GdkWindow, true, CURRENT_TIME);
	
					if (grabbed != Gdk.GrabStatus.Success) {
						Grab.Remove(window);
						window.Destroy();
					}
				} 
				else {
					Grab.Remove(window);
					window.Destroy();
				}
			}
			
			public static void RemoveGrab (Gtk.Window window)
			{
				Grab.Remove(window);
				Gdk.Pointer.Ungrab (CURRENT_TIME);
				Gdk.Keyboard.Ungrab(CURRENT_TIME);
			}	
		}
	
/*		private static Gtk.Entry entryWidget = null;
		public static Gtk.Entry EntryWidget {
			get {
				if (entryWidget == null)
					entryWidget = new Entry();
				return (entryWidget);
			}
		}*/
		
		internal static byte ConvertColorChannel (byte src, byte alpha)
		{
			return ((alpha != null) ? System.Convert.ToByte(((src<<8)-src)/alpha) : (byte) 0);
		}
		
		/**
		 * cairo_convert_to_pixbuf:
		 * Converts from a Cairo image surface to a GdkPixbuf. Why does GTK+ not
		 * implement this?
		 */
		internal static unsafe Gdk.Pixbuf CairoConvertToPixbuf (Cairo.ImageSurface aSurface)
		{
			Gdk.Pixbuf pixbuf;
			int width, height;
			int srcstride, dststride;
			byte *srcpixels;
			byte *dstpixels;
			byte *srcpixel;
			byte *dstpixel;
			int n_channels;
			
			switch (aSurface.Format) {
			case Cairo.Format.ARGB32:
			case Cairo.Format.RGB24:
				break;
			default:
				return (null);
				break;
			}
			
			width = aSurface.Width;
			height = aSurface.Height;
			srcstride = aSurface.Stride;
			srcpixels = (byte*) aSurface.DataPtr;
			
			pixbuf = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, width, height);
			dststride = pixbuf.Rowstride;
			dstpixels = (byte *) pixbuf.Pixels;
			n_channels = pixbuf.NChannels;
			
			for (int y=0; y<height; y++) {
				for (int x=0; x<width; x++) {
					srcpixel = srcpixels + y * srcstride + x * 4;
					dstpixel = dstpixels + y * dststride + x * n_channels;
			
					dstpixel[0] = ConvertColorChannel (srcpixel[2], srcpixel[3]);
					dstpixel[1] = ConvertColorChannel (srcpixel[1], srcpixel[3]);
					dstpixel[2] = ConvertColorChannel (srcpixel[0], srcpixel[3]);
					dstpixel[3] = srcpixel[3];
				}
			}
			
			return (pixbuf);
		}
		
		private static byte PixelClamp(int val)
		{
			return ((byte) Math.Max(0, Math.Min(255, val)));
		}
		
		/// <summary>
		/// Creates colorshifted version of Pixbuf
		/// </summary>
		/// <param name="src">
		/// Original pixbuf <see cref="Gdk.Pixbuf"/>
		/// </param>
		/// <param name="shift">
		/// Shifting value <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// Colorshifted pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		/// <remarks>
		/// Method is originaly written by Aaron Bockover <abockover@novell.com>
		/// for banshee and originates in HoverImageButton.cs
		/// Originaly licensed as MIT, so as far as I understand this copy
		/// into this library is legaly correct. If not method will be removed
		/// and rewritten
		/// </remarks>
		internal static unsafe Gdk.Pixbuf ColorShiftPixbuf(Gdk.Pixbuf src, byte shift)
		{
			Gdk.Pixbuf dest = new Gdk.Pixbuf (src.Colorspace, src.HasAlpha, src.BitsPerSample, src.Width, src.Height);
			
			byte *src_pixels_orig = (byte *) src.Pixels;
			byte *dest_pixels_orig = (byte *) dest.Pixels;
			
			for (int i=0; i<src.Height; i++) {
				byte *src_pixels = src_pixels_orig + i * src.Rowstride;
				byte *dest_pixels = dest_pixels_orig + i * dest.Rowstride;
				
				for (int j=0; j<src.Width; j++) {
					*(dest_pixels++) = PixelClamp (*(src_pixels++) + shift);
					*(dest_pixels++) = PixelClamp (*(src_pixels++) + shift);
					*(dest_pixels++) = PixelClamp (*(src_pixels++) + shift);
					
					if (src.HasAlpha == true)
						*(dest_pixels++) = *(src_pixels++);
				}
			}
			
			return (dest);
		}

		/// <summary>
		/// Creates version of Pixbuf with specified alpha
		/// </summary>
		/// <param name="src">
		/// Original pixbuf <see cref="Gdk.Pixbuf"/>
		/// </param>
		/// <param name="maxalpha">
		/// Max value of alpha, 255 becomes maxalpha, other are scaled <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// Alpha bleeded pixbuf <see cref="Gdk.Pixbuf"/>
		/// </returns>
		internal static unsafe Gdk.Pixbuf CreateAlphaPixbuf(Gdk.Pixbuf src, byte maxalpha)
		{
			Gdk.Pixbuf dest = new Gdk.Pixbuf (src.Colorspace, true, src.BitsPerSample, src.Width, src.Height);
			
			byte *src_pixels_orig = (byte *) src.Pixels;
			byte *dest_pixels_orig = (byte *) dest.Pixels;
			bool spec = false;
			double b = (System.Convert.ToDouble(maxalpha)/255.0);
			for (int i=0; i<src.Height; i++) {
				byte *src_pixels = src_pixels_orig + i * src.Rowstride;
				byte *dest_pixels = dest_pixels_orig + i * dest.Rowstride;
				
				for (int j=0; j<src.Width; j++) {
					*(dest_pixels++) = PixelClamp (*(src_pixels++));
					*(dest_pixels++) = PixelClamp (*(src_pixels++));
					*(dest_pixels++) = PixelClamp (*(src_pixels++));
					
					if (src.HasAlpha == true)
						*(dest_pixels++) = System.Convert.ToByte (*(src_pixels++)*b);
					else
						*(dest_pixels++) = maxalpha;
				}
			}
			
			return (dest);
		}
	}
}
