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

namespace Gtk.DataBindings
{
	/// <summary>
	/// Event box which auto creates widget trough factory based on momentary property or
	/// default property based on specified type
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	public class DataAutoWidget : EventBox, IAdaptableContainer, IContainerControl
	{
		private bool destroying = false;
		private IAdaptor internalAdaptor = new Adaptor();
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
		[Browsable (false), Category ("Data Binding"), Description ("Data Source")]
		public object DataSource {
			get { return (adaptor.DataSource); }
			set { adaptor.DataSource = value; }
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
		/// Updates parent object to DataSource object
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public virtual void GetDataFromDataSource (object aSender)
		{
		}
		
		/// <summary>
		/// Notifies adaptors on adding widget to this container
		/// </summary>
		/// <param name="o">
		/// Widget added to this container <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// Added arguments <see cref="AddedArgs"/>
		/// </param>
		private void NotifyAdaptorsOnAdd (object o, AddedArgs a)
		{
			Adaptor.SendAdaptorMessage ((object) a.Widget, EActionType.RenewParents);
		}
		
		/// <summary>
		/// Dummy resolver for container controls
		/// </summary>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		public bool IsContainer()
		{
			return (true);
		}
				
		private IAdaptableControl dataWidget = null;
		/// <value>
		/// Provides access to widget handling the data
		/// </value>
		public IAdaptableControl DataWidget {
			get { return (dataWidget); }
			protected set { 
				if (adaptor == null)
					return;
				if (adaptor.DestroyInProgress == true)
					return;
				if (dataWidget != null) {
					(dataWidget as Gtk.Widget).Hide();
					dataWidget.InheritedDataSource = false;
					dataWidget.DataSource = null;
					Remove (dataWidget as Gtk.Widget);
					(dataWidget as Gtk.Widget).Destroy();
					dataWidget = null;
				}
				if (destroying == true) {
					dataWidget = null;
					return;
				}
				dataWidget = value;
				if (dataWidget != null) {
					(dataWidget as Gtk.Widget).Show();
					Add (dataWidget as Gtk.Widget);
					dataWidget.InheritedDataSource = true;
					dataWidget.Mappings = internalAdaptor.Mappings;
				}
			}
		}
		
		/// <value>
		/// Specifies mappings
		/// </value>
		[Category ("Data Binding"), Description ("Data Mappings")]
		public string Mappings {
			get { 
				if (dataWidget != null)
					return (dataWidget.Mappings);
				return (internalAdaptor.Mappings); 
			}
			set {
				if (internalAdaptor.Mappings == value)
					return;
				internalAdaptor.Mappings = value;
				SetNullWidget();
				HandleTargetChanged (Adaptor.Adaptor);
			}
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
				SetNullWidget();
				HandleTargetChanged (Adaptor.Adaptor);
			}
		}
		
		private string widgetFilter = "";
		public string WidgetFilter {
			get { return (widgetFilter); }
			set {
				if (widgetFilter == value)
					return;
				widgetFilter = value;
			}
		}
		
		/// <summary>
		/// Destroys widget
		/// </summary>
		private void SetNullWidget()
		{
			DataWidget = null;
		}
		
		/// <summary>
		/// Handles event whenever target changes
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor <see cref="IAdaptor"/>
		/// </param>
		private void HandleTargetChanged (IAdaptor aAdaptor)
		{
			if (adaptor == null)
				return;
			if (adaptor.DestroyInProgress == true)
				return;
			if ((internalAdaptor.Mappings == "") || (internalAdaptor.Mapping(0) == null)) {
				DataWidget = null;
				return;
			}
			System.Type type = null;
			if (Adaptor.Adaptor.FinalTarget == null) {
				if (Adaptor.Adaptor.DataSourceType == null) {
					DataWidget = null;
					return;
				}
				type = Adaptor.Adaptor.DataSourceType;
			}
			else
				type = Adaptor.Adaptor.FinalTarget.GetType();

			FactoryInvocationArgs args;
			PropertyDefinition def = internalAdaptor.Mapping(0).OriginalRWFlags.GetPropertyDefinition();
			if (Editable == false)
				def = PropertyDefinition.ReadOnly;
			args = new GtkFactoryInvocationArgs (def, type, internalAdaptor.Mapping(0).Name);
			args.AddDefaultTheme();
			if ((WidgetFilter.Trim() != "") && (WidgetFilter.Trim().ToLower() != "gtk"))
				args.AddFilter (WidgetFilter);
			DataWidget = GtkWidgetFactory.CreateWidget (args);
		}

		public DataAutoWidget()
			: base()
		{			
			adaptor = new GtkControlAdaptor (this, true);
			Added += NotifyAdaptorsOnAdd;
			Adaptor.Adaptor.TargetChanged += HandleTargetChanged;
		}
		
		/// <summary>
		/// Creates Container
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this container <see cref="System.String"/>
		/// </param>
		public DataAutoWidget (string aMappings)
			: this()
		{
			Mappings = aMappings;
		}

		/// <summary>
		/// Creates Container
		/// </summary>
		/// <param name="aDataSource">
		/// DataSource connected to this container <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this container <see cref="System.String"/>
		/// </param>
		public DataAutoWidget (object aDataSource, string aMappings)
			: this (aMappings)
		{
			DataSource = aDataSource;
		}
		
		~DataAutoWidget()
		{
			adaptor.Adaptor.TargetChanged -= HandleTargetChanged;
			Added -= NotifyAdaptorsOnAdd;
			if (InheritedDataSource != false) {
				InheritedDataSource = false;
				DataSource = null;
			}
			destroying = true;				
			//DataWidget = null;
			adaptor.Disconnect();
			adaptor = null;
			internalAdaptor.Disconnect();
		}
	}
}
