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
using System.Data.Bindings;
using System.Reflection;
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	[GtkWidgetFactoryProvider ("enumgroup", "DefaultFactoryCreate")]
	public partial class DataRadioGroup : Gtk.Bin, IAdaptableControl, IPostableControl
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
				wdg = new DataRadioGroup();
			wdg.Mappings = aArgs.PropertyName;
			return (wdg);
		}
		
		private System.Type lastType = null;
		private List<RadioButton> options = new List<RadioButton>();
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
			if (options.Count == 0)
				return;
			adaptor.DataChanged = false;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == false)
					throw new NotSupportedException ("DataRadioGroup only supports enum types as mapping");
				Array enumValues = Enum.GetValues (type);
				for (int i=0; i<enumValues.Length; i++)
					if (enumValues.GetValue(i).Equals(adaptor.Value))
						options[i].Active = true;
			}
		}
		
		/// <summary>
		/// Updates parent object to DataSource object
		/// </summary>
		public virtual void PutDataToDataSource (object aSender)
		{
			if (options.Count == 0)
				return;
			adaptor.DataChanged = false;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == false)
					throw new NotSupportedException ("DataRadioGroup only supports enum types as mapping");
				Array enumValues = Enum.GetValues (type);
				for (int i=0; i<enumValues.Length; i++)
					if (options[i].Active == true)
						adaptor.Value = enumValues.GetValue(i);
			}
		}
		
		private event EventHandler toggled = null;
		/// <summary>
		/// Event triggered whenever child radio is toggled
		/// </summary>
		public event EventHandler Toggled {
			add { toggled += value; }
			remove { toggled -= value; }
		}
		
		/// <summary>
		/// Overrides OnToggled to put data in DataSource if needed
		/// </summary>
		protected virtual void OnToggled()
		{
			adaptor.DemandInstantPost();
			if (toggled != null)
				toggled (this, new EventArgs());
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
			while (options.Count > 0) {
				lastType = null;
				options[options.Count-1].Toggled -= HandleToggled;
				options[options.Count-1].Hide();
				box.Remove (options[options.Count-1]);
				options[options.Count-1].Destroy();
				options.RemoveAt (options.Count-1);
			}
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				lastType = type;
				if (type.IsEnum == false)
					throw new NotSupportedException (string.Format("DataRadioGroup only supports enum types as mapping, specified was {0}", type));
				RadioButton rb = null;
				string s;
				foreach (FieldInfo info in type.GetFields()) {
					s = info.GetEnumTitle();
					if (s == "value__")
						continue;
					if (options.Count == 0)
						rb = new RadioButton (s);
					else
						rb = new RadioButton (options[0], info.GetEnumTitle());
					rb.TooltipMarkup = info.GetEnumHint();
					rb.Show();
					box.PackEnd (rb);
					options.Add (rb);
					rb.Toggled += HandleToggled;
				}
				GetDataFromDataSource (this);
			}
		}

		/// <summary>
		/// Handles radio toggle event
		/// </summary>
		/// <param name="aSender">
		/// Event sender <see cref="System.Object"/>
		/// </param>
		/// <param name="aArgs">
		/// Arguments <see cref="EventArgs"/>
		/// </param>
		private void HandleToggled (object aSender, EventArgs aArgs)
		{
			OnToggled();
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		public DataRadioGroup()
			: this (null, "")
		{
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataRadioGroup (string aMappings)
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
		public DataRadioGroup (object aDataSource, string aMappings)
			: base ()
		{
			this.Build();
			RadioGroup.Hide();
			box.Remove (RadioGroup);
			RadioGroup = null;
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataRadioGroup()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
