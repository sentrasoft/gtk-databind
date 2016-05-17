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
using System.ComponentModel;
using System.Data.Bindings;
using System.Reflection;
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	[GtkWidgetFactoryProvider ("enum", "DefaultFactoryCreate")]
	[GtkTypeWidgetFactoryProvider ("enumhandler", "DefaultFactoryCreate", typeof(Enum))]
	public class DataEnumComboBox : ComboBox, IAdaptableControl, IPostableControl
	{
		/// <summary>
		/// Registered factory creation method
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
		public static IAdaptableControl DefaultFactoryCreate (FactoryInvocationArgs aArgs)
		{
			IAdaptableControl wdg;
			if (aArgs.State == PropertyDefinition.ReadOnly)
				wdg = new DataLabel();
			else
				wdg = new DataEnumComboBox();
			wdg.Mappings = aArgs.PropertyName;
			return (wdg);
		}
		
		private System.Type lastType = null;
		private ControlAdaptor adaptor = null;

		/// <summary>
		/// Resolves ControlAdaptor in read-only mode
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public ControlAdaptor Adaptor {
			get { return (adaptor); }
		}
		
		/// <summary>
		/// Defines if BoundaryDataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedBoundaryDataSource {
			get { return (adaptor.InheritedBoundaryDataSource); }
			set { adaptor.InheritedBoundaryDataSource = value; }
		}
		
		/// <summary>
		/// BoundaryDataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public IObserveable BoundaryDataSource {
			get { return (adaptor.BoundaryDataSource); }
			set { adaptor.BoundaryDataSource = value; }
		}
		
		/// <summary>
		/// Link to BoundaryMappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data Mappings")]
		public string BoundaryMappings { 
			get { return (adaptor.BoundaryMappings); }
			set { adaptor.BoundaryMappings = value; }
		}
		
		/// <summary>
		/// Defines if DataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedDataSource {
			get { return (adaptor.InheritedDataSource); }
			set { 
				if (adaptor.InheritedDataSource == value)
					return;
				adaptor.InheritedDataSource = value; 
				ResetLayout();
			}
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public object DataSource {
			get { return (adaptor.DataSource); }
			set { 
				if (adaptor == null)
					return;
				if (adaptor.DataSource == value)
					return;
				adaptor.DataSource = value; 
				ResetLayout();
			}
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
			set { 
				if (adaptor == null)
					return;
				if (adaptor.Mappings == value)
					return;
				adaptor.Mappings = value; 
				ResetLayout();
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
		/// Overrides basic Post data behaviour
		///
		/// Assigning this avoids any value transfer between object and data
		/// Basic assigning in DateCalendar for example is
		///    	adaptor.Value = Date;
		/// where Date is the DateCalendar property and Adaptor.Value is direct
		/// reference to the mapped property
		///
		///     public delegate void UserPostDataEvent (ControlAdaptor Adaptor);
		/// </summary>
		public event CustomPostDataEvent CustomPostData {
			add { adaptor.CustomPostData += value; }
			remove { adaptor.CustomPostData -= value; }
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
			adaptor.DataChanged = false;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == false)
					throw new NotSupportedException ("DataRadioGroup only supports enum types as mapping");
				Array enumValues = Enum.GetValues (type);
				for (int i=0; i<enumValues.Length; i++)
					if (enumValues.GetValue(i).Equals(adaptor.Value))
						Active = i;
			}
		}
		
		/// <summary>
		/// Updates parent object to DataSource object
		/// </summary>
		public virtual void PutDataToDataSource (object aSender)
		{
			adaptor.DataChanged = false;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == false)
					throw new NotSupportedException ("DataRadioGroup only supports enum types as mapping");
				Array enumValues = Enum.GetValues (type);
				adaptor.Value = enumValues.GetValue(Active);
			}
		}
		
		/// <summary>
		/// Overrides OnToggled to put data in DataSource if needed
		/// </summary>
		protected override void OnChanged()
		{
			adaptor.DemandInstantPost();
			base.OnChanged();
		}
		
		/// <summary>
		/// Resets widget layout
		/// </summary>
		private void ResetLayout()
		{
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type == lastType)
					return;
			}

			Clear();
			if ((Cells != null) && (Cells.Length > 0))
				ClearAttributes (Cells[0]);
			lastType = null;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				string[] columns = adaptor.Adaptor.Values[0].Value.GetType().GetEnumLayout();
				if (columns.Length == 2) {
					CellRendererPixbuf cp = new CellRendererPixbuf();
					PackStart (cp, false);
					AddAttribute (cp, "pixbuf", 0);
				}
				CellRendererText cr = new CellRendererText();
				PackStart(cr, true);
				AddAttribute (cr, "text", columns.Length-1);
				//System.Console.WriteLine("Building");
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				lastType = type;
				if (type.IsEnum == false)
					throw new NotSupportedException (string.Format("DataRadioGroup only supports enum types as mapping, specified was {0}", type));
				string s;
				Gdk.Pixbuf p;
				ListStore store;
				if (columns.Length == 2)
					store = new ListStore(typeof(Gdk.Pixbuf), typeof (string));
				else
					store = new ListStore(typeof (string));
				Model = store;
				foreach (FieldInfo info in type.GetFields()) {
					s = info.GetEnumTitle();
					p = (Gdk.Pixbuf) info.GetEnumIcon();
					if (s == "value__")
						continue;
					if (columns.Length == 2)
						store.AppendValues (p, s);
					else
						store.AppendValues (s);
				}
				GetDataFromDataSource (this);
			}
		}

		public DataEnumComboBox()
			: this (null, "")
		{
		}
		
		public DataEnumComboBox (string aMappings)
			: this (null, aMappings)
		{
		}
		
		public DataEnumComboBox (object aDataSource, string aMappings)
			: base()
		{
			Sensitive = false;
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataEnumComboBox()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
