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
	/// CellRenderer used by DataTreeView and DataComboBox.
	/// 
	/// Additional functionality is providing information where and
	/// how it is mapped
	/// </summary>
	public class MappedCellRenderer : Gtk.CellRenderer, IMappedColumnItem
	{
		private System.Type mappedType = (System.Type) null;
		/// <value>
		/// System.Type resolved for this mapping
		/// </value>
		public System.Type MappedType {
			get { return (mappedType); } 
			set { mappedType = value; }
		}
		
		private string mappedTo = "";
		/// <value>
		/// Name of the mapped property
		/// </value>
		public string MappedTo {
			get { return (mappedTo); } 
			set { mappedTo = value; }
		}
		
		private int columnIndex = -1;
		/// <value>
		/// Column index of this CellRenderer
		/// </value>
		public int ColumnIndex {
			get { return (columnIndex); }
			set { columnIndex = value; }
		}

		private bool isSubItem = false;
		/// <value>
		/// Returns true if this is subitem in another column
		/// </value>
		public bool IsSubItem {
			get { return (isSubItem); }
			set { isSubItem = value; }
		}

		private int subColumnIndex = -1;
		/// <value>
		/// If this is subitem column of another column, then it specifies
		/// index number of that column
		/// </value>
		public int SubColumnIndex { 
			get { return (subColumnIndex); }
			set { subColumnIndex = value; } 
		}
	
		private bool editable = false;
		public bool Editable {
			get { return (editable); }
			set {
				if (editable == value)
					return;
				editable = value;
			}
		}
		
		/// <summary>
		/// Returns default data property
		/// </summary>
		public virtual string GetDataProperty()
		{
			throw new NotImplementedException ("GetDataProperty must be overriden in MappedCellRenderer class");
		}
		
		/// <summary>
		/// Assigns value to this column
		/// </summary>
		/// <param name="aValue">
		/// Value to be assigned <see cref="System.Object"/>
		/// </param>
		public virtual void AssignValue (object aValue)
		{
			throw new ExceptionInvalidAssignmentCellRenderer (GetType());
		}
	}
}
