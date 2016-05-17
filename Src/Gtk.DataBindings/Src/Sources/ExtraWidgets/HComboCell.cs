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
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies base for derived horizontal combo box cells
	/// </summary>
	public class HComboCell : HBox //TransparentSelectionEventBox
	{
		private EnumItemState itemState = EnumItemState.Normal;
		/// <value>
		/// Specifies item state
		/// </value>
		public EnumItemState ItemState {
			get { return (itemState); }
			set {
				if (itemState == value)
					return;
				itemState = value;
				OnCellChanged (CellChangedType.State);
			}
		}

		/// <summary>
		/// Notifies clients about change in the cell
		/// </summary>
		/// <param name="aChangeType">
		/// Type of change <see cref="CellChangedType"/>
		/// </param>
		protected virtual void OnCellChanged (CellChangedType aChangeType)
		{
			if (cellChanged != null)
				cellChanged (this, new EnumCellChangedEventArgs (aChangeType, this));
		}

		private event EnumCellChangedEvent cellChanged = null;
		/// <value>
		/// Event triggered on cell change
		/// </value>
		public event EnumCellChangedEvent CellChanged {
			add { cellChanged += value; }
			remove { cellChanged -= value; }
		}
		
		private EnumItemLayout layout = EnumItemLayout.Horizontal;
		/// <value>
		/// Defines item layout, some derived classes might handle it, some not
		/// </value>
		public EnumItemLayout Layout {
			get { return (layout); }
			set { 
				if (layout == value)
					return;
				layout = value;
				SetLayout();
			}
		}

		private EnumItemDisplayMode displayMode = EnumItemDisplayMode.TextAndIcon;
		/// <value>
		/// Specifies display mode
		/// </value>
		public EnumItemDisplayMode DisplayMode {
			get { return (displayMode); }
			set {
				if (displayMode == value)
					return;
				displayMode = value;
				OnCellChanged (CellChangedType.Display);
			}
		}
		
		/// <value>
		/// Resolves if image is visible
		/// </value>
		public virtual bool ImageVisible {
			get { return (DisplayMode != EnumItemDisplayMode.TextOnly); }
		}
		
		/// <value>
		/// Resolves if text is visible
		/// </value>
		public virtual bool TextVisible {
			get { return (DisplayMode != EnumItemDisplayMode.IconsOnly); }
		}
		
		/// <summary>
		/// Custom layout method
		/// </summary>
		public virtual void SetLayout()
		{
		}
				
		public HComboCell()
		{
			SetLayout();
		}
	}
}
