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
using Gtk;
using Gdk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Attribute which loads its icon picture from file. Loaded picture 
	/// is then registered into picture store so any additional loading is ommited
	/// </summary>
	public class GtkItemIconFileAttribute : ItemIconAttribute
	{
		/// <summary>
		/// Loads Gdk.Pixbuf resource
		/// </summary>
		/// <returns>
		/// Picture object <see cref="System.Object"/>
		/// </returns>
		protected override object LoadPicture()
		{
			return (new Gdk.Pixbuf (ResourceName));
		}

		public GtkItemIconFileAttribute (string aResourceName)
			: base (aResourceName)
		{
		}
	}
}
