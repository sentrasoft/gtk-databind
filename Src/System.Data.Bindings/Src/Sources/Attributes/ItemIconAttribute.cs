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
	/// Specifies possibility to provide enumeration value with icon, all it needs
	/// is specific extension which provides picture object. Loaded picture 
	/// is then registered into picture store so any additional loading is ommited
	/// </summary>
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface | 
	                 AttributeTargets.Property | AttributeTargets.Struct | 
	                 AttributeTargets.Enum | AttributeTargets.Field, 
	                 Inherited=false, AllowMultiple=false)]
	public class ItemIconAttribute : Attribute
	{
		private string resourceName = "";
		/// <value>
		/// Title
		/// </value>
		public string ResourceName {
			get { return (resourceName); }
			set {
				if (resourceName == value)
					return;
				resourceName = value;
			}
		}
	
		private object picture = null;
		/// <value>
		/// Picture
		/// </value>
		public object Picture {
			get { 
				// Check if picture is loaded
				if (picture == null) {
					// Try loading from store first
					picture = PictureResourceStore.Get (ResourceName);
					if (picture == null) {
						// Load and save picture to store
						picture = LoadPicture();
						if (picture != null)
							PictureResourceStore.Add (ResourceName, picture);
					}
				}
				return (picture); 
			}
		}
		
		/// <summary>
		/// Loads picture
		/// </summary>
		/// <returns>
		/// Picture <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// This method has to be overriden in subclasses
		/// </remarks>
		protected virtual object LoadPicture()
		{
			try { return (PictureResourceStore.LoadResource (ResourceName)); }
			catch (Exception e) {
				throw new NotImplementedException ("ItemIconAttribute has to provide specific resource loaders, use inherited classes instead\n" +
				                                   e.Message);
			}
		}
		
		private ItemIconAttribute()
		{
		}
	
		public ItemIconAttribute (string aResourceName)
		{
			resourceName = aResourceName;
		}
	}
}
