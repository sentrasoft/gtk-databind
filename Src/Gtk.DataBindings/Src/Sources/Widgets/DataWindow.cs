// DataWindow.cs - DataWindow implementation for Gtk#Databindings
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
//	[ToolboxItem ("DataWindow")]
//	[Description ("Adaptable Window")]
//	[Category ("window")]
	public class DataWindow : Window, IAdaptableContainer, IContainerControl
	{
		private ControlAdaptor adaptor = null;
		private bool destroying = false;
		
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
			set { adaptor.InheritedDataSource = false; }
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
		/// Notification method activated from Adaptor 
		/// </summary>
		/// <param name="aSender">
		/// Object that made change <see cref="System.Object"/>
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
		/// Notifies adaptors on closing this window
		/// </summary>
		/// <param name="o">
		/// Same as OnDelete <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// Same as OnDelete <see cref="AddedArgs"/>
		/// </param>
		private void StopAdaptorsOnDelete (object o, DeleteEventArgs a)
		{
			destroying = true;
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
		
		/// <summary>
		/// Creates Window
		/// </summary>
		/// <param name="aTitle">
		/// Title of this window <see cref="System.String"/>
		/// </param>
		public DataWindow (string aTitle)
			: base (aTitle)
		{
			adaptor = new GtkControlAdaptor (this, true);
			Added += NotifyAdaptorsOnAdd;
		}
		
		/// <summary>
		/// Creates Window
		/// </summary>
		/// <param name="aTitle">
		/// Title of this window <see cref="System.String"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this container <see cref="System.Object"/>
		/// </param>
		public DataWindow (string aTitle, object aDataSource)
			: base (aTitle)
		{
			adaptor = new GtkControlAdaptor (this, true, aDataSource, "");
			Added += NotifyAdaptorsOnAdd;
		}
		
		/// <summary>
		/// Disconnects and destroys Window
		/// </summary>
		~DataWindow()
		{
			Added -= NotifyAdaptorsOnAdd;
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
