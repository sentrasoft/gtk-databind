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
	/// Cell specifying one label
	/// </summary>	
	public class HComboCellText : HComboCell
	{
		private Label label = null;
		
		/// <summary>
		/// Cell changed event
		/// </summary>
		/// <param name="aChangeType">
		/// Change type <see cref="CellChangedType"/>
		/// </param>
		protected override void OnCellChanged (CellChangedType aChangeType)
		{
			base.OnCellChanged (aChangeType);
			switch (ItemState) {
			case EnumItemState.Normal:
			case EnumItemState.Preflight:
				label.ModifyFg(StateType.Normal);
				if (BoldWhenSelected == true)
					label.Text = Text;
				break;
			case EnumItemState.Selected:
				label.ModifyFg(StateType.Normal, label.Style.Foregrounds[(int) StateType.Selected]);
				if (BoldWhenSelected == true)
					label.Markup = "<b>" + Text + "</b>";
				break;
			}
			label.Visible = (DisplayMode != EnumItemDisplayMode.IconsOnly);
		}
		
		private string text = "";
		/// <value>
		/// Text displayed in cell
		/// </value>
		public string Text {
			get { return (text); }
			set {
				if (text == value)
					return;
				text = value;
				label.Text = text;
				label.Visible = (text != "");
			}
		}
		
		private bool boldWhenSelected = true;
		public bool BoldWhenSelected {
			get { return (boldWhenSelected); }
			set {
				if (boldWhenSelected == value)
					return;
				if (ItemState == EnumItemState.Selected)
					if (value == true)
						label.Markup = "<b>" + text + "</b>";
					else
						label.Text = text;
			}
		}
		
		public HComboCellText (string aLabel)
		{
			label = new Label();
			Text = aLabel;
			label.Show();
			Add (label);
			SetLayout();
		}
	}
}
