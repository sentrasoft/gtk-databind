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
using System.Collections;
using System.Collections.Generic;
using System.Data.Bindings.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies store where named picture resources are shared
	/// </summary>
	public static class PictureResourceStore
	{
		private static List<NamedPictureResource> pictures = new List<NamedPictureResource>();
		
		private static event LoadPictureEvent loadPicture = null;
		/// <value>
		/// Default method fallback for loading pictures
		/// </value>
		public static event LoadPictureEvent LoadPicture {
			add { loadPicture += value; }
			remove { loadPicture -= value; }
		}
		
		private static event LoadPictureEvent loadPictureResource = null;
		/// <value>
		/// Default method fallback for loading pictures
		/// </value>
		public static event LoadPictureEvent LoadPictureResource {
			add { loadPictureResource += value; }
			remove { loadPictureResource -= value; }
		}
		
		/// <value>
		/// Returns count of registered resources
		/// </value>
		public static int Count {
			get { return (pictures.Count); }
		}
		
		public static object LoadResource (string aName)
		{
			if (Get(aName) != null)
				return (Get (aName));
			if (loadPictureResource == null)
				throw new NotImplementedException ("Default handler LoadPictureResource was not specified");
			object pic = loadPictureResource (aName);
			if (pic != null)
				Add (aName, pic);
			return (pic);
		}
		
		/// <value>
		/// Returns picture object located by name
		/// </value>
		public static object Get (string aName) 
		{
			foreach (NamedPictureResource res in pictures)
				if (res.Name == aName)
					return (res.Picture);
			return (null);
		}

		/// <value>
		/// Returns NamedPictureResource at specified index
		/// </value>
		public static NamedPictureResource Get (int aIdx) 
		{
			if ((aIdx < 0) || (aIdx >= pictures.Count))
				throw new IndexOutOfRangeException ("Picture resource out of range");
			return (pictures[aIdx]);
		}

		/// <summary>
		/// Adds new picture resource into store
		/// </summary>
		/// <param name="aName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <param name="aPicture">
		/// Picture <see cref="System.Object"/>
		/// </param>
		public static void Add (string aName, object aPicture)
		{
			if ((aName == "") || (aPicture == null) || (Get(aName) != null))
				return;
			pictures.Add (new NamedPictureResource (aName, aPicture));
		}
	}
}
