// DataComboBox.cs - DataComboBox implementation for Gtk#Databindings
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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Data.Bindings;
using System.Data.Bindings.Cached;
using System.Data.Bindings.Collections;
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides adapted ComboBox
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Widgets")]
	public class DataComboBox : ComboBox, IAdaptableListControl, IPostableControl, IAdaptableControl
	{
		private ControlAdaptor adaptor = null;
		private MappingsImplementor internalModel = null;
		
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

		private bool cursorPointsEveryType = true;
		/// <summary>
		/// Defines if CurrentSelection adaptor should point every type of object
		/// if false then pointing is limited to defualt type
		/// </summary>
		public bool CursorPointsEveryType {
			get { return (cursorPointsEveryType); }
			set { cursorPointsEveryType = value; } 
		}

		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public object ItemsDataSource {
			get { return (internalModel.ItemsDataSource); }
			set { internalModel.ItemsDataSource = value; }
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
		/// Link to Column Mappings in connected Adaptor 
		/// </summary>
		[Browsable (true), Category ("Data Binding"), Description ("Column Data mappings")]
		public string ColumnMappings { 
			get { return (internalModel.Mappings); }
			set { internalModel.Mappings = value; }
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
		private Adaptor currentSelection = null;
		/// <summary>
		/// Allows controls to bind on the selection in this TreeView
		/// </summary>
		/// <remarks>
		/// DO NOT USE THIS ONE TO SET WHICH ITEM IS SELECTED.
		/// OR AT LEAST NOT YET.
		/// </remarks>
		[Browsable (false), Category ("Data Binding")]
		public Adaptor CurrentSelection {
			get { return (currentSelection); }
		}

		public object ListItems {
			get { return (internalModel.ListItems); }
		}
		
		/// <summary>
		/// Gets activated on every cell to set user parameters on how to draw this cell.
		/// Difference between classic render column is that here arguments passed are not 
		/// TreeIter and TreePath, but rather IList, already resolved object and Path to 
		/// the same object.
		///
		/// Visble should already be set as sane as possible, but this allows fine grained 
		/// setting how to display data
		/// </summary> 
		/// <remarks>
		/// There sholdn't be any need for dispatching additional data, if IMappedColumnItem
		/// type CellRenderers were used, they already provide more than enough by them self.
		/// </remarks>
		public event ListElementCellParamsEvent CellDescription {
			add { internalModel.CellLayoutDescription += value; }
			remove { internalModel.CellLayoutDescription -= value; }
		}
		
		/// <summary>
		/// Reacts on when selected object has being changed from the outside
		/// </summary>
		/// <param name="aSender">
		/// Notification sender object <see cref="System.Object"/>
		/// </param>
/*		private void SelectedObjectChanged (object aSender)
		{
			object obj = CurrentSelection.FinalTarget;
			if (obj == null)
				return;
			TreePath tp;
			TreeViewColumn tv;
			GetCursor (out tp, out tv);
			if (tp == null)
				return;
				
			TreeIter iter;
			if (internalModel.GetIter(out iter, tp) == false)
				return;
			
//			InsertValuesFromObjectToIter (obj, iter);
		}*/
		
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
		}
		
		/// <summary>
		/// Updates parent object to DataSource object
		/// </summary>
		public virtual void PutDataToDataSource (object aSender)
		{
			adaptor.DataChanged = false;
		}
		
		protected override bool OnFocusInEvent (Gdk.EventFocus evnt)
		{
			bool res = base.OnFocusInEvent (evnt);
			OnChanged();
			return (res);
		}

		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			bool res = base.OnFocusOutEvent (evnt);
			OnChanged();
			return (res);
		}
		
		protected override bool OnFocused (DirectionType direction)
		{
			bool res = base.OnFocused (direction);
			OnChanged();
			return (res);
		}
		
		/// <summary>
		/// Overrides OnCursorChanged to handle changes
		/// </summary>
		protected override void OnChanged()
//		protected override void OnCursorChanged()
		{
			base.OnChanged();
			if (Model == null) {
				currentSelection.Target = null;
				return;
			}
			if (currentSelection.Target != null)
				if (ListItems == null) {
					adaptor.DemandInstantPost();
					currentSelection.Target = null;
					return;
				}
				
			TreeIter iter;
//			TreePath tp;

			if (GetActiveIter(out iter) == true) {
				object o = internalModel.NodeFromIter (iter);
//				tp = internalModel.GetPath (iter);
//				if (tp == null)
//					throw new ExceptionResolvingPathFromTreeIterOnChanged (GetType());
				if (ListItems != null) {
//					IList lst = (IList) ListItems;
//					object obj = HierarchicalList.Get (lst, tp.Indices);
					if (TypeValidator.IsCompatible(o.GetType(), internalModel.ListItemType) == true) 
						currentSelection.Target = o;
					else
						currentSelection.Target = null;
					o = null;
//					lst = null;
				}
				else
					currentSelection.Target = null;
			}
			else
				currentSelection.Target = null;

			adaptor.DemandInstantPost();
		}
		
		/// <summary>
		/// Function returns the same result as CurrentSelection, except
		/// CurrentSelection only returns default type or null
		/// </summary>
		public object GetCurrentObject()
		{
			TreeIter iter;
			if (GetActiveIter(out iter) == true)
				return (internalModel.NodeFromIter (iter));
			return (null);
//			if (ListItems == null)
//				return null;
				
/*			TreeIter iter;
			TreePath tp;
			TreeViewColumn tv;
			GetCursor (out tp, out tv);
			if (tv != null) {
				if (internalModel.GetIter (out iter, tp)) {
					object obj = internalModel.NodeFromIter(iter);
					return (obj);
				}
				else
					return (null);
			}
			else
				return (null);*/
		}
		
		/// <summary>
		/// Called when ItemsDataSource changes
		/// </summary>
/*		private void ListTargetChanged (IAdaptor aAdaptor)
		{
			cachedItems = null;
			if (ItemsDataSource == null)
				CurrentSelection.Target = null;
			else {
//				if ((ItemsDataSource is IAdaptor) == false)
//					return;
				object check = ConnectionProvider.ResolveTargetForObject(ItemsDataSource);
				if (check != lastItems) {
					lastItems = check;
					check = ItemsDataSource;
					ItemsDataSource = null;
					ItemsDataSource = check;
				}
			}
			Adaptor.CheckControl();
		}*/
		
		/// <summary>
		/// Creates adaptors associated with this IconView
		/// </summary>
		internal virtual void CreateAdaptors()
		{
			// Allocate selection adaptor
			currentSelection = new GtkAdaptor();
//			currentSelection.OnDataChange += SelectedObjectChanged;
			internalModel.ClearSelection += delegate() {
				if (CurrentSelection != null)
					CurrentSelection.Target = null;
			};
			
			// Create and connect ListAdaptor with this TreeView
/*			listadaptor = new GtkListAdaptor(false, null, this, false); 
			listadaptor.OnListChanged += DSChanged;
			listadaptor.OnElementAdded += DSElementAdded;
			listadaptor.OnElementChanged += DSElementChanged;
			listadaptor.OnElementRemoved += DSElementRemoved;
			listadaptor.OnTargetChange += ListTargetChanged;*/
//			listadaptor.OnTargetChange += ItemsTargetChanged;
			// Create Adaptor
			adaptor = new GtkControlAdaptor (this, false);
			adaptor.DisableMappingsDataTransfer = true;

			internalModel.CheckControl += delegate() {
				if (adaptor != null)
					adaptor.CheckControl();
			};			
//			columnadaptor = new GtkAdaptor();
		}

		/// <summary>
		/// Disconnects everything inside this class
		/// </summary>
		public virtual void Disconnect()
		{
/*			lastItems = null;
			UnsetDragFunctionality();
			onListCellDescription = null;
			if (listadaptor != null) {
				listadaptor.OnListChanged -= DSChanged;
				listadaptor.OnElementAdded -= DSElementAdded;
				listadaptor.OnElementChanged -= DSElementChanged;
				listadaptor.OnElementRemoved -= DSElementRemoved;
				listadaptor.OnTargetChange -= ListTargetChanged;
				listadaptor.Disconnect();
				listadaptor = null;
			}*/
			if (CurrentSelection != null) {
				CurrentSelection.Disconnect();
				currentSelection = null;
			}
			if (adaptor != null) {
				adaptor.Disconnect();
				adaptor = null;
			}
			internalModel.Disconnect();
			internalModel = null;
//			cachedItems = null;
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		public DataComboBox()
			: base()
		{
			internalModel = new MappingsImplementor (this);
			CreateAdaptors();
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataComboBox (string aMappings)
			: base()
		{
			internalModel = new MappingsImplementor (this);
			CreateAdaptors();
			Mappings = aMappings;
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
		public DataComboBox (object aDataSource, string aMappings)
			: base()
		{
			internalModel = new MappingsImplementor (this);
			CreateAdaptors();
			DataSource = aDataSource;
			Mappings = aMappings;
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataComboBox()
		{
			Disconnect();
		}
	}
}
