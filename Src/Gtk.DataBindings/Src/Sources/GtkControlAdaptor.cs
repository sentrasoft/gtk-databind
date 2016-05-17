//GtkControlAdaptor.cs - Description
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
using System.Data.Bindings;
using System.Data.Bindings.DebugInformation;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides DataBindings ControlAdaptor functionality according to Gtk
	/// </summary>
	public class GtkControlAdaptor : ControlAdaptor
	{
		bool shown = false;
		
		/// <summary>
		/// Validates type of the control if it is compatible with adaptor
		/// </summary>
		/// <param name="aControl">
		/// Control to validate type for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is right type, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool ValidateControlType (object aControl)
		{
			return (aControl is Gtk.Widget);
		}

		/// <summary>
		/// Checks if control is Window type
		/// </summary>
		/// <param name="aControl">
		/// Control to check for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is window, false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool ControlIsWindow (object aControl)
		{
			if (aControl == null)
				return (false);
			return (TypeValidator.IsCompatible(aControl.GetType(), typeof(Gtk.Window)));
		}
		
		/// <summary>
		/// Resolves parent control
		/// </summary>
		/// <param name="aControl">
		/// Control whos parent should be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent control, null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		public override object GetParentOfControl (object aControl)
		{
			if (aControl == null)
				return (null);
			return ((aControl as Gtk.Widget).Parent);
		}
		
		/// <summary>
		/// Resolves parent control
		/// </summary>
		/// <param name="aControl">
		/// Control whos parent should be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent control, null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		public  object GetLowestParentOfControl (object aControl)
		{
			if (aControl == null)
				return (null);
			if ((aControl as Gtk.Widget).Parent == null)
				return (aControl);
			else
				return (GetLowestParentOfControl((aControl as Gtk.Widget).Parent));
		}
		
		/// <summary>
		/// Returns ParentWindow for the specified control
		/// </summary>
		/// <param name="aControl">
		/// Control whose parent we need <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// reference to parent window <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Returns widget even if it is not Window. If specified use demands
		/// Window type first thing needed after this is validation with
		/// ControlIsWindow <see cref="GtkAdaptor.ControlIsWindow"/>
		/// </remarks>
		public override object ParentWindow (object aControl)
		{
			if ((aControl == null) || (ValidateControlType(aControl) == false))
				return (null);
			if (TypeValidator.IsCompatible(aControl.GetType(), typeof(Gtk.Window)) == true)
				return (aControl);
			return ((aControl as Gtk.Widget).Toplevel);
		}
		
		/// <summary>
		/// Checks if control is Box type. This method 
		/// needs to be overriden in ControlAdaptor subclasses
		/// </summary>
		/// <param name="aControl">
		/// Control to check for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is window, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws exception
		/// </remarks>
		public override bool ControlIsContainer (object aControl)
		{
			if (aControl == null)
				return (false);
			return (TypeValidator.IsCompatible(aControl.GetType(), typeof(Gtk.Box)));
		}
		
		/// <summary>
		/// Sets control sensitivity on or off
		/// </summary>
		/// <param name="aControl">
		/// Control to set sensitivity for <see cref="System.Object"/>
		/// </param>
		/// <param name="aSensitive">
		/// New sensitivity value <see cref="System.Boolean"/>
		/// </param>
		public override void SetControlSensitivity (object aControl, bool aSensitive)
		{
			if (ValidateControlType(aControl) == true) {
				if (ControlIsContainer(aControl) == true)
					return;
				if ((aControl is ILayoutWidget) == true)
					return;
				if (((aControl as Gtk.Widget).Visible == false) || (shown == false))
					return;
				//DebugConnection(Control);
				Gtk.Application.Invoke (delegate {
					(aControl as Gtk.Widget).Sensitive = aSensitive;
				});
			}
		}
		
		/// <summary>
		/// Calls controls AdapteeDataChange, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call AdapteeDataChanged <see cref="IChangeableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		public override void InvokeControlAdapteeDataChange (IChangeableControl aControl, object aSender)
		{
			Gtk.Application.Invoke (delegate {
				foreach (MappedProperty prop in Values)
					if (prop.Valid == false) {
						SetControlSensitivity(aControl, false);
						return;
					}
				base.InvokeControlAdapteeDataChange (aControl, aSender);
				SetControlSensitivity(aControl, true);
			});
		}
		
		/// <summary>
		/// Valid for complex controls like TreeView
		/// </summary>
/*		public override void ClearBeforeRemapping()
		{
			Gtk.Application.Invoke (delegate {
				base.ClearBeforeRemapping();
			});
		}
		
		/// <summary>
		/// Valid for complex controls like TreeView
		/// </summary>
		public override void RemapControl()
		{
			Gtk.Application.Invoke (delegate {
				base.RemapControl();
			});
		}*/
		
		/// <summary>
		/// Calls controls GetBoundaryValuesFromDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call GetDataFromDataSource <see cref="IBoundedContainer"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		protected override void InvokeControlBoundaryDataChange (IBoundedContainer aControl, object aSender)
		{
			Gtk.Application.Invoke (delegate {
				base.InvokeControlBoundaryDataChange (aControl, aSender);
			});
		}

		/// <summary>
		/// Connects base control events like gaining or loosing focus etc.
		/// 
		/// ControlAdaptor subclasses should override this method to connect
		/// to specific widget set events 
		/// </summary>
		protected override void ConnectControlEvents()
		{
			if (Control == null)
				return;
			if (TypeValidator.IsCompatible(Control.GetType(), typeof(Gtk.Widget)) == true) {
				(Control as Gtk.Widget).LeaveNotifyEvent += OnLeaveNotifyEvent;
				(Control as Gtk.Widget).ExposeEvent += HandleExposeEvent;
			}
		}

		void HandleExposeEvent(object o, ExposeEventArgs args)
		{
			shown = true;
			(Control as Gtk.Widget).ExposeEvent -= HandleExposeEvent;
			CheckControlState();
		}
		
		/// <summary>
		/// Disconnects base control events like gaining or loosing focus etc.
		/// 
		/// ControlAdaptor subclasses should override this method to connect
		/// to specific widget set events 
		/// </summary>
		protected override void DisconnectControlEvents()
		{
			if (Control == null)
				return;
			if (TypeValidator.IsCompatible(Control.GetType(), typeof(Gtk.Widget)) == true) {
				(Control as Gtk.Widget).LeaveNotifyEvent -= OnLeaveNotifyEvent;
				(Control as Gtk.Widget).ExposeEvent -= HandleExposeEvent;
			}
		}
		
		/// <summary>
		/// Creates new Adaptor instance. Use this to overclass Adaptor with derived type
		/// </summary>
		public override Adaptor CreateAdaptorInstance (bool aIsBoundary, object aControl, bool aSingleMapping)
		{
			return (new GtkAdaptor (aIsBoundary, this, aControl, aSingleMapping));
		}

		/// <summary>
		/// Overrides OnLeaveEvent
		/// </summary>
		/// <param name="args">
		/// Normal OnLeaveNotify object parameter <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Normal OnLeaveNotify parameters <see cref="Gdk.EventCrossing"/>
		/// </param>
		/// <returns>
		/// Same as OnLeaveNotify <see cref="System.Boolean"/>
		/// </returns>
		protected void OnLeaveNotifyEvent (object o, Gtk.LeaveNotifyEventArgs args)
		{
			DemandOnLeavePost();
		}

		/// <summary>
		/// Writes debug information about this adaptor
		/// </summary>
		public override void DebugAdaptor()
		{
			System.Console.WriteLine("=========================================");
			base.DebugAdaptor();
			if (Control != null) {
				Gtk.Widget wdg = (Gtk.Widget) Control;
				if (wdg.Parent is System.Data.Bindings.IAdaptableContainer) {
					System.Console.WriteLine("Parent=" + wdg.Parent);
					System.Console.WriteLine("Parent.Name=" + wdg.Parent.Name);
					System.Console.WriteLine("Parent.DataSource=" + (wdg.Parent as System.Data.Bindings.IAdaptableContainer).DataSource);
					System.Console.WriteLine("Parent.Adaptor.FinalTarget=" + (wdg.Parent as System.Data.Bindings.IAdaptableContainer).Adaptor.Adaptor.FinalTarget);
				}
				System.Console.WriteLine("-----------------------------------------");
				DebugConnection();				
			}
			System.Console.WriteLine("=========================================");
		}
		
		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		protected GtkControlAdaptor() 
			: base ()
		{
		}

		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		public GtkControlAdaptor (object aControl, bool aSingleMapping)
			: base (aControl, aSingleMapping)
		{
		}

		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public GtkControlAdaptor (object aControl, bool aSingleMapping, string aMappings)
			: base (aControl, aSingleMapping, aMappings)
		{
		}
		
		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this Adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public GtkControlAdaptor (object aControl, bool aSingleMapping, object aDataSource, string aMappings)
			: base (aControl, aSingleMapping, aDataSource, aMappings)
		{
		}
		
		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Custom created Adaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aBoundaryAdaptor">
		/// Custom created BoundaryAdaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		public GtkControlAdaptor (object aControl, IAdaptor aAdaptor, IAdaptor aBoundaryAdaptor)
			: base (aControl, aAdaptor, aBoundaryAdaptor)
		{
		}

		/// <summary>
		/// Creates GtkControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Custom created Adaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aBoundaryAdaptor">
		/// Custom created BoundaryAdaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this Adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public GtkControlAdaptor (object aControl, IAdaptor aAdaptor, IAdaptor aBoundaryAdaptor, object aDataSource, string aMappings)
			: base (aControl, aAdaptor, aBoundaryAdaptor, aDataSource, aMappings)
		{
		}
	}
}
