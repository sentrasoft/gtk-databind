// DataSpinButton.cs - DataSpinButton implementation for Gtk#Databindings
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
	/// SpinButton control connected to adaptor and direct updating if connected object
	/// supports IChangeable
	///
	/// Supports single mapping
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	[GtkWidgetFactoryProvider ("number", "DefaultFactoryCreate")]
	[GtkTypeWidgetFactoryProvider ("inthandler", "DefaultFactoryCreate", typeof(int))]
	[GtkTypeWidgetFactoryProvider ("floathandler", "DefaultFactoryCreate", typeof(float))]
	[GtkTypeWidgetFactoryProvider ("doublehandler", "DefaultFactoryCreate", typeof(double))]
	[GtkTypeWidgetFactoryProvider ("bytehandler", "DefaultFactoryCreate", typeof(byte))]
	public class DataSpinButton : SpinButton, IAdaptableControl, IPostableControl
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
				wdg = new DataSpinButton(0, 100, 1);
			wdg.Mappings = aArgs.PropertyName;
			return (wdg);
		}
		
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
			get { return (adaptor.DataSource); }
			set { adaptor.DataSource = value; }
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data mappings")]
		public string Mappings { 
			get { return (adaptor.Mappings); }
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
			object val = adaptor.Value;
			double ival = (double) System.Convert.ToDouble(val); 
			if (ival != Value)
				Value = ival;
			val = null;
		}
		
		/// <summary>
		/// Updates parent object to DataSource object
		/// </summary>
		public virtual void PutDataToDataSource (object aSender)
		{
			adaptor.DataChanged = false;
			adaptor.Value = (object) Value;
		}
		
		/// <summary>
		/// Overrides OnValueChanged to put data in DataSource if needed
		/// </summary>
		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			adaptor.DemandInstantPost();
		}
		
		/// <summary>
		/// Creates Widget
		/// </summary>
		/// <param name="aMin">
		/// Minimum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMax">
		/// Maximum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aStep">
		/// Step value for scale <see cref="System.Int32"/>
		/// </param>
		public DataSpinButton (int aMin, int aMax, int aStep)
			: base(aMin, aMax, aStep)
		{
			adaptor = new GtkControlAdaptor (this, true);
		}
		
		/// <summary>
		/// Creates Widget
		/// </summary>
		/// <param name="aMin">
		/// Minimum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMax">
		/// Maximum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aStep">
		/// Step value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataSpinButton (int aMin, int aMax, int aStep, string aMappings)
			: base(aMin, aMax, aStep)
		{
			adaptor = new GtkControlAdaptor (this, true, aMappings);
		}
		
		/// <summary>
		/// Creates Widget
		/// </summary>
		/// <param name="aMin">
		/// Minimum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMax">
		/// Maximum value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aStep">
		/// Step value for scale <see cref="System.Int32"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this widget <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataSpinButton (int aMin, int aMax, int aStep, object aDataSource, string aMappings)
			: base(aMin, aMax, aStep)
		{
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataSpinButton()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
