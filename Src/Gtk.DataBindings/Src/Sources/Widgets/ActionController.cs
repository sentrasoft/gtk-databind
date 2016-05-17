// ActionController.cs created with MonoDevelop
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
using System.ComponentModel;
using System.Data.Bindings;
using System.Data.Bindings.Cached;
using System.Data.Bindings.Collections;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Action controller which controlls multiple actions
	/// </summary>
	public class ActionController : IAdaptableObjectReader
	{
		private WeakReference lastTarget = new WeakReference (null);
		private bool firsttime = true;
		private ArrayList actions = new ArrayList();

		private IAdaptor adaptor = null;
		/// <summary>
		/// Adaptor connected to this object
		/// </summary>
		[Browsable (false)]
		public IAdaptor Adaptor {
			get { return (adaptor); }
		}

		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false)]
		public object DataSource {
			get { return (adaptor.Target); }
			set { adaptor.Target = value;	}
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Browsable (false)]
		public string Mappings { 
			get { return (adaptor.Mappings); }
			set { adaptor.Mappings = value; }
		}
		
		/// <summary>
		/// Calculates Sensitive for specific ActionMonitor
		/// </summary>
		/// <param name="aMonitor">
		/// Action monitor being calculated Sensitive for <see cref="ActionMonitor"/>
		/// </param>
		/// <param name="aValue">
		/// New value to be assigned <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// true if sensitive, false if not <see cref="System.Boolean"/>
		/// </returns>
		private bool CalcSensitive (ActionMonitor aMonitor, bool aValue)
		{
			if (IsValid == false) {
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NotNullTarget)
					return (aMonitor.MonitorType == ActionMonitorType.InvertedSensitivity);
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NeedsValid)
					return (aMonitor.Defaults.DefaultValue);
			}
			else {
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NotNullTarget)
					return (aMonitor.MonitorType == ActionMonitorType.Sensitivity);
				else {
					if (aMonitor.MonitorType == ActionMonitorType.Sensitivity)
						return (aValue);
					return (!aValue);
				}
			}
			// Should not get here
			throw new Exception ("Invalid ActionMonitor");
		}
		
		private bool sensitive = false;
		/// <summary>
		/// Makes controlled actions either sensitive or not
		/// </summary>
		public bool Sensitive {
			get { return (sensitive); }
			set {
/*				if (firsttime == false)
					if (sensitive == value)
						if (lastTarget.Target == adaptor.FinalTarget)
							return;*/
				sensitive = value;
				foreach (ActionMonitor am in actions) {
					if (am.IsValid == false)
						continue;
					if ((am.MonitorType == ActionMonitorType.Sensitivity) ||
					    (am.MonitorType == ActionMonitorType.InvertedSensitivity)) {
						bool val = CalcSensitive (am, value);
						if (am.Action.Sensitive != val) {
							am.Action.Sensitive = val;
						}
					}
				}
			} 
		}

		/// <summary>
		/// Calculates Visible for specific ActionMonitor
		/// </summary>
		/// <param name="aMonitor">
		/// Action monitor being calculated Visible for <see cref="ActionMonitor"/>
		/// </param>
		/// <param name="aValue">
		/// New value to be assigned <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// true if visible, false if not <see cref="System.Boolean"/>
		/// </returns>
		private bool CalcVisible (ActionMonitor aMonitor, bool aValue)
		{
			if (IsValid == false) {
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NotNullTarget)
					return (aMonitor.MonitorType == ActionMonitorType.InvertedVisibility);
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NeedsValid)
					return (aMonitor.Defaults.DefaultValue);
			}
			else {
				if (aMonitor.Defaults.Mode == ActionMonitorDefaultsType.NotNullTarget) {
					if ((aMonitor.MonitorType == ActionMonitorType.Visibility) ||
					    (aMonitor.MonitorType == ActionMonitorType.InvertedVisibility))
						return (aMonitor.MonitorType == ActionMonitorType.Visibility);
				}
				else {
					if (aMonitor.MonitorType == ActionMonitorType.Visibility)
						return (aValue);
					if (aMonitor.MonitorType == ActionMonitorType.InvertedVisibility)
						return (!aValue);
				}
			}
			// Should not get here
			throw new Exception ("Invalid ActionMonitor");
		}
		
		private bool visible = false;
		/// <summary>
		/// Makes controlled actions either visible or not
		/// </summary>
		public bool Visible {
			get { return (visible); }
			set {
				if (firsttime == false)
					if (lastTarget.Target == adaptor.FinalTarget)
						if (visible == value) 
							return;
				visible = value;
				foreach (ActionMonitor am in actions) {
					if (am.IsValid == false)
						continue;
					if ((am.MonitorType == ActionMonitorType.Visibility) ||
					    (am.MonitorType == ActionMonitorType.InvertedVisibility)) {
						bool val = CalcVisible (am, value);
						if (am.Action.Visible != val)
							am.Action.Visible = val;
					}
				}
			}
		}

		/// <summary>
		/// Returns if datasource is valid or not
		/// </summary>
		public bool IsValid {
			get { return (Adaptor.FinalTarget != null); }
		}

		private string sensitiveMapping = "";
		/// <summary>
		/// Connects mapping to the Sensitive property
		/// </summary>
		public string SensitiveMapping {
			get { return (sensitiveMapping); }
			set { 
				sensitiveMapping = value;
				adaptor.Mappings = GetMapping();
			} 
		}

		private string visibleMapping = "";
		/// <summary>
		/// Connects mapping to the Sensitive property
		/// </summary>
		public string VisibleMapping {
			get { return (visibleMapping); }
			set { 
				visibleMapping = value; 
				adaptor.Mappings = GetMapping();
			} 
		}

		/// <summary>
		/// Calculates mappings for this object
		/// </summary>
		/// <returns>
		/// Mapping string <see cref="System.String"/>
		/// </returns>
		private string GetMapping()
		{
			string map = "bogusmapping; ";
			if (visibleMapping != "")
				map = map + VisibleMapping + ">>Visible; ";
			if (sensitiveMapping != "")
				map = map + SensitiveMapping + ">>Sensitive;";
			return (map);
		}

		/// <summary>
		/// Notification method activated from Adaptor 
		/// </summary>
		/// <param name="aSender">
		/// Object that made change <see cref="System.Object"/>
		/// </param>
		public virtual void GetDataFromDataSource (object aSender)
		{
/*			if (Adaptor.FinalTarget == null) {
				Visible = false;
				Sensitive = false;
				return;
			}*/
//System.Data.Bindings.DebugInformation.ConsoleDebug.TraceStack ("GetData");
			object vis, sen;
			CachedProperty.UncachedGetValue (Adaptor.FinalTarget, VisibleMapping, out vis, false);
			CachedProperty.UncachedGetValue (Adaptor.FinalTarget, SensitiveMapping, out sen, false);
			if (firsttime == false)
				if (((bool) vis == Visible) && ((bool) sen == Sensitive) && (lastTarget.Target == adaptor.FinalTarget))
					return;
//System.Console.WriteLine("AssignData (" + adaptor.FinalTarget + ") vis=" + (bool) vis + " sen=" + (bool) sen);
			Gtk.Application.Invoke (delegate {
				Visible = (bool) vis;
				Sensitive = (bool) sen;
//System.Console.WriteLine("Set firsttime");
				firsttime = false;
				lastTarget.Target = adaptor.FinalTarget;
			});
		}
		
		/// <summary>
		/// Renews data on target change
		/// </summary>
		protected void TargetChanged (IAdaptor aAdaptor)
		{
			//System.Console.WriteLine("Target=" + adaptor.FinalTarget + " called by " + aAdaptor.Id);
			GetDataFromDataSource (this);
		}

		/// <summary>
		/// Adds action to the list of monitored actions
		/// </summary>
		/// <param name="aAction">
		/// Action to add <see cref="ActionMonitor"/>
		/// </param>
		public void Add (ActionMonitor aAction)
		{
			if ((object) aAction == null)
				return;
			
			foreach (ActionMonitor am in actions)
				if ((object) am != null)
					if (am.IsValid == true)
						if ((am.Action == aAction.Action) && (am.MonitorType == aAction.MonitorType))
							return;

			actions.Add (aAction);
//			aAction.Action.Activated += OnActionActivated;
		}
		
		protected void OnActionActivated (object sender, System.EventArgs e)
		{
			//System.Console.WriteLine("ACTIVATEd");
			GetDataFromDataSource (this);
		}
			
		/// <summary>
		/// Removes action from the list of monitored actions
		/// </summary>
		/// <param name="aAction">
		/// Action to remove <see cref="ActionMonitor"/>
		/// </param>
		public void Remove (ActionMonitor aAction)
		{
			if ((object) aAction == null)
				return;
			
			ActionMonitor am;
			for (int i=actions.Count-1; i>=0; i--) {
				am = (ActionMonitor) actions[i];
				if ((object) am != null)
					if (am.IsValid == true)
						if ((am.Action == aAction.Action) && (am.MonitorType == aAction.MonitorType)) {
//							am.Action.Activated -= OnActionActivated;
							actions.Remove (am);
						}
			}
		}
		
		/// <summary>
		/// Removes action from the list of monitored actions
		/// </summary>
		/// <param name="aAction">
		/// Action to remove <see cref="Gtk.Action"/>
		/// </param>
		public void Remove (Gtk.Action aAction)
		{
			if (aAction == null)
				return;
			
			ActionMonitor am;
			for (int i=actions.Count-1; i>=0; i--) {
				am = (ActionMonitor) actions[i];
				if ((object) am != null)
					if (am.IsValid == true)
						if (am.Action == aAction) 
							actions.Remove (am);
			}
		}

		/// <summary>
		/// Clears the mapping list
		/// </summary>
		public void Clear()
		{
			for (int i=0; i<actions.Count; i++)
				actions[i] = null;
			actions.Clear();
		}
		
		/// <summary>
		/// Throws exception for its invalid creation
		/// </summary>
		private ActionController()
		{
			throw new ExceptionWrongCaseOfActionControllerInitialization();
		}
		
		/// <summary>
		/// Disconnects ActionController
		/// </summary>
		protected void Disconnect()
		{
			adaptor.DataChanged -= null;
			adaptor.TargetChanged -= null;
			adaptor.Disconnect();
			for (int i=0; i<actions.Count; i++)
				actions[i] = null;
			actions = null;
			adaptor = null;
		}
		
		/// <summary>
		/// Creates ActionController
		/// </summary>
		/// <param name="aActions">
		/// List of actions to control <see cref="ActionMonitor"/>
		/// </param>
		public ActionController (params ActionMonitor[] aActions)
		{
			adaptor = new GtkAdaptor();

			adaptor.DataChanged += GetDataFromDataSource;
			adaptor.TargetChanged += TargetChanged;

			foreach (ActionMonitor action in aActions)
				if ((object) action != null)
					if (action.IsValid == true)
						Add (action);
			GetDataFromDataSource (this);
		}

		/// <summary>
		/// Creates ActionController and maps Visibility and sensitivity
		/// </summary>
		/// <param name="aSensitivityMapping">
		/// Property mapped to Sensitivity <see cref="System.String"/>
		/// </param>
		/// <param name="aActions">
		/// List of actions to control <see cref="ActionMonitor"/>
		/// </param>
		public ActionController (string aSensitivityMapping, params ActionMonitor[] aActions)
		{
			adaptor = new GtkAdaptor();

			adaptor.DataChanged += GetDataFromDataSource;
			adaptor.TargetChanged += TargetChanged;
			
			sensitiveMapping = aSensitivityMapping;

			foreach (ActionMonitor action in aActions)
				if ((object) action != null)
					if (action.IsValid == true)
						Add (action);

			adaptor.Mappings = GetMapping();
			GetDataFromDataSource (this);
		}

		/// <summary>
		/// Creates ActionController and maps Visibility and sensitivity
		/// </summary>
		/// <param name="aVisibilityMapping">
		/// Property mapped to Visibility <see cref="System.String"/>
		/// </param>
		/// <param name="aSensitivityMapping">
		/// Property mapped to Sensitivity <see cref="System.String"/>
		/// </param>
		/// <param name="aActions">
		/// List of actions to control <see cref="ActionMonitor"/>
		/// </param>
		public ActionController (object aDataSource, string aVisibilityMapping, string aSensitivityMapping, params ActionMonitor[] aActions)
		{
			adaptor = new GtkAdaptor();

			adaptor.DataChanged += GetDataFromDataSource;
			adaptor.TargetChanged += TargetChanged;
			
			sensitiveMapping = aSensitivityMapping;
			visibleMapping = aVisibilityMapping;
				
			foreach (ActionMonitor action in aActions)
				if ((object) action != null)
					if (action.IsValid == true) {
						Add (action);
					//System.Console.WriteLine(action.Action.Name);
				}

			adaptor.Mappings = GetMapping();
			adaptor.Target = aDataSource;
			GetDataFromDataSource (this);
		}

		/// <summary>
		/// Disconnects adaptor and destroy its self
		/// </summary>
		~ActionController()
		{
			Disconnect();
		}
	}
}
