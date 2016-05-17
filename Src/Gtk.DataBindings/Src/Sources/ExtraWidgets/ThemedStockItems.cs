//ThemedStockItems.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 4:08 PM 1/19/2009
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
using System.Collections;
using Gdk;

namespace Gtk.ExtraWidgets
{
	public enum PixbufState
	{
		Normal,
		Highlighted
	}
	
	public static class ThemedStockItems
	{
		private static ArrayList list = new ArrayList();
		
		internal class ThemedPixbufItem
		{
			private Pixbuf normal = null;
			public Pixbuf Normal {
				get { return (normal); }
				private set {
					if (normal == value)
						return;
					if (normal != null)
						normal.Dispose();
					if (highlighted != null)
						highlighted.Dispose();
					normal = value;
					highlighted = BasicUtilities.ColorShiftPixbuf (normal, 30);
				}
			}

			private Pixbuf highlighted = null;
			public Pixbuf Highlighted {
				get { return (highlighted); }
			}

			private string name = "";
			public string Name {
				get { return (name); }
			}

			public static ThemedPixbufItem CreateFromStock (string aName, string aStockName)
			{
				return (null);
			}
			
			public static ThemedPixbufItem CreateFromResource (string aName, string aResourceName)
			{
				return (new ThemedPixbufItem (aName, Pixbuf.LoadFromResource(aResourceName)));
			}
			
			private ThemedPixbufItem (string aName, Pixbuf aPixbuf)
			{
				if (name.Trim() == "")
					throw new Exception ("Tried allocating unnamed ThemedPixbufItem");
				if (aPixbuf == null)
					throw new Exception ("Tried allocating null ThemedPixbufItem");
				name = aName;
				Normal = aPixbuf;
			}

			public static Pixbuf GetItem (string aName, PixbufState aState)
			{
				foreach (ThemedPixbufItem item in list)
					if (item.Name == aName)
						if (aState == PixbufState.Normal)
							return (item.Normal);
						else
							return (item.Highlighted);
				return (null);
			}
		}
	}
}
