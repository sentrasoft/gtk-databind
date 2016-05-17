// DataWidgetLabel.cs - DataLabel implementation for Gtk#Databindings
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
	public class DataTitleLabel : Label, IAdaptableControl, ILayoutWidget
	{
		private string cachedDefault = "";
		
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

		private bool important = false;
		/// <value>
		/// Draws title as bolded
		/// </value>
		public bool Important {
			get { return (important); }
			set { important = value; }
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
			if ((adaptor.Adaptor.FinalTarget == null) ||
			    (adaptor.Values.Count == 0) || 
			    (adaptor.Values[0].Name == "")) {
				cachedDefault = "";
				if (adaptor.Adaptor.DataSourceType != null)
					cachedDefault = adaptor.Values[0].ResolveTitle (adaptor.Adaptor.DataSourceType);
				if (Important == true) 
					Markup = "<b>" + cachedDefault + "</b>";
				else
					Text = cachedDefault;
				return;
			}
			if (Important == true) 
				Markup = "<b>" + adaptor.Values[0].Title + "</b>";
			else
				Text = adaptor.Values[0].Title;
		}

		/// <summary>
		/// Internal method which handles target change and sets new title
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor <see cref="IAdaptor"/>
		/// </param>
		private void _TargetChanged (IAdaptor aAdaptor)
		{
			Gtk.Application.Invoke (delegate {
				GetDataFromDataSource (null);
			});
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		public DataTitleLabel()
			: base("")
		{
			Xalign = 0;
			adaptor = new GtkControlAdaptor (this, true);
			adaptor.Adaptor.TargetChanged += _TargetChanged;
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataTitleLabel (string aMappings)
			: base("")
		{
			Xalign = 0;
			adaptor = new GtkControlAdaptor (this, true, aMappings);
			adaptor.Adaptor.TargetChanged += _TargetChanged;
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
		public DataTitleLabel (object aDataSource, string aMappings)
			: base("")
		{
			Xalign = 0;
			adaptor = new GtkControlAdaptor (this, false, aDataSource, aMappings);
			adaptor.Adaptor.TargetChanged += _TargetChanged;
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataTitleLabel()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
