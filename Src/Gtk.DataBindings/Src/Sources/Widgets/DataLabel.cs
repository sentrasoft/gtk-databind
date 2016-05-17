// DataLabel.cs - DataLabel implementation for Gtk#Databindings
//
// Author: m. <ml@arsis.net>
//
// Copyright (c) 2006 m.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

using System;
using System.ComponentModel;
using System.Data.Bindings;
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// CheckButton control connected to adaptor and direct updating if connected object
	/// supports IChangeable
	///
	/// Supports single mapping
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	public class DataLabel : Label, IAdaptableControl
	{
		private ControlAdaptor adaptor = null;
		
		/// <summary>
		/// Resolves ControlAdaptor in read-only mode
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public ControlAdaptor Adaptor {
			get { return (adaptor); }
		}
		
		/// <summary>
		/// Defines if DataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedDataSource {
			get { return (adaptor.InheritedDataSource); }
			set { adaptor.InheritedDataSource = value; }
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public object DataSource {
			get { 
				if (adaptor == null)
					return (null);
				return (adaptor.DataSource); 
			}
			set { adaptor.DataSource = value; }
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data mappings")]
		public string Mappings { 
			get { 
				if (adaptor == null)
					return ("");
				return (adaptor.Mappings); 
			}
			set { adaptor.Mappings = value; }
		}
		
		/// <summary>
		/// Defines if DataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedBoundaryDataSource {
			get { return (adaptor.InheritedBoundaryDataSource); }
			set { adaptor.InheritedBoundaryDataSource = value; }
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public IObserveable BoundaryDataSource {
			get { return (adaptor.BoundaryDataSource); }
			set { adaptor.BoundaryDataSource = value; }
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data Mappings")]
		public string BoundaryMappings { 
			get { return (adaptor.BoundaryMappings); }
			set { adaptor.BoundaryMappings = value; }
		}

		private bool important = false;
		public bool Important {
			get { return (important); }
			set {
				if (important == value)
					return;
				important = value;
				GetDataFromDataSource (this);
			}
		}
		
		/// <summary>
		/// Overrides basic Get data behaviour
		///
		/// Assigning this avoids any value transfer between object and data
		/// Basic assigning in DateCalendar for example is
		///    	Date = (DateTime) Adaptor.Value;
		/// where Date is the DateCalendar property and Adaptor.Value is direct
		/// reference to the mapped property
		///
		///     public delegate void UserGetDataEvent (ControlAdaptor Adaptor);
		/// </summary>
		public event CustomGetDataEvent CustomGetData {
			add { adaptor.CustomGetData += value; }
			remove { adaptor.CustomGetData -= value; }
		}
		
		/// <summary>
		/// Calls ControlAdaptors method to transfer data, so it can be wrapped
		/// into widget specific things and all checkups
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public void CallAdaptorGetData (object aSender)
		{
			Adaptor.InvokeAdapteeDataChange (this, aSender);
		}
		
		/// <summary>
		/// Notification method activated from Adaptor 
		/// </summary>
		/// <param name="aSender">
		/// Object that made change <see cref="System.Object"/>
		/// </param>
		public virtual void GetDataFromDataSource (object aSender)
		{
			string preffix = "";
			string suffix = "";
			if (Important == true) {
				preffix = "<b>"; 
				suffix = "</b>";
			}
			// Translate enumeration value
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == true) {
					Array enumValues = Enum.GetValues (type);
					for (int i=0; i<enumValues.Length; i++)
						if (enumValues.GetValue(i).Equals(adaptor.Value)) {
							string desc = enumValues.GetValue(i).ToString();
							if ((UseMarkup == true) || (Important == true))
								Markup = preffix + enumValues.GetValue(i).GetType().GetField(desc).GetEnumTitle() + suffix;
							else
								Text = enumValues.GetValue(i).GetType().GetField(desc).GetEnumTitle();
						}
					return;
				}
			}
			// Translate basic value
			if (adaptor.Value == null)
				Text = "";
			else {
				PropertyDescriptionAttribute attr = adaptor.Adaptor.Mapping(0).GetDescription();
				if (attr != null) {
					switch (attr.DataTypeHandler) {
					case "date":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + ((DateTime) adaptor.Value).ToShortDateString() + suffix;
						else
							Text = ((DateTime) adaptor.Value).ToShortDateString();
						break;
					case "time":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + ((DateTime) adaptor.Value).ToShortTimeString() + suffix;
						else
							Text = ((DateTime) adaptor.Value).ToShortTimeString();
						break;
					case "percent":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + TypeConversions.NumberToPercentString(adaptor.Value) + suffix;
						else
							Text = TypeConversions.NumberToPercentString(adaptor.Value);
						break;
					case "numeric":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + TypeConversions.NumberToNumericString(adaptor.Value) + suffix;
						else
							Text = TypeConversions.NumberToNumericString(adaptor.Value);
						break;
					case "financial":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + TypeConversions.NumberToFinancialString(adaptor.Value) + suffix;
						else
							Text = TypeConversions.NumberToFinancialString(adaptor.Value);
						break;
					case "currency":
						if ((UseMarkup == true) || (Important == true))
							Markup = preffix + TypeConversions.NumberToCurrencyString(adaptor.Value) + suffix;
						else
							Text = TypeConversions.NumberToCurrencyString(adaptor.Value);
						break;
					default:
						Text = adaptor.Value.ToString();
						break;
					}
					return;
				}
				if ((UseMarkup == true) || (Important == true)) {
					if (TypeValidator.IsString (adaptor.Value.GetType()) == false)
						Markup = preffix + adaptor.Value.ToString() + suffix;
					else
						Markup = preffix + (string) adaptor.Value + suffix;
				}
				else {
					if (TypeValidator.IsString (adaptor.Value.GetType()) == false)
						Text = adaptor.Value.ToString();
					else
						Text = (string) adaptor.Value;
				}
			}
		}
		
		protected override void OnShown ()
		{
			if (adaptor.Adaptor.FinalTarget == null)
				Text = "";
			base.OnShown ();
		}

		/// <summary>
		/// Creates Widget 
		/// </summary>
		public DataLabel()
			: this (null, "")
		{
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataLabel (string aMappings)
			: this (null, aMappings)
		{
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aDataSource">
		/// DataSource connected to this widget <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataLabel (object aDataSource, string aMappings)
			: base("")
		{
			Xalign = 0;
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataLabel()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
