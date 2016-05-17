//ExtensionMethods.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 3:35 PM 12/31/2008
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
using Cairo;

namespace Gtk.DataBindings
{
	public static class PathExtensions
	{
		public static string ToHtmlColor (this Gdk.Color aColor)
		{
			return (String.Format("#{0:X2}{1:X2}{2:X2}", aColor.Red>>8, aColor.Green>>8, aColor.Blue>>8));
		}
	}
}
