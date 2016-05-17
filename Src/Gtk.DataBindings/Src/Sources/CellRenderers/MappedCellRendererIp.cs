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
using Gtk.ExtraWidgets;
using GLib;
using System.Globalization;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Specifies cell renderer which handles date
	/// </summary>
	[GtkCellFactoryProvider ("ip", "DefaultFactoryCreate")]
	[GtkTypeCellFactoryProvider ("iphamdler", "DefaultFactoryCreate", typeof(System.Net.IPAddress))]
	public class MappedCellRendererIp : MappedCellRendererDrawingCell
	{
		/// <summary>
		/// Registered factory creation method
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		public static IMappedColumnItem DefaultFactoryCreate (FactoryInvocationArgs aArgs)
		{
			IMappedColumnItem wdg = new MappedCellRendererIp();
//			if (aArgs.State == PropertyDefinition.ReadOnly)
			wdg.MappedTo = aArgs.PropertyName;
			return (wdg);
		}
		
		private DrawingCellEditText[] labels = new DrawingCellEditText[4] {
			new DrawingCellEditText(), new DrawingCellEditText(), new DrawingCellEditText(), new DrawingCellEditText()
		};
		
		private bool isImportant = false;
		/// <value>
		/// Defines if text is important (bold) or not
		/// </value>
		public bool IsImportant {
			get { return (isImportant); }
			set {
				if (isImportant == value)
					return;
				isImportant = value;
				foreach (IDrawingCell cell in Cells)
					if (TypeValidator.IsCompatible(cell.GetType(), typeof(DrawingCellEditText)) == true)
						(cell as DrawingCellEditText).IsImportant = isImportant;
			}
		}
		
		private System.Net.IPAddress ip = new System.Net.IPAddress(0);
		/// <value>
		/// Specifies property which is used to assign value
		/// </value>
		[Property ("ip")]
		public System.Net.IPAddress Ip {
			get { return (ip); }
			set {
				if (ip.Equals(value) == true)
					return;
				ip = value;
				byte[] addr = ip.GetAddressBytes();
				for (int i=0; i<4; i++)
					labels[i].Text = addr[i].ToString();
				addr = null;
			}
		}
		
		public override string GetDataProperty ()
		{
			return ("ip");
		}

		/// <summary>
		/// Assigns value to this column
		/// </summary>
		/// <param name="aValue">
		/// Value to be assigned <see cref="System.Object"/>
		/// </param>
		public void AssignValue (object aValue)
		{
			if (aValue != null)
				Ip = (System.Net.IPAddress) aValue;
		}

		public MappedCellRendererIp()
			: base (new DrawingCellHBox())
		{
			for (int i=0; i<4; i++)
				labels[i].SizeText = "888";
			MainBox.PackEnd (new DrawingCellNull(), true);
			for (int i=0; i<4; i++)
				labels[i].XPos = 0.5;
			MainBox.PackEnd (labels[0], false);
			MainBox.PackEnd (new DrawingCellEditText("."), false);
			MainBox.PackEnd (labels[1], false);
			MainBox.PackEnd (new DrawingCellEditText("."), false);
			MainBox.PackEnd (labels[2], false);
			MainBox.PackEnd (new DrawingCellEditText("."), false);
			MainBox.PackEnd (labels[3], false);
		}
	}
}
