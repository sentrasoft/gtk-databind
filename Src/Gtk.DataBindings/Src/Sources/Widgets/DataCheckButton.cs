// DataCheckButton.cs - DataCheckButton implementation for Gtk#Databindings
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
using Gdk;

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
	[GtkWidgetFactoryProvider ("bool", "DefaultFactoryCreate")]
	[GtkTypeWidgetFactoryProvider ("boolhandler", "DefaultFactoryCreate", typeof(bool))]
	public class DataCheckButton : CheckButton, IAdaptableControl, IPostableControl, IEditable, IAutomaticTitle
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
			IAdaptableControl wdg = new DataCheckButton();
			wdg.Mappings = aArgs.PropertyName;
			if (aArgs.State == PropertyDefinition.ReadOnly)
				(wdg as IEditable).Editable = false;
			(wdg as DataCheckButton).AutomaticTitle = aArgs.ResolveTitle;
			if ((wdg as DataCheckButton).AutomaticTitle == false)
				(wdg as DataCheckButton).Label = aArgs.Title;
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
			set { adaptor.InheritedDataSource = value; }
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false)]
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

		private bool editable = true;
		/// <value>
		/// Specifies if widget is editable or not
		/// </value>
		public bool Editable {
			get { return (editable); }
			set {
				if (editable == value)
					return;
				editable = value;
			}
		}
		
		private bool automaticTitle = false;
		/// <value>
		/// Specifies if title should be automaticaly resolved trough
		/// attribute processing based on mapping
		/// </value>
		public bool AutomaticTitle {
			get { return (automaticTitle); }
			set {
				if (automaticTitle == value)
					return;
				automaticTitle = value;
				if (automaticTitle == true)
					_TargetChanged (null);
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
			if ((bool) System.Convert.ToBoolean(adaptor.Value) != Active)
				Active = (bool) adaptor.Value;
		}

		/// <summary>
		/// Updates parent object to DataSource object
		/// </summary>
		public virtual void PutDataToDataSource (object aSender)
		{
			adaptor.DataChanged = false;
			if ((bool) adaptor.Value != Active)
				adaptor.Value = Active;
		}
		
		/// <summary>
		/// Overrides OnToggled to put data into object if needed
		/// </summary>
		protected override void OnToggled()
		{
			base.OnToggled();
			adaptor.DemandInstantPost();
		}
		
		[GLib.ConnectBefore]
		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			if (Editable == false)
				return (true);
			return base.OnButtonPressEvent (evnt);
		}

		[GLib.ConnectBefore]
		protected override bool OnButtonReleaseEvent (EventButton evnt)
		{
			if (Editable == false)
				return (true);
			return base.OnButtonReleaseEvent (evnt);
		}

		[GLib.ConnectBefore]
		protected override bool OnKeyPressEvent (EventKey evnt)
		{
			if (Editable == false)
				return (true);
			return base.OnKeyPressEvent (evnt);
		}

		[GLib.ConnectBefore]
		protected override bool OnKeyReleaseEvent (EventKey evnt)
		{
			if (Editable == false)
				return (true);
			return base.OnKeyReleaseEvent (evnt);
		}

		private void _TargetChanged (IAdaptor aAdaptor)
		{
			Gtk.Application.Invoke (delegate {
				if ((adaptor.Adaptor.FinalTarget == null) ||
				    (adaptor.Values.Count == 0) || 
				    (adaptor.Values[0].Name == "")) {
					string cachedDefault = "";
					if (adaptor.Adaptor.DataSourceType != null)
						cachedDefault = adaptor.Values[0].ResolveTitle (adaptor.Adaptor.DataSourceType);
					Label = cachedDefault;
					return;
				}
				Label = adaptor.Values[0].Title;
			});
		}
		
		/// <summary>
		/// Initializes internals
		/// </summary>
		protected void InitializeInternals()
		{
			adaptor.Adaptor.TargetChanged += _TargetChanged;
		}
		
		/// <summary>
		/// Creates DataCheckButton 
		/// </summary>
		public DataCheckButton()
			: base()
		{
			adaptor = new GtkControlAdaptor (this, true);
			InitializeInternals();
		}
		
		/// <summary>
		/// Creates DataCheckButton 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataCheckButton (string aMappings)
			: base()
		{
			adaptor = new GtkControlAdaptor (this, true, aMappings);
			InitializeInternals();
		}
		
		/// <summary>
		/// Creates DataCheckButton 
		/// </summary>
		/// <param name="aDataSource">
		/// DataSource connected to this widget <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataCheckButton (object aDataSource, string aMappings)
			: base()
		{
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
			InitializeInternals();
		}
		
		/// <summary>
		/// Destroys and disconnects DataCheckBox
		/// </summary>
		~DataCheckButton()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
