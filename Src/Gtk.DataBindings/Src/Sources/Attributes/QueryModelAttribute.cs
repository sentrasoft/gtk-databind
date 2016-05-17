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

namespace Gtk.DataBindings
{	
	/// <summary>
	/// Specifies attribute which defines class is query implementor for 
	/// specific type
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
	public class QueryModelAttribute : Attribute
	{
		private System.Type listType = null;
		/// <value>
		/// Type which is handled by derived QueryImplementor
		/// </value>
		public System.Type ListType {
			get { return (listType); }
		}
		
		private bool inherited = true;
		/// <value>
		/// Specifies if QueryImplementor handles inherited classes too or not
		/// </value>
		public bool Inherited {
			get { return (inherited); }
		}
		
		private System.Type itemTypeOverride = null;
		/// <value>
		/// Provides type override for Query model
		/// </value>
		public System.Type ItemTypeOverride {
			get { return (itemTypeOverride); }
		}
		
		public QueryModelAttribute (System.Type aType)
		{
			if (aType == null)
				throw new NullReferenceException ("QueryModelAttribute can't handle null type");
			listType = aType;
		}
		
		public QueryModelAttribute (System.Type aType, System.Type aItemOverrideType)
			: this (aType)
		{
			itemTypeOverride = aItemOverrideType;
		}
	}
}
