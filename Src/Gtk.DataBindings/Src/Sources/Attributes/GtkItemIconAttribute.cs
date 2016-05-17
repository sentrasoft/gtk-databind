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
	/// Attribute which loads its icon picture from resource. Loaded picture 
	/// is then registered into picture store so any additional loading is ommited
	/// </summary>
	public class GtkItemIconAttribute : ItemIconAttribute
	{
		/// <summary>
		/// Loads Gdk.Pixbuf resource
		/// </summary>
		/// <param name="aName">
		/// Picture resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Picture object <see cref="System.Object"/>
		/// </returns>
		protected static object LoadPictureResource (string aName)
		{
			return (new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource(aName)));
		}

		/// <summary>
		/// Registers default resource handler for picture resources
		/// </summary>
		public static void RegisterDefaultResourceHandler()
		{
			PictureResourceStore.LoadPictureResource += LoadPictureResource;
		}
		
		/// <summary>
		/// Loads Gdk.Pixbuf resource
		/// </summary>
		/// <returns>
		/// Picture object <see cref="System.Object"/>
		/// </returns>
		protected override object LoadPicture()
		{
			return (new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource(ResourceName)));
//			return (Gdk.Pixbuf.LoadFromResource (ResourceName));
		}

		public GtkItemIconAttribute (string aResourceName)
			: base (aResourceName)
		{
		}
	}
}
