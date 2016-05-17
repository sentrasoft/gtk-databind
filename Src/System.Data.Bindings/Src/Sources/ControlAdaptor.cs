// ControlAdaptor.cs - ControlAdaptor implementation for Gtk#Databindings
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
using System.Reflection;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Handles all connections to the specific IAdaptor, which simplifies coding of new
	/// controls, basically all that control needs to be Adaptable is redirect IAdaptableControl
	/// properties to controlled Adaptor, and that's it
	/// 
	/// It includes caching of parent DataSource and few other tricks which would otherwise have to
	/// be implemented for each control separately
	/// </summary>
	public class ControlAdaptor
	{
		private bool destroyed = false;
		private bool avoidPost = false;

		private bool throwException = false;
		/// <summary>
		/// Defines if Adaptor expects exception to be thrown
		/// </summary>
		public bool ThrowException {
			get { return (throwException); }
			set { throwException = value; } 
		}

		private bool dataChanged = false;
		/// <summary>
		/// Widgets should simply set this property to true no matter if they use
		/// Bindings or not.
		/// 
		/// Setting this property to true means pinging internal engine of Bindings
		/// which will 
		/// </summary>
		[ToDo ("Check if calling postroutine is needed")]
		public bool DataChanged {
			get { return (dataChanged); }
			set {
				if (value == dataChanged)
					return;
				dataChanged = value;
				if (dataChanged == false)
					return;
				if (TypeValidator.IsCompatible(Control.GetType(), typeof(IChangeableControl)) == true)
				{
					//TODO
					// Call post routine
				}
			}
		}

		private bool destroyInProgress = false;
		/// <value>
		/// Specifies if destroy is in progress
		/// </value>
		public bool DestroyInProgress {
			get { return (destroyInProgress); }
		}
		
		private WeakReference parentContainer = new WeakReference (null);
		/// <summary>
		/// Specifies last used parent container
		/// </summary>
		public object ParentContainer {
			get { return (parentContainer.Target); }
			set { parentContainer.Target = value; }
		}
		
		private bool disableMappingsDataTransfer = false;
		/// <summary>
		/// Disable automatical transfer of secondary mappings
		/// </summary>
		public bool DisableMappingsDataTransfer {
			get { return (disableMappingsDataTransfer); }
			set { disableMappingsDataTransfer = value; } 
		}

		private bool checkup = false;
		/// <value>
		/// true if CheckControl is in progress
		/// </value>
		public bool ControlCheckup {
			get { return (ControlCheckup); }
		}

		private WeakReference control = null;
		/// <summary>
		/// Connected control
		/// </summary>
		public object Control {
			get {
				if (control != null)
					return (control.Target);
				return (null);
			}
		}

		private IAdaptor adaptor = null;
		/// <summary>
		/// Resolve IAdaptor for DataSource
		/// </summary>
		public IAdaptor Adaptor {
			get { return (adaptor); }
		}
		
		private IAdaptor boundaryAdaptor = null;
		/// <summary>
		/// Resolve IAdaptor for BoundaryDataSource
		/// </summary>
		public IAdaptor BoundaryAdaptor {
			get { return (boundaryAdaptor); }
		}
		
		/// <summary>
		/// Returns if Adaptor is controlling widget or not. This makes
		/// the same control usable with Adaptor or with classic approach
		/// </summary>
		/// <returns>
		/// Result of Adaptor.IsControllingWidget
		/// </returns>
		public bool IsControllingWidget {
			get {
				if (adaptor != null)
					return (adaptor.IsControllingWidget);
				return (false);
			}
		}

		/// <summary>
		/// Returns if this adaptor is active or not
		/// </summary>
		public bool Activated {
			get {
				if (adaptor == null)
					return (false);
				return (adaptor.Activated);
			}
		}
		
		/// <summary>
		/// Checks if Get request is possible in this moment
		/// </summary>
		public bool CanGet {
			get {
				if (Activated == false)
					return (false);
				IObserveable observer = DataSourceController.GetInfoFor (adaptor.FinalTarget);
				if (observer == null) 
					return (false);
				bool res = observer.CanGet;
				observer = null;
				return (res);
			}
		}
		
		/// <summary>
		/// Checks if Get request is possible in this moment
		/// </summary>
		public bool CanPost {
			get {
				if (Activated == false)
					return (false);
				IObserveable observer = DataSourceController.GetInfoFor (adaptor.FinalTarget);
				if (observer == null) 
					return (false);
				bool res = observer.CanPost;
				observer = null;
				return (res);
			}
		}
		
		/// <summary>
		/// Defines if BoundaryDataSource is inherited fom parent controls or not
		/// </summary>
		public bool InheritedBoundaryDataSource {
			get { 
				if (boundaryAdaptor == null)
					return (false);
				return (boundaryAdaptor.InheritedTarget); 
			}
			set { boundaryAdaptor.InheritedTarget = value; }
		}
		
		/// <summary>
		/// BoundaryDataSource object control is connected to
		/// </summary>
		public IObserveable BoundaryDataSource {
			get { 
				if (boundaryAdaptor == null)
					return (null);
				return ((IObserveable) boundaryAdaptor.Target); 
			}
			set { boundaryAdaptor.Target = value; }
		}
		
		/// <summary>
		/// Link to BoundaryMappings in connected Adaptor 
		/// </summary>
		public string BoundaryMappings { 
			get { 
				if (boundaryAdaptor == null)
					return ("");
				return (boundaryAdaptor.Mappings); 
			}
			set { boundaryAdaptor.Mappings = value; }
		}
		
		/// <summary>
		/// Defines if DataSource should be resolved from parent IAdaptableControls
		/// It also keeps cache about the last valid DataSource it found
		/// </summary>
		public bool InheritedDataSource {
			get { 
				if (adaptor == null)
					return (false);
				return (adaptor.InheritedTarget); 
			}
			set { adaptor.InheritedTarget = value; }
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		public object DataSource {
			get { 
				if (adaptor == null)
					return (null);
				return (adaptor.Target); 
			} 
			set {
/*				if ((InheritedDataSource == true) && (value == null)) {
					CheckControl();
					return;
				}*/
					
				if (InheritedDataSource == true)
					InheritedDataSource = false;

				adaptor.Silence = true;
				if (adaptor.Target != value)
					adaptor.Target = value;
				adaptor.Silence = false;
/*				if (checkup == false)
					CheckControl();*/
//				CheckControlState();
			}
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		public string Mappings { 
			get { 
				if (adaptor == null)
					return ("");
				return (adaptor.Mappings); 
			}
			set { 
				adaptor.Mappings = value;
/*				if (checkup == false)
					CheckControl();*/
//				CheckControlState();
			}
		}
		
		/// <summary>
		/// Link to default Value in connected Adaptor 
		/// </summary>
		public object Value { 
			get {
				if (adaptor == null)
					throw new ExceptionAccessAdaptorInContainerControl();
				return (adaptor.GetDefaultMappingValue()); 
			}
			set { 
				if (adaptor == null)
					throw new ExceptionAccessAdaptorInContainerControl();
				adaptor.SetDefaultMappingValue(value);
			}
		}
		
		/// <summary>
		/// Links to other values by name
		/// </summary>
		public ValueList Values {
			get { return (adaptor.Values); }
		}
		
		private event CustomGetDataEvent customGetData = null;
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
			add { customGetData += value; }
			remove { customGetData -= value; }
		}
		
		private event CustomPostDataEvent customPostData = null;
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
			add { customPostData += value; }
			remove { customPostData -= value; }
		}
		
		/// <summary>
		/// Sends message to its adaptor if adaptor is valid
		/// </summary>
		/// <param name="aSender">
		/// Object sending message <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Message context <see cref="EActionType"/>
		/// </param>
		public void SendAdaptorMessage (object aSender, EActionType aAction)
		{
			if (Adaptor != null)
				Adaptor.SendAdaptorMessage (aSender, aAction);
		}

		/// <summary>
		/// Checks if boundary adaptor is allowed
		/// </summary>
		/// <returns>
		/// true if allowed, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Returns true by default
		/// </remarks>
		public virtual bool IsBoundaryAdaptorAllowed()
		{
			return (true);
		}
		
		/// <summary>
		/// Transfers default value from Control to DataSource as mapping and makes all
		/// change notifications
		/// </summary>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		public void PutDefaultValueToDataSourceAs (string aMapping)
		{
			object DS = ConnectionProvider.ResolveTargetFor (Adaptor);
			if ((Control == null) || (DS == null)) {
				DS = null;
				return;
			}
				
			foreach (MappedProperty mp in Values)
				if (mp != null)
					if (mp.IsDefault == true) {
						mp.AssignValueToObject (EDataDirection.FromControlToDataSource, DS, Control, aMapping);
						DS = null;
						return;
					}
			DS = null;
		}
		
		/// <summary>
		/// Transfers default value from DataSource to Control as mapping and makes all
		/// change notifications
		/// </summary>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		public void GetDefaultValueFromDataSource (string aMapping)
		{
			object DS = ConnectionProvider.ResolveTargetFor (Adaptor);
			if ((Control == null) || (DS == null)) {
				DS = null;
				return;
			}
			
			foreach (MappedProperty mp in Values)
				if (mp.IsDefault == true) {
					mp.AssignValueToObject (EDataDirection.FromDataSourceToControl, DS, Control, aMapping);
					DS = null;
					return;
				}
			DS = null;
		}
		
		/// <summary>
		/// Transfers all secondary values from DataSource to Control and notifies about
		/// change if needed
		/// </summary>
		public void GetValuesFromDataSource ()
		{
			Adaptor.FillObjectWithDataSourceValues (Control);
/*			object DS = ConnectionProvider.ResolveTargetFor (Adaptor);
			if ((Control == null) || (DS == null)) {
				DS = null;
				return;
			}

			foreach (MappedProperty mp in Values) {
				if (mp.IsSecondaryTargeted == true)
					mp.AssignValueToObject (EDataDirection.FromDataSourceToControl, DS, Control);
			}
			DS = null;*/
		}
		
		/// <summary>
		/// Transfers all secondary values from Control to DataSource and notifies about
		/// change if needed
		/// </summary>
		public void PutValuesToDataSource ()
		{
			adaptor.FillDataSourceWithObjectValues (Control);
/*			object DS = ConnectionProvider.ResolveTargetFor (Adaptor);
			if ((Control == null) || (DS == null)) {
				DS = null;
				return;
			}
				
			foreach (MappedProperty mp in Values)
				if (mp != null)
					if (mp.IsSecondaryTargeted == true)
						mp.AssignValueToObject (EDataDirection.FromControlToDataSource, DS, Control);
			DS = null;*/
		}
		
		/// <summary>
		/// Transfers all secondary values from DataSource to Control and notifies about
		/// change if needed
		/// </summary>
		protected void GetBoundaryValuesFromDataSource ()
		{
			if (BoundaryAdaptor != null)
				BoundaryAdaptor.FillObjectWithDataSourceValues (Control);
/*			object DS = ConnectionProvider.ResolveTargetFor (BoundaryAdaptor);
			if ((Control == null) || (DS == null)) {
				DS = null;
				return;
			}

			foreach (MappedProperty mp in Values) {
				if (mp.IsSecondaryTargeted == true)
					mp.AssignValueToObject (EDataDirection.FromDataSourceToControl, DS, Control);
			}
			DS = null;*/
		}
		
		/// <summary>
		/// Assigns data or disqualifies validity if needed
		/// </summary>
		public void CheckControlState()
		{
			bool canuse = adaptor.IsValidMapping;
			if (canuse == false) {
				//DebugAdaptor();
				if (this.InheritedDataSource == true) {
					InheritedDataSource = false;
					InheritedDataSource = true;
					canuse = adaptor.IsValidMapping;
				}
			}
			if ((Control is IContainerControl) == false)
				SetControlSensitivity (Control, canuse);
		}

		/// <summary>
		/// Writes debug information about this adaptor
		/// </summary>
		public virtual void DebugAdaptor()
		{
			System.Console.WriteLine("Target=" + Adaptor.FinalTarget);
			System.Console.WriteLine("Control=" + Control);
		}
		
		/// <summary>
		/// Calls controls GetDataFromDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call GetDataFromDataSource <see cref="IChangeableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		public void InvokeAdapteeDataChange (IChangeableControl aControl, object aSender)
		{
			avoidPost = true;
			if (adaptor.IsControllingWidget == true)
				if (CanGet == true) {
					if (SetState(EObserveableState.GetDataInProgress) == false) {
						avoidPost = false;
						return;
					}
					InvokeControlAdapteeDataChange (aControl, aSender);
					SetState (EObserveableState.None);
				}
			avoidPost = false;
		}
		
		/// <summary>
		/// Calls controls GetDataFromDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call GetDataFromDataSource <see cref="IChangeableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		public virtual void InvokeControlAdapteeDataChange (IChangeableControl aControl, object aSender)
		{
			if (TypeValidator.IsCompatible(aControl.GetType(), typeof(ICustomGetData)) == true)
				if (ActivateUserGetData() == true) 
					return;
			// Since control will load new values clear flag that data has changed
			DataChanged = false;
			// Transfer data
			aControl.GetDataFromDataSource (aSender);
			if (DisableMappingsDataTransfer == false)
				if (TypeValidator.IsCompatible(adaptor.GetType(), typeof(Adaptor)) == true)
					if ((adaptor as Adaptor).SingleMappingOnly == false)
						GetValuesFromDataSource();
		}
		
		/// <summary>
		/// Calls controls PutDataToDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call PutDataToDataSource <see cref="IChangeableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		internal void InvokeControlDataChange (IPostableControl aControl, object aSender)
		{
			if (avoidPost == true) 
				return;
			if (adaptor.IsControllingWidget == true)
				if (CanPost == true) {
					if (SetState(EObserveableState.PostDataInProgress) == false)
						return;
					InvokeDirectControlDataChange (aControl, aSender);
					SetState (EObserveableState.None);
				}
		}
		
		/// <summary>
		/// Calls controls PutDataToDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call PutDataToDataSource <see cref="IPostableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		public virtual void InvokeDirectControlDataChange (IPostableControl aControl, object aSender)
		{
			if (TypeValidator.IsCompatible(aControl.GetType(), typeof(ICustomPostData)) == true)
				if (ActivateUserPostData() == true)  
					return;			
			// Since control will load new values clear flag that data has changed
			DataChanged = false;
			// Transfer data
			aControl.PutDataToDataSource (aSender);
			if (DisableMappingsDataTransfer == false)
				if (TypeValidator.IsCompatible(adaptor.GetType(), typeof(Adaptor)) == true)
					if ((adaptor as Adaptor).SingleMappingOnly == false)
						PutValuesToDataSource();
		}
		
		/// <summary>
		/// Calls controls GetDataFromDataSource, in case of gtk this handles being called
		/// in the right thread
		/// </summary>
		/// <param name="aControl">
		/// Control to call GetBoundaryValuesFromDataSource <see cref="IChangeableControl"/>
		/// </param>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// By overriding this method one can handle things differently. Specific
		/// example of this is GTK+ which needs to call control changes in its master thread 
		/// </remarks>
		public void InvokeBoundaryDataChange (IBoundedContainer aControl, object aSender)
		{
			InvokeControlBoundaryDataChange (aControl, aSender);
		}
		
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
		protected virtual void InvokeControlBoundaryDataChange (IBoundedContainer aControl, object aSender)
		{
			GetBoundaryValuesFromDataSource();
		}

		/// <summary>
		/// Activates OnUserGetDataEvent
		/// </summary>
		/// <returns>
		/// true if event was called, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool ActivateUserGetData()
		{
			if (customGetData != null) { 
				customGetData (this, new CustomDataTransferArgs(this, adaptor.FinalTarget, EDataDirection.FromDataSourceToControl));
				return (true);
			}
			return (false);
		}
		
		/// <summary>
		/// Activates OnUserGetDataEvent
		/// </summary>
		/// <returns>
		/// true if event was called, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool ActivateUserPostData()
		{
			if (customPostData != null) { 
				customPostData (this, new CustomDataTransferArgs(this, adaptor.FinalTarget, EDataDirection.FromControlToDataSource));
				return (true);
			}
			return (false);
		}
		
		/// <summary>
		/// Assigns data or disqualifies validity if needed
		/// </summary>
		public void CheckControl()
		{
			if (destroyed == true)
				return;
			if (Activated == false)
				return;
			if (checkup == true)
				return;
			if (IsControllingWidget == false)
				return;
			checkup = true;
			bool canuse = adaptor.IsValidMapping;
			
			CheckControlState();
			
			IObserveable observer = null;
			IObserveable boundaryobserver = null;
			if (Adaptor != null)
				observer = DataSourceController.GetInfoFor (Adaptor.FinalTarget);
			if (Adaptor != null)
				boundaryobserver = DataSourceController.GetInfoFor (BoundaryAdaptor.FinalTarget);
			if ((canuse == true) && (Control != null))
				if (Control is IChangeableControl) {
					if (observer != null)
						if (observer.CanGet == true)
							InvokeAdapteeDataChange ((Control as IChangeableControl), Adaptor.FinalTarget);
					if (boundaryobserver != null)
						if (boundaryobserver.CanGet == true)
							if (TypeValidator.IsCompatible(Control.GetType(), typeof(IBoundedContainer)) == true)
								InvokeBoundaryDataChange ((Control as IBoundedContainer), this);
				}
			observer = null;
			boundaryobserver = null;
			checkup = false;
		}
		
		/// <summary>
		/// Valid for complex controls like TreeView
		/// </summary>
/*		public virtual void ClearBeforeRemapping()
		{
			if (Control is IComplexAdaptableControl)
				(Control as IComplexAdaptableControl).ClearBeforeRemapping();
		}
		
		/// <summary>
		/// Valid for complex controls like TreeView
		/// </summary>
		public virtual void RemapControl()
		{
			if (Control is IComplexAdaptableControl)
				(Control as IComplexAdaptableControl).RemapControl();
		}*/
		
		/// <summary>
		/// Sets method of DataSource update
		/// </summary>
		/// <param name="aMethod">
		/// Method of update <see cref="EApplyMethod"/>
		/// </param>
		/// <returns>
		/// true if succeessful <see cref="System.Boolean"/>
		/// </returns>
		public bool DataSourceNeedsUpdateOn (EApplyMethod aMethod)
		{
			return (DataSourceController.DataSourceNeedsUpdateOn (adaptor.FinalTarget, aMethod));
		}

		/// <summary>
		/// Outputs connection information with Debug.DevelInfo
		/// </summary>
		public virtual void DebugConnection()
		{
			string preffix="  ";
			if (Adaptor.FinalTarget == null)
				return;
			string s = "Control=" + Control + " Target=" + Adaptor.FinalTarget;
			if (TypeValidator.IsCompatible(Adaptor.FinalTarget.GetType(), typeof(IAdaptor)) == true) {
				IAdaptor a = (IAdaptor) Adaptor.FinalTarget;
				while (TypeValidator.IsCompatible(a.GetType(), typeof(IAdaptor)) == true) {
					if (a.Control == null)
						s = s + "\n" + preffix + "Control=[POINTER] Target=" + a.FinalTarget;
					else
						s = s + "\n" + preffix + "Control=" + a.Control + " Target=" + a.FinalTarget;
					preffix += "  ";
				}
				s = s + "\n" + preffix + "Control=" + a.Control + " Target=" + a.FinalTarget;
			}
			Debug.DevelInfo("ControlAdaptor.DebugConnection (" + Control + ")", s);
		}

		/// <summary>
		/// Outputs connection information with Debug.DevelInfo if aControl is
		/// IAdaptableContainer
		/// </summary>
		/// <param name="aControl">
		/// Control which needs to be debugged <see cref="System.Object"/>
		/// </param>
		public void DebugConnection (object aControl)
		{
			if (aControl is IAdaptableContainer)
				(aControl as IAdaptableContainer).Adaptor.DebugConnection();
			else
				Debug.DevelInfo ("ControlAdaptor.DebugConnection (" + aControl + ")", "Cannot be debugged");
		}
		
		/// <summary>
		/// Checks if Get request is possible in this moment
		/// </summary>
		/// <param name="aState">
		/// State <see cref="EObserveableState"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool SetState (EObserveableState aState)
		{
			IObserveable observer = DataSourceController.GetInfoFor (adaptor.FinalTarget);
			if (observer == null) 
				return (false);
			bool res = observer.SetState(aState);
			observer = null;
			return (res);
		}
		
		/// <summary>
		/// Validates type of the control if it is compatible with adaptor. This method 
		/// needs to be overriden in ControlAdaptor subclasses
		/// </summary>
		/// <param name="aControl">
		/// Control to validate type for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is right type, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws exception
		/// </remarks>
		public virtual bool ValidateControlType (object aControl)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "ValidateConotrolType");
		}

		/// <summary>
		/// Checks if control is Window type. This method 
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
		public virtual bool ControlIsWindow (object aControl)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "ControlIsWindow");
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
		/// Returns false
		/// </remarks>
		public virtual bool ControlIsContainer (object aControl)
		{
			return (false);
		}
		
		/// <summary>
		/// Resolves ReadOnly of this and bases result on parent controls
		/// </summary>
		/// <param name="aControl">
		/// Control whos state should be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// false by default if there is no IReadOnly classes <see cref="System.Object"/>
		/// </returns>
		public virtual bool ResolveReadOnly (object aControl)
		{
			if (aControl == null)
				return (false);
			if (aControl is IEditable)
				if ((aControl as IEditable).Editable == true)
					return (true);

			return (ResolveReadOnly (GetParentOfControl (aControl)));
		}
		
		/// <summary>
		/// Checks if control has Parent window and reeturns reference to it. This method 
		/// needs to be overriden in ControlAdaptor subclasses
		/// </summary>
		/// <param name="aControl">
		/// Control to check for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent window if there is one <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Throws exception
		/// </remarks>
		public virtual object ParentWindow (object aControl)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "ParentWindow");
		}
		
		/// <summary>
		/// Creates new Adaptor instance. Use this to overclass Adaptor with derived type
		/// </summary>
		/// <remarks>
		/// Example of overriding:
		/// <code>
		/// public virtual Adaptor CreateAdaptorInstance (bool aIsBoundary, object aControl, bool aSingleMapping)
		/// {
		///    return (new XXXXXAdaptor (aIsBoundary, this, aControl, aSingleMapping));
		/// }
		/// </code>
		/// </remarks>
		/// <returns>
		/// Throws exception, but subclasses should return instance to Adaptor object
		/// </returns>
		public virtual Adaptor CreateAdaptorInstance (bool aIsBoundary, object aControl, bool aSingleMapping)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "CreateAdaptorInstance");
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
		public virtual object GetParentOfControl (object aControl)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "GetParentOfControl");
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
		/// <remarks>
		/// Throws exception
		/// </remarks>
		public virtual void SetControlSensitivity (object aControl, bool aSensitive)
		{
			throw new ExceptionDescedantOverrideMethod (this.GetType(), "SetControlSensitivity");
		}

		/// <summary>
		/// Subscribers should call this routine when widget changes. If DataSource
		/// supports EApplyMethod.Instant then it will simply invoke this adaptors 
		/// Post routine
		/// </summary>
		public virtual void DemandInstantPost()
		{
			dataChanged = true;
			if (Control == null)
				return;
			if (TypeValidator.IsCompatible(Control.GetType(), typeof(IPostableControl)) == true)
				if (DataSourceNeedsUpdateOn(EApplyMethod.Instant) == true)
					InvokeControlDataChange (Control as IPostableControl, this);
		}
		
		/// <summary>
		/// Subscribers should call this routine when widget changes. If DataSource
		/// supports EApplyMethod.Instant then it will simply invoke this adaptors 
		/// Post routine
		/// </summary>
		public virtual void DemandOnLeavePost()
		{
			dataChanged = true;
			if (TypeValidator.IsCompatible(Control.GetType(), typeof(IPostableControl)) == true)
				if (DataSourceNeedsUpdateOn(EApplyMethod.OnLeave) == true)
					InvokeControlDataChange (Control as IPostableControl, this);
		}
		
		/// <summary>
		/// Connects base control events like gaining or loosing focus etc.
		/// 
		/// ControlAdaptor subclasses should override this method to connect
		/// to specific widget set events 
		/// </summary>
		protected virtual void ConnectControlEvents()
		{
		}
		
		/// <summary>
		/// Disconnects base control events like gaining or loosing focus etc.
		/// 
		/// ControlAdaptor subclasses should override this method to connect
		/// to specific widget set events 
		/// </summary>
		protected virtual void DisconnectControlEvents()
		{
		}
		
		/// <summary>
		/// Unlink mappings in connected Adaptor
		/// </summary>
		public void Disconnect()
		{
			destroyInProgress = true;
			if (control != null)
				DisconnectControlEvents();
			customGetData = null;
			customPostData = null;
			destroyed = true;
			if (adaptor != null) {
				DataSource = null;
				adaptor.DestroyInProgress = true;
				adaptor.Disconnect();
				adaptor = null;
			}
			if (boundaryAdaptor != null) {
				boundaryAdaptor.DestroyInProgress = true;
				BoundaryDataSource = null;
				boundaryAdaptor.Disconnect();
				boundaryAdaptor = null;
			}
			control = null;
		}

		/// <summary>
		/// Creates ControlAdaptor
		/// </summary>
		protected ControlAdaptor()
		{
		}

		/// <summary>
		/// Creates ControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control being controlled <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// true if control allows only one single mapping <see cref="System.Boolean"/>
		/// </param>
		public ControlAdaptor(object aControl, bool aSingleMapping)
		{
			if (ValidateControlType(aControl) == false)
				throw new ExceptionControlAdaptorConnectedWithWrongWidgetType (this, aControl);
			control = new WeakReference(aControl);
			if (control.Target != null)
				ConnectControlEvents();
			if ((aControl is IAdaptableContainer) || (aControl is IAdaptableObjectReader))
				adaptor = CreateAdaptorInstance (false, aControl, aSingleMapping);
			else
				adaptor = null;
			if (IsBoundaryAdaptorAllowed() == true)
				boundaryAdaptor = CreateAdaptorInstance (true, aControl, false);
			else
				boundaryAdaptor = null;
		}

		/// <summary>
		/// Creates ControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control being controlled <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// true if control allows only one single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings associated by this adaptor <see cref="System.String"/>
		/// </param>
		public ControlAdaptor (object aControl, bool aSingleMapping, string aMappings)
		{
			if (ValidateControlType(aControl) == false)
				throw new ExceptionControlAdaptorConnectedWithWrongWidgetType (this, aControl);
			control = new WeakReference(aControl);
			if (control.Target != null)
				ConnectControlEvents();
			if ((aControl is IAdaptableContainer) || (aControl is IAdaptableObjectReader))
				adaptor = CreateAdaptorInstance (false, aControl, aSingleMapping);
			else
				adaptor = null;
			if (IsBoundaryAdaptorAllowed() == true)
				boundaryAdaptor = CreateAdaptorInstance (true, aControl, false);
			else
				boundaryAdaptor = null;
			Mappings = aMappings;
		}

		/// <summary>
		/// Creates ControlAdaptor
		/// </summary>
		/// <param name="aControl">
		/// Control being controlled <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// true if control allows only one single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource to be connected with adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings associated by this adaptor <see cref="System.String"/>
		/// </param>
		public ControlAdaptor (object aControl, bool aSingleMapping, object aDataSource, string aMappings)
		{
			if (ValidateControlType(aControl) == false)
				throw new ExceptionControlAdaptorConnectedWithWrongWidgetType (this, aControl);
			control = new WeakReference(aControl);
			if (control.Target != null)
				ConnectControlEvents();
			if ((aControl is IAdaptableContainer) || (aControl is IAdaptableObjectReader))
				adaptor = CreateAdaptorInstance (false, aControl, aSingleMapping);
			else
				adaptor = null;
			if (IsBoundaryAdaptorAllowed() == true)
				boundaryAdaptor = CreateAdaptorInstance (true, aControl, false);
			else
				boundaryAdaptor = null;
			DataSource = aDataSource;
			Mappings = aMappings;
		}
		
		/// <summary>
		/// Creates ControlAdaptor with custom created Adaptors
		/// </summary>
		/// <param name="aControl">
		/// Control to be connected with <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Data adaptor <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aBoundaryAdaptor">
		/// Boundary adaptor <see cref="IAdaptor"/>
		/// </param>
		public ControlAdaptor (object aControl, IAdaptor aAdaptor, IAdaptor aBoundaryAdaptor)
		{
			if (ValidateControlType(aControl) == false)
				throw new ExceptionControlAdaptorConnectedWithWrongWidgetType (this, aControl);
			control = new WeakReference(aControl);
			if (control.Target != null)
				ConnectControlEvents();
			adaptor = aAdaptor;
			if (IsBoundaryAdaptorAllowed() == true)
				boundaryAdaptor = aBoundaryAdaptor;
			else
				boundaryAdaptor = null;
		}

		/// <summary>
		/// Creates ControlAdaptor with custom created Adaptors
		/// </summary>
		/// <param name="aControl">
		/// Control to be connected with <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Data adaptor <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aBoundaryAdaptor">
		/// Boundary adaptor <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource to be connected with <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings to be set <see cref="System.String"/>
		/// </param>
		public ControlAdaptor (object aControl, IAdaptor aAdaptor, IAdaptor aBoundaryAdaptor, object aDataSource, string aMappings)
		{
			if (ValidateControlType(aControl) == false)
				throw new ExceptionControlAdaptorConnectedWithWrongWidgetType (this, aControl);
			control = new WeakReference(aControl);
			if (control.Target != null)
				ConnectControlEvents();
			adaptor = aAdaptor;
			if (IsBoundaryAdaptorAllowed() == true)
				boundaryAdaptor = aBoundaryAdaptor;
			else
				boundaryAdaptor = null;
			DataSource = aDataSource;
			Mappings = aMappings;
		}
		
		/// <summary>
		/// Disconnects and destroys ControlAdaptor
		/// </summary>
		~ControlAdaptor()
		{
			Disconnect();
		}
	}
}
