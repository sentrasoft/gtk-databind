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

namespace Gtk.DataBindings
{
	/// <summary>
	/// Specifies cell renderer which handles date
	/// </summary>
	[GtkCellFactoryProvider ("currency", "DefaultFactoryCreate")]
	[GtkCellFactoryProvider ("numeric", "DefaultFactoryCreate")]
	[GtkTypeCellFactoryProvider ("currencyhandler", "DefaultFactoryCreate", typeof(decimal))]
	public class MappedCellRendererCurrency : MappedCellRendererDrawingCell
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
			MappedCellRendererCurrency wdg = new MappedCellRendererCurrency();
			if (aArgs.InvokationHandler == "currency")
				wdg.ShowCurrency = true;
//			if (aArgs.State == PropertyDefinition.ReadOnly)
			wdg.MappedTo = aArgs.PropertyName;
			return (wdg);
		}
		
		/// <summary>
		/// Converts data to defined string
		/// </summary>
		/// <param name="aData">
		/// Data <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		private string GetDataText (object aData, string aDestination)
		{
			if (aData == null)
				return ("");
			if (TypeValidator.IsNumeric(aData.GetType()) == true) {
				System.Type type = aData.GetType();
				if (type == typeof(byte))
					return (((byte) aData).ToString(aDestination));
				if (type == typeof(int))
					return (((int) aData).ToString(aDestination));
				if (type == typeof(Int16))
					return (((Int16) aData).ToString(aDestination));
				if (type == typeof(UInt16))
					return (((UInt16) aData).ToString(aDestination));
				if (type == typeof(Int32))
					return (((Int32) aData).ToString(aDestination));
				if (type == typeof(UInt32))
					return (((UInt32) aData).ToString(aDestination));
				if (type == typeof(Int64))
					return (((Int64) aData).ToString(aDestination));
				if (type == typeof(UInt64))
					return (((UInt64) aData).ToString(aDestination));
				if (type == typeof(float))
					return (((float) aData).ToString(aDestination));
				if (type == typeof(decimal))
					return (((decimal) aData).ToString(aDestination));
				if (type == typeof(double))
					return (((double) aData).ToString(aDestination));
			}
			return ("");
		}

		private DrawingCellEditText currencytext = new DrawingCellEditText();
		private DrawingCellEditText currencysymbol = null;
		
		private bool showCurrency = false;
		/// <value>
		/// Defines if text is important (bold) or not
		/// </value>
		public bool ShowCurrency {
			get { return (showCurrency); }
			set {
				if (showCurrency == value)
					return;
				showCurrency = value;
				currencysymbol.Visible = showCurrency;
			}
		}
		
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
				currencytext.IsImportant = isImportant;
				currencysymbol.IsImportant = isImportant;
			}
		}
		
		private decimal val = new decimal (0);
		/// <value>
		/// Specifies property which is used to assign value
		/// </value>
		[Property ("value")]
		public decimal Value {
			get { return (val); }
			set {
				if (val.Equals(value) == true)
					return;
				val = value;
				currencytext.Text = GetDataText(val, "N");
			}
		}

		/// <summary>
		/// Returns default data property
		/// </summary>
		/// <returns>
		/// Property name <see cref="System.String"/>
		/// </returns>
		public override string GetDataProperty ()
		{
			return ("value");
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
				Value = System.Convert.ToDecimal (aValue);
			else
				Value = new decimal (0);
		}

		public MappedCellRendererCurrency()
			: base (new DrawingCellHBox())
		{
			System.Console.WriteLine("Created");
			MainBox.PackEnd (new DrawingCellNull(), true);
			MainBox.PackEnd (currencytext, false);
			string s = " " + System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
			currencysymbol = new DrawingCellEditText(s, s);
			MainBox.PackEnd (currencysymbol, false);
			currencysymbol.Visible = false;
		}
	}
}
