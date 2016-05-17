//Interfaces.cs - Description
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) 2008 m.
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of version 2 of the Lesser GNU General 
//Public License as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA 02111-1307, USA.
//
//

using System;
using System.Collections;
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Specifies delegate method for ModeButton events
	/// </summary>
	public delegate void ModeButtonEventHandler (object sender, ModeButtonEventArgs args);

	/// <summary>
	/// ModeButton event arguments
	/// </summary>
	public class ModeButtonEventArgs: EventArgs 
	{
		private Widget widget;
		/// <value>
		/// Widget concerned in event
		/// </value>
		public Widget Widget {
			get { return (widget); }
		}

		private int index;
		/// <value>
		/// Widget index
		/// </value>
		public int Index {
			get { return (index); }
		}

		public ModeButtonEventArgs (int aIndex, Widget aWidget)
		{
			widget = aWidget;
			index = aIndex;
		}
	}

	/// <summary>
	/// Specifies item state
	/// </summary>
	public enum EnumItemState
	{
		/// <summary>
		/// Normal state
		/// </summary>
		Normal,
		/// <summary>
		/// Selected
		/// </summary>
		Selected,
		/// <summary>
		/// Mouse over
		/// </summary>
		Preflight
	}
	
	/// <summary>
	/// Specifies how Enum item should display it self
	/// </summary>
	public enum EnumItemDisplayMode
	{
		/// <summary>
		/// Icons only
		/// </summary>
		IconsOnly,
		/// <summary>
		/// Text only
		/// </summary>
		TextOnly,
		/// <summary>
		/// Text and icon
		/// </summary>
		TextAndIcon
	}
	
	/// <summary>
	/// Specifies layout for Enum item HComboCell widget
	/// </summary>
	public enum EnumItemLayout
	{
		/// <summary>
		/// Horizontal
		/// </summary>
		Horizontal,
		/// <summary>
		/// Vertical
		/// </summary>
		Vertical
	}
	
	/// <summary>
	/// Event sent on date changed
	/// </summary>
	public delegate void DateChangedEvent (object aSender, DateChangedEventArgs aArgs);

	/// <summary>
	/// Arguments passed on date changed event
	/// </summary>
	public class DateChangedEventArgs : EventArgs
	{
		private DateTime date = DateTime.Now;
		/// <value>
		/// Date
		/// </value>
		public DateTime Date {
			get { return (date); }
			set { date = value; }
		}
		
		public DateChangedEventArgs (DateTime aDate)
		{
			date = aDate;
		}
	}
	
	/// <summary>
	/// Event sent to notifiy tree/list widget is needed to clear selection
	/// </summary>
	public delegate void CheckControlEvent();
	
	/// <summary>
	/// Event sent to notifiy tree/list widget is needed to clear selection
	/// </summary>
	public delegate void ClearSelectionEvent();
	
	/// <summary>
	/// Event sent to notifiy tree/list widget is needed to clear model
	/// </summary>
	public delegate void ResetModelEvent();
	
	/// <summary>
	/// Event sent to notifiy widget is needed to clear column information
	/// </summary>
	public delegate void ClearColumnsEvent();
	
	/// <summary>
	/// Event for decribing every cell in ComBox or TreeView. Difference is
	/// that this one passes IList and object instead of TreeViewModel
	/// </summary>
	/// <param name="aList">
	/// List object resides in <see cref="IList"/>
	/// </param>
	/// <param name="aPath">
	/// Path to access object <see cref="System.Int32"/>
	/// </param>
	/// <param name="aObject">
	/// Object to be drawn <see cref="System.Object"/>
	/// </param>
	/// <param name="aCell">
	/// CellRenderer to be used to draw <see cref="Gtk.CellRenderer"/>
	/// </param>
	public delegate void ListElementCellParams (IList aList, int[] aPath, object aObject, Gtk.CellRenderer aCell);

	/// <summary>
	/// Event for decribing every cell in ComBox or TreeView. Difference is
	/// that this one passes IList and object instead of TreeViewModel
	/// </summary>
	/// <param name="aList">
	/// List object resides in <see cref="System.Object"/>
	/// </param>
	/// <param name="aPath">
	/// Path to access object <see cref="System.Int32"/>
	/// </param>
	/// <param name="aObject">
	/// Object to be drawn <see cref="System.Object"/>
	/// </param>
	/// <param name="aCell">
	/// CellRenderer to be used to draw <see cref="Gtk.CellRenderer"/>
	/// </param>
	public delegate void ListElementCellParamsEvent (object aList, int[] aPath, object aObject, Gtk.CellRenderer aCell);

	/// <summary>
	/// Event for decribing every cell in ComBox or TreeView. Difference is
	/// that this one passes object instead of TreeViewModel
	/// </summary>
	/// <param name="aColumn">
	/// Object that sent message <see cref="Gtk.TreeViewColumn"/>
	/// </param>
	/// <param name="aObject">
	/// Object to be drawn <see cref="System.Object"/>
	/// </param>
	/// <param name="aCell">
	/// CellRenderer to be used to draw <see cref="Gtk.CellRenderer"/>
	/// </param>
	public delegate void CellDescriptionEvent (Gtk.TreeViewColumn aColumn, object aObject, Gtk.CellRenderer aCell);

	/// <summary>
	/// Defines type of action which is monitored
	/// </summary>
	public enum ActionMonitorType
	{
		/// <summary>
		/// Monitores visibility
		/// </summary>
		Visibility,
		/// <summary>
		/// Monitores sensitivity
		/// </summary>
		Sensitivity,
		/// <summary>
		/// Monitores visibility with inverted value
		/// </summary>
		InvertedVisibility,
		/// <summary>
		/// Monitores sensitivity with inverted value
		/// </summary>
		InvertedSensitivity
	}
	
	/// <summary>
	/// Defines ActionMonitor defaults
	/// </summary>
	public enum ActionMonitorDefaultsType
	{
		/// <summary>
		/// true when DataSource Target is not null
		/// </summary>
		NotNullTarget,
		/// <summary>
		/// Will be possible to be true only if DataSource is valid (is null)
		/// </summary>
		NeedsValid,
		/// <summary>
		/// Will be possible to be true even if DataSource is not valid (is null)
		/// </summary>
		Always
	}
	
	/// <summary>
	/// Declares description of action monitor
	/// </summary>
	public struct ActionMonitor
	{
		private WeakReference reference;
		
		private ActionMonitorType monitorType;
		/// <summary>
		/// Declares Action monitor type
		/// </summary>
		public ActionMonitorType MonitorType {
			get { return (monitorType); }
			set { monitorType = value; } 
		}
		
		/// <summary>
		/// Returns action which is monitored
		/// </summary>
		public Gtk.Action Action {
			get { return ((Gtk.Action) reference.Target); }
		}

		/// <summary>
		/// Returns if ActionMonitor is valid
		/// </summary>
		public bool IsValid {
			get { return (reference.Target != null); }
		}

		private ActionMonitorDefaults defaults;
		/// <summary>
		/// Defaults for handling when DataSource is null
		/// </summary>
		public ActionMonitorDefaults Defaults {
			get { return (defaults); }
		}
		
		/// <summary>
		/// Creates ActionMonitor
		/// </summary>
		/// <param name="aType">
		/// Type of monitoring <see cref="ActionMonitorType"/>
		/// </param>
		/// <param name="aAction">
		/// Action which is monitored <see cref="Gtk.Action"/>
		/// </param>
		/// <param name="aDefaults">
		/// Defaults how to resolve when DataSource is null <see cref="ActionMonitorDefaults"/>
		/// </param>
		public ActionMonitor (ActionMonitorType aType, Gtk.Action aAction, ActionMonitorDefaults aDefaults)
		{
			reference = new WeakReference (aAction);
			monitorType = aType;
//			reference.Target = aAction;
			defaults = aDefaults;
		}
		
		/// <summary>
		/// Creates ActionMonitor
		/// </summary>
		/// <param name="aType">
		/// Type of monitoring <see cref="ActionMonitorType"/>
		/// </param>
		/// <param name="aAction">
		/// Action which is monitored <see cref="Gtk.Action"/>
		/// </param>
		public ActionMonitor (ActionMonitorType aType, Gtk.Action aAction)
		{
			reference = new WeakReference (aAction);
			monitorType = aType;
//			reference.Target = aAction;
			defaults = new ActionMonitorDefaults (ActionMonitorDefaultsType.Always, false);
		}
	}

	/// <summary>
	/// Controls default value when DataSource can't be accessed
	/// </summary>
	public struct ActionMonitorDefaults
	{
		private ActionMonitorDefaultsType mode;
		/// <summary>
		/// Default mode
		/// </summary>
		public ActionMonitorDefaultsType Mode {
			get { return (mode); }
		}
		
		private bool defaultValue;
		/// <summary>
		/// Value which will only be set if Mode is NeedsValid
		/// </summary>
		public bool DefaultValue {
			get { return (defaultValue); }
		}
		
		/// <summary>
		/// Creates ActionMonitorDefaults
		/// </summary>
		/// <param name="aMode">
		/// Mode for default setting <see cref="ActionMonitorDefaultsType"/>
		/// </param>
		public ActionMonitorDefaults (ActionMonitorDefaultsType aMode)
		{
			if ((aMode == ActionMonitorDefaultsType.Always) ||
			    (aMode == ActionMonitorDefaultsType.NotNullTarget))
				mode = aMode;
			else
				throw new Exception ("ActionMonitorDefaults activated abnormally");
			defaultValue = true;
		}

		/// <summary>
		/// Creates ActionMonitorDefaults
		/// </summary>
		/// <param name="aMode">
		/// Mode for default setting <see cref="ActionMonitorDefaultsType"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Value which needs to be set if DataSource is null and mode is NeedsValid <see cref="System.Boolean"/>
		/// </param>
		public ActionMonitorDefaults (ActionMonitorDefaultsType aMode, bool aDefaultValue)
		{
			mode = aMode;
			defaultValue = aDefaultValue;
		}
	}

	/// <summary>
	/// Defines how toolbars should behave
	/// </summary>
	public enum ToolbarConstriction
	{
		/// <summary>
		/// When set with this, toolbars will respect system toolbar settings
		/// </summary>
		SystemDefaults,
		/// <summary>
		/// When set with this, toolbars will be able to change according to its own settings
		/// </summary>
		ApplicationDefaults
	}
}
