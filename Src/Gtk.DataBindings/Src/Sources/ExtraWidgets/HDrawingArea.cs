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
using System.ComponentModel;
using Gtk;
using Gdk;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies DrawingArea with horizontally aligned drawing cells
	/// </summary>
	[ToolboxItem (true)]
	public class HDrawingArea : CellDrawingArea
	{
		/// <summary>
		/// Creates main box which defines basic layout
		/// </summary>
		/// <returns>
		/// Box <see cref="DrawingCellBox"/>
		/// </returns>
		protected override DrawingCellBox CreateBox ()
		{
			return (new DrawingCellHBox());
		}
	}
}
