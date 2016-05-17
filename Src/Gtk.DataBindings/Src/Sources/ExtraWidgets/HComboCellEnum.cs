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
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class HComboCellEnum : HComboCell
	{
		private Box box = null;
		private PrelightAwareImage image = null;
		private Label label = null;
		
		protected override void OnCellChanged (CellChangedType aChangeType)
		{
			base.OnCellChanged (aChangeType);
			switch (ItemState) {
			case EnumItemState.Normal:
				image.MouseOver = false;
				label.ModifyFg(StateType.Normal);
				break;
			case EnumItemState.Preflight:
				image.MouseOver = true;
				label.ModifyFg(StateType.Normal);
				break;
			case EnumItemState.Selected:
				image.MouseOver = false;
				label.ModifyFg(StateType.Normal, label.Style.Foregrounds[(int) StateType.Selected]);
				break;
			}
			label.Visible = (DisplayMode != EnumItemDisplayMode.IconsOnly);
			image.Visible = (DisplayMode != EnumItemDisplayMode.TextOnly);
		}
		
		private string text = "";
		public string Text {
			get { return (label.Text); }
			set {
				if (label.Text == value)
					return;
				label.Text = text;
				label.Visible = (label.Text != "");
			}
		}
		
		private Gdk.Pixbuf pixbuf = null;
		public Gdk.Pixbuf Pixbuf {
			get { return (image.Pixbuf); }
			set { 
				image.Pixbuf = value; 
				image.Visible = (image.Pixbuf != null);
			}
		}
		
		public override bool ImageVisible {
			get { return ((base.ImageVisible == true) && (Pixbuf != null)); }
		}
		
		public override bool TextVisible {
			get { return ((base.TextVisible == true) && (Text != "")); }
		}
		
		public override void SetLayout()
		{
			if ((image == null) || (label == null))
				return;
			if (box != null) {
				box.Remove (label);
				box.Remove (image);
				Remove (box);
				box.Destroy();
			}
			else {
				image.Show();
				label.Show();
			}
			if (Layout == EnumItemLayout.Horizontal)
				box = new HBox();
			else
				box = new VBox();
			box.Spacing = 4;
			if (ImageVisible == true)
				box.PackStart (image, false, false, 2);
			if (TextVisible == true)
				box.PackStart (label, false, false, 2);
			box.Show();
			Add (box);
		}
		
		public HComboCellEnum (Gdk.Pixbuf aImage, string aLabel)
		{
			if (aImage != null)
				image = new PrelightAwareImage (false, aImage);
			else
				image = new PrelightAwareImage (false);
			label = new Label (aLabel);
			SetLayout();
		}
	}
}
