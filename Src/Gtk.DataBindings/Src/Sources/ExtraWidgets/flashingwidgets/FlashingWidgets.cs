//FlashingWidgets.cs - Description
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) 2008 m.
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
//
//

using System;
using System.Collections;
using Gtk;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public static class FlashingWidgets
	{
		private static ArrayList queue = new ArrayList();

		public static WidgetFlasher GetFlasher (Widget aWidget) 
		{
			foreach (WidgetFlasher item in queue)
				if (item.FlashedWidget == aWidget)
					return (item);
			return (null);
		}
		
		public static bool Exists (WidgetFlasher aFlasher) 
		{
			if (queue.IndexOf(aFlasher) > -1)
				return (true);
			return (false);
		}
		
		public static void Add (Widget aWidget)
		{
			WidgetFlasher item = GetFlasher(aWidget); 
			if (item != null)
				item.Cancel();
			queue.Add (new WidgetFlasher (aWidget));
		}

		public static void Add (WidgetFlasher aFlasher)
		{
			if ((aFlasher == null) || (Exists(aFlasher) == true))
				return;

			queue.Add (aFlasher);
		}

		public static void Remove (Widget aWidget)
		{
			if (aWidget == null)
				return;
			WidgetFlasher flasher = GetFlasher(aWidget);
			if (flasher == null)
				return;
			if (flasher.IsActive == true) {
				flasher.Cancel();
				return;
			}
		}
		
		public static void Remove (WidgetFlasher aFlasher)
		{
			if (aFlasher == null)
				return;
			if (aFlasher.IsActive == true) {
				aFlasher.Cancel();
				return;
			}
			queue.Remove (aFlasher);
		}
		
		public static void CancelAll()
		{
			for (int i=(queue.Count-1); i==0; i--)
				if (queue[i] != null)
					(queue[i] as WidgetFlasher).Cancel();
		}
	}
}
