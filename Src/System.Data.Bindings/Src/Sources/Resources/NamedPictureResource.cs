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

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies named picture resource
	/// </summary>
	public class NamedPictureResource : NamedObject
	{
		private object picture = null;
		/// <value>
		/// Specifies picture as presented by resource
		/// </value>
		public object Picture {
			get { return (picture); }
			set {
				if (picture == value)
					return;
				picture = value;
				OnPropertyChanged ("Picture");
			}
		}
		
		public NamedPictureResource (string aName, object aPicture)
			: base (aName)
		{
			picture = aPicture;
		}
	}
}
