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
using System.Collections.Generic;

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies filters in theme. If filter is added manually then adding needs to
	/// respect the fact that themes are invoked by using 0 as first and so on
	/// </summary>
	public class WidgetFactoryTheme
	{
		private List<string> filters = new List<string>();
		/// <value>
		/// Filter list
		/// </value>
		public List<string> Filters {
			get { return (filters); }
		}
		
		/// <summary>
		/// Adds filter in correct order
		/// </summary>
		/// <param name="aFilterName">
		/// A <see cref="System.String"/>
		/// </param>
		public void AddFilter (string aFilterName)
		{
			if ((aFilterName.Trim() == "") || (filters.IndexOf(aFilterName.Trim()) > -1))
				return;
			if (filters.Count == 0)
				filters.Add (aFilterName.Trim());
			else
				filters.Insert (0, aFilterName.Trim());
		}
	}
}
