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

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	public class MappedCellRendererCustomText : MappedCellRendererText
	{
		public virtual string GetDataText (object aData)
		{
			throw new NotImplementedException ("GetDataText must be overriden in subclasses");
		}
		
		/// <summary>
		/// Assigns value to this column
		/// </summary>
		/// <param name="aValue">
		/// Value to be assigned <see cref="System.Object"/>
		/// </param>
		public virtual void AssignValue (object aValue)
		{
			if (aValue != null)
				Text = GetDataText (aValue);
		}
		
		public MappedCellRendererCustomText()
		{
		}
	}
}
