//CustomPaintedContainer.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 2:19 AM 1/23/2009
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

namespace Gtk.ExtraWidgets
{	
	/// <summary>
	/// Methods used to control painted containers
	/// </summary>
	public static class CustomPaintedContainer
	{
		public static bool OnExposeEvent (Gtk.Container aContainer, Gdk.EventExpose ev)
		{
			if (aContainer == null)
				return (true);
			int x = aContainer.Allocation.Left;
			int y = aContainer.Allocation.Top;
			int width = aContainer.Allocation.Width;
			int height = aContainer.Allocation.Height;

			Widget e = GetEntryStyle (aContainer);
			Gtk.Style s = aContainer.Style;
			StateType st = aContainer.State;
			if (e != null) {
				s = e.Style;
				st = e.State;
			}
			if (GetContainerFocus(aContainer) == true) {
				if ((e == null) || (e.HasFocus == false)) {
					st = StateType.Selected;
				}
				Gtk.Style.PaintFlatBox(s, aContainer.GdkWindow, st, Gtk.ShadowType.In, 
			   	                    aContainer.Allocation, aContainer, "entry_bg", x, y, width, height);
				Gtk.Style.PaintFocus (s, aContainer.GdkWindow, st,
				                      ev.Area, aContainer, "entry", x, y, width, height);
				Gtk.Style.PaintShadow(s, aContainer.GdkWindow, st, Gtk.ShadowType.In, 
				                      ev.Area, aContainer, "entry", x, y, width, height);
			}
			else {
				Gtk.Style.PaintFlatBox(s, aContainer.GdkWindow, st, Gtk.ShadowType.In, 
				                       aContainer.Allocation, aContainer, "entry_bg", x, y, width, height);
				Gtk.Style.PaintShadow (s, aContainer.GdkWindow, st, Gtk.ShadowType.In,
				                       ev.Area, aContainer, "entry", x, y, width, height);
			} 
			e = null;
			s = null;

			return (true);
		}
				
		public static bool GetContainerFocus (Widget aWidget)
		{
			return (GetFocusedWidget(aWidget) != null);
		}
		
		public static Widget GetFocusedWidget (Widget aWidget)
		{
			if (aWidget == null)
				return (null);
			if (aWidget.HasFocus == true)
				return (aWidget);
			if (aWidget is Container) {
				foreach (Widget wdg in (aWidget as Container).Children) {
					if (wdg is Container) {
						if (GetContainerFocus(wdg) == true)
							return (wdg);
					}
					else
						if (wdg.HasFocus == true)
							return (wdg);
				}
					
			}
			return (null);
		}
		
		public static Widget GetEntryStyle (Widget aWidget)
		{
			if (aWidget == null)
				return (null);
			Widget w = GetFocusedWidget (aWidget);
			if (w is Entry)
				return (w);
			
			if (aWidget is Container) {
				foreach (Widget wdg in (aWidget as Container).Children) {
					if (wdg is Container) {
						if (GetEntryStyle(wdg) != null)
							return (GetEntryStyle(wdg));
					}
					else
						if (wdg is Entry)
							return (wdg);
				}
					
			}
			return (null);
		}
		
		public static void Add (Gtk.Container aContainer, Widget aWidget)
		{
			if (aWidget == null)
				return;
			if (aContainer == null)
				throw new Exception ("Adding widget to null container");
			aContainer.Add (aWidget);
		}

		public static void SetRedrawOnFocusChildSet (Gtk.Container aContainer)
		{
			if (aContainer == null)
				throw new Exception ("Adding widget to null container");
			aContainer.FocusChildSet += delegate(object o, FocusChildSetArgs args) {
				aContainer.QueueDraw(); 
			};
		}
	}
}
