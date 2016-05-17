// Adaptor.cs - Adaptor implementation for Gtk#Databindings
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
using System.Collections.Generic;
using System.Reflection;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Adaptor class can reference any object and any property it exposes to the world
	/// </summary>
	public class Adaptor : IAdaptor
	{
		private bool insideReparenting = false;
		private bool stateResolvingInheritedTarget = false;
		private WeakReference lastFinalTarget = null;
		
		bool discover = false;
		/// <summary>
		/// Used to check on the Control. Provides optimization to resolving
		/// </summary>
		private struct PrevCheckup
		{
			private bool fmapp;
			
			/// <value>
			/// WeakReference to target enables more efficient work of GC
			/// </value>
			public WeakReference Target;

			/// <value>
			/// WeakReference to Items enables more efficient work of GC
			/// </value>			
			public WeakReference Items;

			/// <value>
			/// true if mapping is valid
			/// </value>
			public bool IsValidMapping;
			
			/// <summary>
			/// Checks if params are different. After that updates old params with the new ones as being
			/// passed in this call
			/// </summary>
			public bool NeedsCheckup (bool aValidMapping, object aTarget, object aItems)
			{
				bool res = ((IsValidMapping != aValidMapping) || (aItems != Items.Target) || 
				            (aTarget != Target.Target) || (fmapp == true));
				IsValidMapping = aValidMapping;
				Target.Target = aTarget;
				Items.Target = aItems;
				if (res == false)
					fmapp = false;
				return (res);
			}

			/// <value>
			/// Creates record
			/// </value>
			public PrevCheckup(bool aValidMapping, object aTarget, object aItems)
			{
				IsValidMapping = aValidMapping;
				Target = new WeakReference (aTarget);
				Items = new WeakReference (aItems);
				fmapp = true;
			}
		}
		
		private bool untouchedDataSource = true;
		private bool untouchedMappings = true;
		private ControlAdaptor controlAdaptor = null;
//		private ArrayList mappingList = new ArrayList();
		private List<MappedProperty> mappingList = new List<MappedProperty>();

		private bool renewingparent = false;
		private bool renewingtarget = false;
		private bool isDataAdaptor = false;
		private bool complexremapping = false;
		private bool targetisinherited = false;
		private PrevCheckup prevcheck = new PrevCheckup (false, null, null);
		private bool canaccessFinal = true;
		private bool destroyed = false;

		private System.Int64 id = 0;
		/// <summary>
		///
		/// </summary>
		public System.Int64 Id {
			get { return (id); }
		}

		/// <summary>
		/// Defines if cursor was created
		/// </summary>
		public bool CursorMode = false;

		private string dataSourceType = "";
		/// <summary>
		/// Constriction to DataSource Kind
		/// </summary>
		public string DataSourceTypeName {
			get { return (dataSourceType); }
		}
		
		private Type cachedDataSourceType = (Type) null;
		/// <summary>
		/// Provides constricted type of datasource
		/// </summary>
		/// <remarks>
		/// This value is changed everytime mapping is changed
		/// </remarks>
		public Type DataSourceType {
			get { 
				object res = null;
				if (DataSourceTypeName != "")
					res = cachedDataSourceType;
				if (res == null)
					return (null);
				return ((System.Type) res);
			}
		}
		
		/// <summary>
		/// Private setting of DataSourceType
		/// </summary>
		private string DSType {
			set {
				if (dataSourceType == value)
					return;
				dataSourceType = value;
				if (value != "") {
					cachedDataSourceType = TypeValidator.GetTypeInWholeAssembly (dataSourceType); // static method
					if (cachedDataSourceType == null)
						throw new ExceptionDataSourceType (value);
				}
				else
					cachedDataSourceType = (Type) null;
			}
		}
		
		private bool silence = false;
		/// <summary>
		/// Activates silent mode where meesaging will be handled afterwards
		/// </summary>
		public bool Silence {
			get { return (silence); }
			set { silence = value; }
		}
		
		/// <summary>
		/// Returns if Adaptor is controlling widget or not. This makes
		/// the same control usable with Adaptor or with classic approach
		/// </summary>
		/// <returns>
		/// true for Boundary Adaptor or if InheritedTarget is true
		/// 
		/// Otherwise it returns false if both Mappings and DataSource were 
		/// untouched
		/// </returns>
		public bool IsControllingWidget {
			get {
				if ((IsBoundaryAdaptor == true) || (InheritedTarget == true))
					return (true);
				return ((untouchedDataSource == false) || (untouchedMappings == false)); 
			}
		}

		/// <value>
		/// Unfortunatelly, it is unavoidable for this not to be public
		/// But. Do not use it! Only usage case is when Databindings 
		/// are extended to provide new functionality
		/// </value>
		public ControlAdaptor ControlAdaptor {
			get { return (controlAdaptor); }
			set { 
				// if this was done then it was not default case of Adaptor
				// but specified as extra param
				controlAdaptor = value; 
				if (controlAdaptor != null) {
					if (IsBoundaryAdaptor == false)
						ConnectionProvider.ConnectControlAndAdaptor (Control, this);
					DataSourceController.AddAdaptor (this);
				}
			}
		}
		
		private bool destroyInProgress = false;
		/// <summary>
		/// Called by control to notify end of events. Disposing goes faster this
		/// way. And GC gets a little kick in the ass sooner than it usualy would
		/// </summary>
		public bool DestroyInProgress {
			get { return (destroyInProgress); }
			set { destroyInProgress = value; }
		}
		
		private bool inheritedTarget = false;
		/// <summary>
		/// Declares if this adaptor has inherited target or not
		/// </summary>
		public bool InheritedTarget {
			get { return ((inheritedTarget == true) && (CursorMode == false)); }
			set {
				if (inheritedTarget == value)
					return;
				if (CursorMode == true)
					inheritedTarget = false;
				if (inheritedTarget == false) {
					if (destroyInProgress == false)
						Target = null;
				}
				else {
					if ((Control != null) && (IsBoundaryAdaptor == false) && (destroyInProgress == false) && (destroyed == false)) {
//						targetisinherited = true;
						finalTarget.Target = null;
						Target = null;
//						Target = ConnectionProvider.ResolveDataSourceFromParent (controlAdaptor, Control);
//						targetisinherited = false;
					}
				}
				inheritedTarget = value;
			}
		}
		
		private bool isBoundary = false;
		/// <summary>
		/// Return if this Adaptor is boundary or not.
		///
		/// Difference between BoundaryAdaptor and Adaptor is that one is setting data between
		/// Target and control, while Boundary on the other hand uses one purpose only, to map
		/// controls layout as controlled by external object
		///
		/// example of Boundary Mapping:
		///    MinValue>>Min, MaxValue>>Max, IsVisible>>Visible
		///
		/// in case of Scale those would control Min, Max and Visible property in Scale
		///
		/// General use of BoundaryDataSource intends to simplify controlling of the view from 
		/// external file for example
		/// </summary>
		public bool IsBoundaryAdaptor {
			get { return (isBoundary); }
		}
		
		private ValueList values = null;
		/// <summary>
		/// Links to other values by name
		/// </summary>
		public ValueList Values {
			get { return (values); }
		}
		
		/// <summary>
		/// Returns if this adaptor is active or not
		/// </summary>
		public bool Activated {
			get { return (((mappingList.Count > 0) || (Target != null)) || (isDataAdaptor == true)); }
		}
		
		/// <value>
		/// Target object where items are collected from
		/// </value>
		private object ItemsTarget {
			get {
				if (Control == null)
					if (Control is IAdaptableListControl)
						return ((Control as IAdaptableListControl).ItemsDataSource);
				return (null);
			}
		}
		
		internal WeakReference finalTarget = new WeakReference (null);
		/// <summary>
		/// Resolves final target
		/// </summary>
		public object FinalTarget {
			get { return (GetFinalTarget()); }
		}
		
		private object target = null;
		/// <summary>
		/// Target is the referenced object in question, if that object is IChangeable
		/// Adaptor will automaticaly connect OnChange event and send those notifications to
		/// connected control if control is supporting this option
		/// </summary>
		public object Target { 
			get {
				if (stateResolvingInheritedTarget == false)
					if ((target == null) && (InheritedTarget == true)) {
						stateResolvingInheritedTarget = true;
						Target = ConnectionProvider.ResolveDataSourceFromParent (controlAdaptor, Control); 
						stateResolvingInheritedTarget = false;
					}
				return (target); 
			}
			set {
				if (target == value)
					return;
			
				untouchedDataSource = false;
				object prevtarget = target;

/*				if (targetisinherited == false)
					if ((target != null) && (InheritedTarget == true) && (destroyInProgress == false)) 
						InheritedTarget = false;*/
				if ((InheritedTarget == true) && (stateResolvingInheritedTarget == false))
					if ((destroyInProgress == false) && (value != null))
						throw new Exception ("Setting Adaptor.Target, but Target should be inherited\n" +
						                     "Control=" + Control + " PAram=" + value);
				
				if (destroyInProgress == false)
					if ((target != null) || (value == null) || (FinalTarget == null) || (FinalTarget.GetType() != value.GetType()))
						ResetPropertyItems();

				if (target != null) {
					OnDataSourceDisconnect (target);
					MSEventSupport.DisconnectEvent (target);
				}

				// Set target and notify all Adaptors interested in this change
				target = value;
				finalTarget.Target = null;
				
				if (target != null) {
					OnDataSourceConnect (target);
					MSEventSupport.ConnectEvent (target);
				}

				if (prevtarget != target)
					OnTargetChanged (this);
				prevtarget = null;
/*				if ((controlAdaptor != null) && (IsBoundaryAdaptor == false))
					if ((controlAdaptor.ControlIsWindow(Control) == false) && (controlAdaptor.GetParentOfControl(Control) != null) && (controlAdaptor.ParentWindow(Control) != null))
						if (prevcheck.NeedsCheckup(IsValidMapping, FinalTarget, ItemsTarget) == true)
							controlAdaptor.CheckControl();*/
				if (Mappings != "")
					Mappings = Mappings;
			}
		}
		
		private bool singleMappingOnly = false;
		/// <summary>
		/// Most controls can support one mapping only
		///
		/// More complex examples binding to more than one mapping should set this to true
		/// </summary>
		public bool SingleMappingOnly {
			get { return (singleMappingOnly); }
		}
		
		/// <summary>
		/// Control connected to this adaptor
		///
		/// </summary>
		[ToDo ("Check for multiple control possibility")]
		public object Control {
			get { 
				if (controlAdaptor != null)
					return (controlAdaptor.Control);
				return (null);
			}
		}
		
		private bool cachedMappings = false;
		private string mappings = "";
		/// <summary>
		/// Generic mappings for Target, if applied then Target has to be specified.
		///
		/// </summary>
		[ToDo ("Check for possibility of specifiying mappings without connected Target")]
		public string Mappings {
			get { 
				isValidated = false;
				if ((SingleMappingOnly == false) && (mappings == ""))
					mappings = ResolveMappingsAsString();
				if (SingleMappingOnly == false)
					return (mappings);
				else 
					return (ResolveMappingsAsString());
			}
			set {
				if ((value != mappings) || (value != ""))
					untouchedMappings = false;
				string oldmappings = Mappings;

				complexremapping = true;

				ClearMappings();
				SetNewMappingsFromStrings (value);

				complexremapping = false;
				
				if ((controlAdaptor != null) && (IsBoundaryAdaptor == false))
					if ((controlAdaptor.ControlIsWindow(Control) == false) && (controlAdaptor.GetParentOfControl(Control) != null) && (controlAdaptor.ParentWindow(Control) != null))
						if (prevcheck.NeedsCheckup(IsValidMapping, FinalTarget, ItemsTarget) == true)
							controlAdaptor.CheckControl();
/*				if ((oldmappings != Mappings) && (Control is IAdaptableListControl)) {
					controlAdaptor.ClearBeforeRemapping();
					controlAdaptor.RemapControl();
				}*/
			}
		}
		
		/// <summary>
		/// Number of applied mappings
		/// </summary>
		public int MappingCount {
			get {
				if (SingleMappingOnly == false)
					return (mappingList.Count);
				else {
					if (mappingList.Count > 1)
						return (1);
					else
						return (mappingList.Count);
				}
			}
		}
		
		private bool cachedValidResult = false;
		private bool isValidated = false;
		/// <summary>
		/// Checks if mapping is valid
		/// </summary>
		[ToDo ("Cache validation if possible")]
		public bool IsValidMapping {
			get {
				// return true if this is pointer adaptor
				if (isDataAdaptor == true)
					return (true);
//				if (isValidated == true)
//					return (cachedValidResult);

				// Some controls don't need mappings, but at the same time they contain child controls
				// and they shouldn't be eliminated this way
				if (canaccessFinal == true) {
					if (controlAdaptor.ControlIsWindow(Control) == false)
						// If this is list control then items should be decisive factor
						if (Control is IAdaptableListControl) {
							if ((Control as IAdaptableListControl).ItemsDataSource == null)
								return (false);
						}
						else
							if (FinalTarget == null)
								if (mappingList.Count != 0)
									return (false);
				}
				else
					// If this is list control then items should be decisive factor
					if (Control is IAdaptableListControl) {
						if ((Control as IAdaptableListControl).ItemsDataSource == null)
							return (false);
					}
					else
						if (finalTarget.Target == null)
							if (mappingList.Count != 0)
								return (false);
				return (true); 
			}
		}
		
		private bool hasDefaultMapping = false;
		/// <summary>
		/// Returns if this adaptor has Default mapping
		/// </summary>
		public bool HasDefaultMapping {
			get { return (hasDefaultMapping); }
		}
		
		private event AdapteeDataChangeEvent dataChanged = null;
		/// <summary>
		/// OnChange event that is crossconnecting Target and controls
		/// </summary>
		public event AdapteeDataChangeEvent DataChanged {
			add { dataChanged += value; }
			remove { dataChanged -= value; }
		}

		private event TargetChangedEvent targetChanged = null;
		/// <summary>
		/// OnChange event that is crossconnecting Target and controls
		/// </summary>
		public event TargetChangedEvent TargetChanged {
			add { targetChanged += value; }
			remove { targetChanged -= value; }
		}

		private event PostMethodEvent postRequested = null;
		/// <summary>
		/// PostRequested event that is crossconnecting Target and controls
		/// </summary>
		public event PostMethodEvent PostRequested {
			add { postRequested += value; }
			remove { postRequested -= value; }
		}
		
		/// <summary>
		/// Auto connects properties with controls based on the same name
		///
		/// AutoConnect() should be called after DataSource is specified. After this
		/// only special bindings should be specified
		///
		/// Should avoid any kind of boundary adaptors
		/// </summary>
		[ToDo ("Would be nice to have autoconnect")]
		public void AutoConnect()
		{
			if ((controlAdaptor == null) || (IsBoundaryAdaptor == true))
				return;
			if (controlAdaptor.Control == null)
				return;
			// Do the magic of autoconnecting with reflection trough DataSource
			//if (controlAdaptor.Control.Name ==
		}
		
		/// <summary>
		/// Checks if MappedProperty exists
		/// </summary>
		public bool Exists (string aName)
		{
			if (values == null)
				return (false);
			return (Values[aName] != null);
		}
		
		/// <summary>
		/// Transfers all secondary values from DataSource to Requester and notifies about
		/// change if needed
		/// </summary>
		/// <param name="aRequester">
		/// Object to be filled with secondary values <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// In case of error caller is responsible to handle exceptions
		/// </remarks>
		public virtual void FillObjectWithDataSourceValues (object aRequester)
		{
			object DS = ConnectionProvider.ResolveTargetFor (this);
			if ((aRequester == null) || (DS == null)) {
				DS = null;
				return;
			}

			foreach (MappedProperty mp in Values) {
				if (mp.IsSecondaryTargeted == true)
					mp.AssignValueToObject (EDataDirection.FromDataSourceToControl, DS, aRequester);
			}
			DS = null;
		}
		
		/// <summary>
		/// Transfers all secondary values from Control to DataSource and notifies about
		/// change if needed
		/// </summary>
		/// <param name="aRequester">
		/// Object to be used as source for secondary values <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// In case of error caller is responsible to handle exceptions
		/// </remarks>
		public virtual void FillDataSourceWithObjectValues (object aRequester)
		{
			object DS = ConnectionProvider.ResolveTargetFor (this);
			if ((aRequester == null) || (DS == null)) {
				DS = null;
				return;
			}
				
			foreach (MappedProperty mp in Values)
				if (mp != null)
					if (mp.IsSecondaryTargeted == true)
						mp.AssignValueToObject (EDataDirection.FromControlToDataSource, DS, aRequester);
			DS = null;
		}
		
		/// <summary>
		/// Additional actions to be taken on connecting DataSource
		/// </summary>
		/// <param name="aTarget">
		/// Target object being connected <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// Connect all the messaging derived adaptors need here
		/// </remarks>
		internal virtual void OnDataSourceConnect (object aTarget)
		{
			// If target is IAdaptor then it can't be added to DataSourceController
			// it has to be connected manually
			if (aTarget != null) {
				if ((aTarget is IAdaptor) == false)
					DataSourceController.Add (aTarget, this);
				else {
					(aTarget as IAdaptor).TargetChanged += AdaptorTargetChanged;
					(aTarget as IAdaptor).DataChanged += DoAdapteeDataChanged;
					(aTarget as IAdaptor).PostRequested += PostMethod;
				}
				DataSourceController.GetRequest (FinalTarget);
			}

/*			if ((controlAdaptor != null) && (controlAdaptor.Control is IComplexAdaptableControl)) {
				// renew mappings in this control as t probably contains more complex connections
				controlAdaptor.ClearBeforeRemapping();
				controlAdaptor.RemapControl();
			}*/
		}
		
		/// <summary>
		/// Additional actions to be taken on disconnecting DataSource
		/// </summary>
		/// <param name="aTarget">
		/// Target object being disconnected <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// Disconnect all the messaging derived adaptors need here
		/// </remarks>
		internal virtual void OnDataSourceDisconnect (object aTarget)
		{
			// If target is IAdaptor then it can't be removed from DataSourceController
			// it has to be disconnected manually
			if (aTarget != null)
				if ((aTarget is IAdaptor) == false)
					DataSourceController.Remove (aTarget, this);
				else {
					(aTarget as IAdaptor).TargetChanged -= AdaptorTargetChanged;
					(aTarget as IAdaptor).DataChanged -= DoAdapteeDataChanged;
					(aTarget as IAdaptor).PostRequested -= PostMethod;
				}
		}
		
		/// <summary>
		/// Invalidates cached validation
		/// </summary>
		public void InvalidateCacheValidation()
		{
			isValidated = false;
		}
		
		/// <summary>
		/// Resolves new mapping from string 
		/// </summary>
		/// <param name="aMapping">
		/// Mapping string to be resolved <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Mapped item <see cref="SMappedItem"/>
		/// </returns>
		public SMappedItem ResolveMappingString(string aMapping)
		{
			return (new SMappedItem(aMapping));
		}

		/// <summary>
		/// Searches for 
		/// </summary>
		/// <param name="aStr">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		private int CharIndexOf (string aStr)
		{
			int pos = aStr.IndexOf(";");
			int sbpos = aStr.IndexOf("[");
			int cbpos = aStr.IndexOf("]");
			if (pos == -1)
				return (pos);
			if ((sbpos > -1) && (cbpos > -1))
				if (sbpos < pos)
					if (cbpos > pos)
						// Start looking for first ; after ]
						pos = aStr.IndexOf (";", cbpos);
					else
						return (pos);
			return (pos);
		}
		
		/// <summary>
		/// Resolves new mappings
		/// </summary>
		/// <param name="aMapping">
		/// Mapping string to be resolved <see cref="System.String"/>
		/// </param>
		public void SetNewMappingsFromStrings(string aMapping)
		{
			ArrayList lst = new ArrayList();

			int s = aMapping.IndexOf("{");
			int e = aMapping.IndexOf("}");
			if (s != e) {
				if ((s+1) >= e)
					throw new ExceptionWrongFormulatedType();
				else
					DSType = aMapping.Substring (s+1, e-s-1).Trim();
				aMapping = aMapping.Remove (s, e-s+1);
			}

			string map = "";
			int pos = CharIndexOf (aMapping);
			while (pos > -1) {
				map = aMapping.Substring (0, pos).Trim();
				if (map != "")
					lst.Add (ResolveMappingString (map));
				aMapping = aMapping.Remove (0, pos+1);
				pos = CharIndexOf (aMapping);
			}
			aMapping = aMapping.Trim();
			if (aMapping != "")
				lst.Add (ResolveMappingString (aMapping));

			while (MappingCount > 0) {
				Values[0].Disconnect();
				mappingList[0] = null;
				mappingList.RemoveAt (0);
			}
			
			foreach (SMappedItem item in lst)
				if (item.Name != "")
					AddMapping (item.Name, ConnectionProvider.ResolveMappingType(FinalTarget, item.Name),
					            item.RWFlags, item.ClassName, (item.ColumnName != ""),
					            item.ColumnName, item.MappedItem, item.ColumnItems);
			lst.Clear();
			lst = null;
		}
		
		/// <summary>
		/// Returns value of Mappings as formatted string
		/// </summary>
		/// <returns>
		/// formatted mappings <see cref="System.String"/>
		/// </returns>
		public string ResolveMappingsAsString()
		{
			if (cachedMappings == true)
				return (mappings);
				
			if (mappingList.Count == 0) 
				return ("");
			string res = "";
			for (int i=0; i<MappingCount; i++)
				if (i < (MappingCount - 1))
					res = res + Values[i].ToString() + "; ";
				else
					res = res + Values[i].ToString();
			return (res);
		}
		
		/// <summary>
		/// Reposts renewal to all children controls
		/// </summary>
		/// <param name="aControl">
		/// Control which should repost renew <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Action <see cref="EActionType"/>
		/// </param>
		public virtual void RepostRenewToAllChildren(object aControl, EActionType aAction)
		{
//			throw new ExceptionAdaptorRepostNotSupported(); 
		}
		
		/// <summary>
		/// Checks if control is right type for Adaptor
		/// </summary>
		/// <param name="aControl">
		/// Control to be checked <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is right type, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected virtual bool CheckControlType (object aControl)
		{
			//controlAdaptor is null if this is pointer type of adaptor
			if (controlAdaptor == null)
				return (true);
			return (controlAdaptor.ValidateControlType (aControl));
		}
		
		/// <summary>
		/// Message loop for Adaptors
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Action type <see cref="EActionType"/>
		/// </param>
		public void SendAdaptorMessage (object aSender, EActionType aAction)
		{
			// No need for checking if message is here then message was inteded to be here
			switch (aAction) {
			case EActionType.RenewParents :
				if ((aSender != null) && (CheckControlType(aSender) == true))
					RepostRenewToAllChildren (aSender, EActionType.RenewParents);
				else {
					if (renewingparent == true)
						return;
					renewingparent = true;
					finalTarget.Target = null;
					if (controlAdaptor != null) {
						if (InheritedTarget == true) {
							bool renewneeded = false;
							object o = ConnectionProvider.ResolveParentContainer (controlAdaptor, Control);
							if (controlAdaptor.ParentContainer == null) {
								if (o != null) {
									controlAdaptor.ParentContainer = o;
									renewneeded = true;
								}
							}
							else {
								if (o != controlAdaptor.ParentContainer)
									renewneeded = true;
							}
							if ((Control != null) && (IsBoundaryAdaptor == false) && (renewneeded == true)) {
								stateResolvingInheritedTarget = true;
								Target = null;
								stateResolvingInheritedTarget = false;
								controlAdaptor.CheckControlState();
							}
						}

						RepostRenewToAllChildren (Control, EActionType.RenewParents);
					}
				}
				renewingparent = false;
				break;
/*			case EActionType.PostRequest :
				break;
			case EActionType.GetRequest :
				AdapteeDataChanged (null);
				break;*/
			}
		}
		
		/// <summary>
		/// Reset all mapped property items
		/// </summary>
		internal void ResetPropertyItems()
		{
			isValidated = false;
			foreach (MappedProperty prop in mappingList)
				prop.Reset();
		}
		
		/// <summary>
		/// Returns mapping name based on its index number
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped property <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// MappedProperty if found <see cref="MappedProperty"/>
		/// </returns>
		public MappedProperty Mapping (int aIdx)
		{
			if ((aIdx < 0) || (aIdx >= mappingList.Count))
				return (null);
			
			return ((MappedProperty) mappingList[aIdx]);
		}
		
		/// <summary>
		/// Returns mapping name based on its index number
		/// </summary>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// MappedProperty by that name <see cref="MappedProperty"/>
		/// </returns>
		public MappedProperty MappingByName (string aName)
		{
			if (aName == "")
				return (GetDefaultProperty());
				
			foreach (MappedProperty mp in mappingList)
				if (mp != null)
					if (mp.Name == aName)
						return (mp);
			return (null);
		}
		
		/// <summary>
		/// Returns mapping type based on its index number
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped property <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Type of property value <see cref="System.Type"/>
		/// </returns>
		public System.Type MappingType (int aIdx)
		{
			if ((aIdx < 0) || (aIdx >= mappingList.Count))
				return ((System.Type) System.Type.Missing);
			
			return ((System.Type) ((MappedProperty) mappingList[aIdx]).GetMappedType());
		}
		
		/// <summary>
		/// Adds mapping by name
		/// </summary>
		/// <param name="aName">
		/// Name of mapping <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool AddMapping (string aName)
		{
			return (AddMapping(aName, ConnectionProvider.ResolveMappingType(FinalTarget, aName)));
		}
		
		/// <summary>
		/// Adds mapping by name and System.Type
		/// </summary>
		/// <param name="aName">
		/// Name of mapping <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type of mapping value <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool AddMapping (string aName, System.Type aType)
		{
			return (AddMapping(aName, ConnectionProvider.ResolveMappingType(FinalTarget, aName), EReadWrite.ReadWrite,  
			                   "", false, "", "", null));
		}
		
		/// <summary>
		/// Adds new mapping
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type of mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aRW">
		/// Read-write settings for mapped property <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aClassName">
		/// Defines class name <see cref="System.String"/>
		/// </param>
		/// <param name="aIsColumn">
		/// Defines if mapping is coumn mapping or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aColumnName">
		/// Name of column <see cref="System.String"/>
		/// </param>
		/// <param name="aMapping">
		/// Mapping string <see cref="System.String"/>
		/// </param>
		/// <param name="aSubItems">
		/// List of subitems <see cref="SMappedItem"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool AddMapping (string aName, System.Type aType, EReadWrite aRW, string aClassName, bool aIsColumn, 
		                        string aColumnName, string aMapping, SMappedItem[] aSubItems)
		{
			cachedMappings = false;
			MappedProperty mp = Values[aName];
			if (mp != null) {
				// There is one case when duplicate is allowed, if there is only one as global and
				// only one as default
				bool found = false;
				foreach (MappedProperty mpr in Values)
					if ((mpr.Name == aName) && (mpr.IsColumnMapping == aIsColumn))
						found = true;
				if (found == false)
					mp = null;
			}
			if (mp != null) {
				if (mp.BoundingClassName != aClassName)
					mp.BoundingClassName = aClassName;
				if (mp.IsColumnMapping != aIsColumn)
					mp.IsColumnMapping = aIsColumn;
				if (mp.ColumnName != aColumnName)
					mp.ColumnName = aColumnName;
				if (mp.MappingTarget != aMapping)
					mp.MappingTarget = aMapping;
				mp = null;
				throw new ExceptionAddWrongMapping();
			}
			
			if (aName == "")
				throw new ExceptionAddEmptyMapping();

			isValidated = false;
			if (IsBoundaryAdaptor == true)
				if (aClassName != "")
					mappingList.Add (new MappedProperty (this, aClassName, aName, aRW, aMapping));
				else
					mappingList.Add (new MappedProperty (this, aName, aRW, aMapping));
			else
				if ((aIsColumn == true) && (aColumnName != ""))
					if (aMapping == "")
						mappingList.Add (new MappedProperty (this, true, aName, aColumnName, aRW, aSubItems));
					else
						mappingList.Add (new MappedProperty (this, true, aName, aColumnName, aRW, aMapping, aSubItems));
				else
					if (aMapping == "") 
						mappingList.Add (new MappedProperty (this, aName));
					else
						mappingList.Add (new MappedProperty (this, aName, aRW, aMapping));

			if (HasDefaultMapping == false) 
				hasDefaultMapping = (GetDefaultProperty() != null);
			return (true);
		}
		
		/// <summary>
		/// Removes mapping by name
		/// </summary>
		/// <param name="aName">
		/// Name of mapping to remove <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if success <see cref="System.Boolean"/>
		/// </returns>
		public bool RemoveMapping (string aName)
		{
			cachedMappings = false;
			int idx = 0;
			foreach (MappedProperty prop in mappingList)
				if (prop.Name == aName) {
					prop.Disconnect();
					mappingList.RemoveAt (idx);
					hasDefaultMapping = (GetDefaultProperty() != null);
					return (true);
				}
				else
					idx++;
			throw new ExceptionMappingNotFound(aName);
		}
		
		/// <summary>
		/// Clears all mappings from adaptor and control
		/// </summary>
		public void ClearMappings()
		{
			cachedMappings = false;
			isValidated = false;
			if (Control is IAdaptableControl)
				if ((Control as IAdaptableControl).Mappings != "") {
					mappings = "";
					mappingList.Clear();
					hasDefaultMapping = false;
				}
		}
		
		/// <summary>
		/// Resolves Default MappedProperty
		/// </summary>
		/// <returns>
		/// Default Mapped property <see cref="MappedProperty"/>
		/// </returns>
		public MappedProperty GetDefaultProperty()
		{
			if (IsBoundaryAdaptor == true)
				throw new ExceptionBoundaryDefaultPropertyAccess();
			foreach (MappedProperty mp in Values) {
				if (mp != null)
					if (mp.IsDefault == true)
						return (mp);
			}
			return (null);
		}
		
		/// <summary>
		/// Resolves first mapping value from the connected mappings
		/// </summary>
		/// <returns>
		/// Default Mapped property value <see cref="System.Object"/>
		/// </returns>
		public object GetDefaultMappingValue()
		{
			if (FinalTarget is IAdaptor)
				return ((FinalTarget as IAdaptor).GetDefaultMappingValue());
			
			MappedProperty mp = GetDefaultProperty();
			if (mp == null)
				return (null);
			object res = null;
			if (mp.Valid == true)
				res = mp.Value;
			mp = null;
			return (res);
		}
		
		/// <summary>
		/// Sets first mapping value from the connected mappings
		/// </summary>
		/// <param name="aValue">
		/// Value to be set <see cref="System.Object"/>
		/// </param>
		public void SetDefaultMappingValue (object aValue)
		{
			if (FinalTarget is IAdaptor) {
				(FinalTarget as IAdaptor).SetDefaultMappingValue (aValue);
				return;
			}
			
			MappedProperty mp = GetDefaultProperty();
			if (mp == null)
				throw new ExceptionAssigningNonExistantDefaultProperty();
			if (mp.Valid == true)
				mp.Value = aValue;
			mp = null;
		}
		
		/// <summary>
		/// Resolves mapping value from the connected mappings
		/// </summary>
		/// <param name="aName">
		/// Name of mapping <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Value of property <see cref="System.Object"/>
		/// </returns>
		public object GetMappingValue (string aName)
		{
			if (FinalTarget is IAdaptor)
				return ((FinalTarget as IAdaptor).GetMappingValue (aName));
			
			foreach (MappedProperty prop in mappingList)
				if (prop.Name == aName)
					return (prop.Value);

			return (null);
		}
		
		/// <summary>
		/// Sets mapping value from the connected mappings
		/// </summary>
		/// <param name="aName">
		/// Name of mapping <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Value to be mapped <see cref="System.Object"/>
		/// </param>
		public void SetMappingValue(string aName, object aValue)
		{
			if (FinalTarget is IAdaptor) {
				(FinalTarget as IAdaptor).SetMappingValue (aName, aValue);
				return;
			}
			
			foreach (MappedProperty prop in mappingList)
				if (prop.Name == aName)
					prop.Value = aValue;
		}
		
		/// <summary>
		/// Resolves mapping[index] value from the connected mappings
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped property <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Value <see cref="System.Object"/>
		/// </returns>
		public object GetMappingValue (int aIdx)
		{
			if (FinalTarget is IAdaptor)
				return ((FinalTarget as IAdaptor).GetMappingValue (aIdx));
			
			if ((aIdx >= 0) && (aIdx < mappingList.Count))
				return (((MappedProperty) mappingList[aIdx]).Value);

			return (null);
		}
		
		/// <summary>
		/// Sets mapping[index] value from the connected mappings
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped property <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// Value to be set <see cref="System.Object"/>
		/// </param>
		public void SetMappingValue (int aIdx, object aValue)
		{
			if (FinalTarget is IAdaptor) {
				(FinalTarget as IAdaptor).SetMappingValue (aIdx, aValue);
				return;
			}
			
			if ((aIdx >= 0) && (aIdx < mappingList.Count))
				((MappedProperty) mappingList[aIdx]).Value = aValue;
		}
		
		/// <summary>
		/// Handles notifications for dispatch of bounding DataSource properties. 
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public void BoundaryAdapteeDataChanged (object aSender)
		{
			if (IsBoundaryAdaptor == false)
				throw new ExceptionNonBoundaryAdaptorConnectedToBoundaryEvent();
				
			SetBoundaryMappingValues (this, controlAdaptor.Control);
		}
		
		/// <summary>
		/// Sets non global
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor to set boundary values for <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control to set properties for <see cref="System.Object"/>
		/// </param>
		public void SetBoundaryMappingValues (IAdaptor aAdaptor, object aControl)
		{
			if ((aAdaptor == null) || (aControl == null))
				return;
				
			if (aAdaptor.IsBoundaryAdaptor == false)
				throw new ExceptionNonBoundaryAdaptorConnectedToBoundaryEvent();
			
			foreach (MappedProperty mp in aAdaptor.Values)
				if (mp != null)
					if (mp.IsGlobal == false)
						mp.AssignValueToObject (EDataDirection.FromDataSourceToControl, FinalTarget, aControl);
		}
		
		/// <summary>
		/// Notifies all connected parties and then  proceeds to global dispatch
		/// loop which sends this same message to all Adaptors who have 
		/// InheritedDataSource = true and are part of this Adaptors container.
		///
		/// Example: WindowAdaptor having set DataSource is automatically dispatching 
		/// the same messages about changes to its child controls that have not set
		/// their own DataSource and have InheritedDataSource = true
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public virtual void AdapteeDataChanged (object aSender)
		{
			if (FinalTarget != null)
				if (FinalTarget is IObserveable) {
					IObserveable observer = (FinalTarget as IObserveable);
					if (observer != null)
						if (observer.HasChanged == false)
							InvokeOnDataChange (aSender);
					observer = null;
				}
				else {
					IObserveable observer = DataSourceController.GetInfoFor(FinalTarget);
					if (observer != null)
						if (observer.HasChanged == false)
							InvokeOnDataChange (aSender);
					observer = null;
				}
		}
		
		/// <summary>
		/// Notifies all connected parties and then  proceeds to global dispatch
		/// loop which sends this same message to all Adaptors who have 
		/// InheritedDataSource = true and are part of this Adaptors container.
		///
		/// Example: WindowAdaptor having set DataSource is automatically dispatching 
		/// the same messages about changes to its child controls that have not set
		/// their own DataSource and have InheritedDataSource = true
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		private void DoAdapteeDataChanged (object aSender)
		{
			AdapteeDataChanged (aSender);
		}
		
		/// <summary>
		/// Invokes OnDataChange event
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public virtual void InvokeOnDataChange (object aSender)
		{
			if (dataChanged != null)
				dataChanged (aSender);
		}
		
		/// <summary>
		/// Notifies all connected parties about new Target
		///
		/// The only ones connected here are ControlAdaptor types which took care
		/// of the controls
		/// </summary>
		protected void OnTargetChanged (IAdaptor aSender)
		{
			if (insideReparenting == true)
				return;
			if (destroyInProgress == true)
				return;
			insideReparenting = true;
			finalTarget.Target = null;
			isValidated = false;
			GetFinalTarget();
			if (lastFinalTarget != null) {
				if (finalTarget.Target == lastFinalTarget.Target) {
					insideReparenting = false;					
					return;
				}
				else
					lastFinalTarget.Target = finalTarget.Target;
			}
			else
				lastFinalTarget = new WeakReference (finalTarget.Target);
			
/*			if (CursorMode == true) {
				if (Target is IAdaptor)
					if ((Target as IAdaptor) != aSender) {
					insideReparenting = false;
					System.Console.WriteLine("Avoid non-parent cursor");
					return;
				}
			}*/

			if (targetChanged != null)
				targetChanged (aSender);
/*			if (controlAdaptor != null)
				if (controlAdaptor.ControlIsContainer(Control) == true)
					SendAdaptorMessage (Control, EActionType.RenewTargets);*/
/*			if (Control == null) {
				Notificator.NotifyObjectChanged (Target);
				DataSourceController.RecheckStateOfInheritedControls();
			}*/
			insideReparenting = false;
		}

		/// <summary>
		/// Resets FinalTarget and resolves it again
		/// </summary>
		protected void ResetFinalTarget()
		{
			finalTarget.Target = null;
			GetFinalTarget();
		}
		
		/// <summary>
		/// Resolves final target for Adaptor 
		/// </summary>
		/// <returns>
		/// Object of the final target this adaptor is pointing to <see cref="System.Object"/>
		/// </returns>
		protected virtual object DoGetFinalTarget (out bool aCallControl)
		{
			aCallControl = false;
			if (destroyed == true)
				return (null);
			if (finalTarget.Target == null) {
				finalTarget.Target = ConnectionProvider.ResolveTargetForObject (Target);
				aCallControl = true;
			}

			return (finalTarget.Target);
		}
		
		/// <summary>
		/// Resolves final target for Adaptor 
		/// </summary>
		/// <returns>
		/// Object of the final target this adaptor is pointing to <see cref="System.Object"/>
		/// </returns>
		protected object GetFinalTarget()
		{
			bool call = false;
			object res = DoGetFinalTarget (out call);

			if (call == true)
				if (destroyInProgress == false)
					if ((IsBoundaryAdaptor != true) && (Control != null) && (controlAdaptor != null)) {
						canaccessFinal = false;
						if ((controlAdaptor.ControlIsWindow(Control) == false) && (controlAdaptor.GetParentOfControl(Control) != null) && (controlAdaptor.ParentWindow(Control) != null))
							if (prevcheck.NeedsCheckup(IsValidMapping, finalTarget.Target, ItemsTarget) == true) {
								controlAdaptor.CheckControl();
							}
						
						canaccessFinal = true;
					}
			
			return (res);
		}
		
		/// <summary>
		/// Notifies all connected parties about new Target
		///
		/// The only ones connected here are ControlAdaptor types which took care
		/// of the controls
		/// </summary>
		/// <param name="aAdaptor">
		/// Calling adaptor <see cref="IAdaptor"/>
		/// </param>
		public void AdaptorTargetChanged (IAdaptor aAdaptor)
		{
			OnTargetChanged (this);
		}
		
		/// <summary>
		/// Activates GetRequest for this Adaptor.
		///
		/// GetRequest transfers data from DataSource to connected Adaptors and their controls
		/// </summary>
		public void GetRequest()
		{
			AdapteeDataChanged (FinalTarget);
		}
		
		/// <summary>
		/// Calls PostRequest for the Target
		/// </summary>
		public void PostMethod()
		{
			if (IsBoundaryAdaptor == true)
				return;
			if (postRequested != null)
				postRequested();
			DataSourceController.PostRequest (Target);
		}
		
		/// <summary>
		/// Executes anonymous delegate event in specialized terms. For example
		/// all data transfers in Gtk.DataBindings have to be executed trough
		/// Gtk.Invoke
		/// Basically just provides for user function to be executed in right
		/// conditions
		/// </summary>
		/// <param name="aEvent">
		/// Event to be executed <see cref="AnonymousDelegateEvent"/>
		/// </param>
		public virtual void ExecuteUserMethod (AnonymousDelegateEvent aEvent)
		{
			if (aEvent != null)
				aEvent();
		}
		
		/// <summary>
		/// Disconnects Adaptor. Called automatically on destroy to speed up cleaning
		/// </summary>
		public virtual void Disconnect()
		{
			destroyed = true;
			InheritedTarget = false;
			if ((isBoundary == false) && (isDataAdaptor == false))
				ConnectionProvider.DisconnectControlAndAdaptor (Control, this);
			DataSourceController.RemoveAdaptor (this);
			dataChanged = null;
			targetChanged = null;
			postRequested = null;
			Target = null;
			if (finalTarget != null)
				finalTarget.Target = null;
			finalTarget = null;
			if (lastFinalTarget != null)
				lastFinalTarget.Target = null;
			lastFinalTarget = null;
			controlAdaptor = null;
			if (values != null) {
				values.Disconnect();
				values = null;
			}
		}
		
		/// <summary>
		/// Creates new adaptor
		/// </summary>
		[ToDo ("Solve problem with initialization, merely from aesthetic point")]
		public Adaptor()
		{
			id = DataSourceController.AssignId();
			CursorMode = true;
			values = new ValueList (this);
			isBoundary = false;
			singleMappingOnly = false;
			controlAdaptor = null;
			isDataAdaptor = true;
			DataSourceController.AddAdaptor (this);
			//TODO: Solve this problem, it doesn't initialize correctly???
			// Not really nice workaraound for first initialization, assign anything and null??? And then it works???
			// Not serious, It can wait. KNOWN REGRESSION
			DateTime dt = DateTime.Now;
			Target = dt;
			Target = null;
		}
		
		/// <summary>
		/// Creates new adaptor
		/// </summary>
		/// <param name="aIsBoundary">
		/// Defines if this adaptor is boundary or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aControlAdaptor">
		/// Control adaptor which created this adaptor <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control owning this adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMappingOnly">
		/// Set true if this adaptor allows single (master) mapping only <see cref="System.Boolean"/>
		/// </param>
		public Adaptor(bool aIsBoundary, ControlAdaptor aControlAdaptor, object aControl, bool aSingleMappingOnly)
		{
			id = DataSourceController.AssignId();
			if (CheckControlType(aControl) == false)
				throw new ExceptionAdaptorConnectedWithWrongWidgetType (this, aControl); 
			values = new ValueList (this);
			isBoundary = aIsBoundary;
			singleMappingOnly = aSingleMappingOnly;
			controlAdaptor = aControlAdaptor;
			if (controlAdaptor != null) {
				if (aIsBoundary == false)
					ConnectionProvider.ConnectControlAndAdaptor (Control, this);
				DataSourceController.AddAdaptor (this);
				if (aIsBoundary == false)
					if (aControl != null)
						if (controlAdaptor.ControlIsContainer(aControl) == false)
							InheritedTarget = true;
			}
		}
		
		/// <summary>
		/// Calls disconnect()
		/// </summary>
		~Adaptor()
		{
			Disconnect();
		}
	}
}
