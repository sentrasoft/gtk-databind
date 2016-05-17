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
	/// Specifies possibility to provide enumeration value with hint
	/// </summary>
	[AttributeUsage (AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple=false)]
	public class ItemHintAttribute : Attribute
	{
		private string hint = "";
		/// <value>
		/// Title
		/// </value>
		public string Hint {
			get { return (hint); }
			set {
				if (hint == value)
					return;
				hint = value;
			}
		}
	
		private ItemHintAttribute()
		{
		}
	
		public ItemHintAttribute (string aHint)
		{
			hint = aHint;
		}
	}
}
