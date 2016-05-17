// DataSourceInfo.cs - DataSourceInfo implementation for Gtk#Databindings
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

namespace System.Data.Bindings
{
	/// <summary>
	/// DataSource Info for objects that don't provide IObserveable functionality
	/// This object is completely hadled by IAdaptor
	/// </summary>
	public class DataSourceInfo : IObserveable
	{
		/// <value>
		/// Target object of this DataSourceInfo
		/// </value>
		public object Target;
		
		/// <value>
		/// ReferenceCount
		/// </value>
		public int RefCount = 1;
		//private EObserveableState state = EObserveableState.None;
		private int stateCounter = 0;
		private int freezeCount = 0;
		private bool updateInProgress = false;
		
		private bool hasCalledForChange = false;
		/// <value>
		/// true if this object called for change
		/// </value>
		public bool HasCalledForChange {
			get { return (hasCalledForChange); }
		}
		
		/// <summary>
		/// If Observeable is frozen then updates will be pending until its counter reaches zero
		/// </summary>
		public bool IsFrozen {
			get { return (freezeCount > 0); }
		}
		
		/// <summary>
		/// Checks if Get request is possible in this moment
		/// </summary>
		public bool CanGet {
			get { return (! (State == EObserveableState.PostDataInProgress)); }
		}
		
		/// <summary>
		/// Checks if Get request is possible in this moment
		/// </summary>
		public bool CanPost {
			get { return (! (State == EObserveableState.GetDataInProgress)); }
		}
		
		/// <summary>
		/// Sets and invokes change if needed
		/// </summary>
		public bool HasChanged {
			get { return (updateInProgress); }
			set {
				hasCalledForChange = true;
				if (updateInProgress == value)
					return;
				updateInProgress = value;
				if (value == true)
					if (IsFrozen == false)
						GetRequest();
			}
		}
		
		private EApplyMethod applyMethod = DataSourceController.DefaultApplyMethod;
		/// <summary>
		/// The way how DataSource should receive data
		/// </summary>
		public EApplyMethod ApplyMethod {
			get { return (applyMethod); }
			set { applyMethod = value; }
		}

		private event PostMethodEvent postRequested = null;
		/// <summary>
		/// OnPostRequest event that is crossconnecting Target and controls
		/// </summary>
		public event PostMethodEvent PostRequested {
			add { postRequested += value; }
			remove { postRequested -= value; }
		}
		
		private event AdapteeDataChangeEvent dataChanged = null;
		/// <summary>
		/// OnDataChange event that is crossconnecting Target and controls
		/// </summary>
		public event AdapteeDataChangeEvent DataChanged {
			add { dataChanged += value; }
			remove { dataChanged -= value; }
		}

		/// <summary>
		/// Returns state this datasource is in
		/// </summary>
		public EObserveableState State {
			get {
				if (stateCounter > 0)
					return (EObserveableState.GetDataInProgress);
				else
					if (stateCounter < 0)
						return (EObserveableState.PostDataInProgress);
					else
						return (EObserveableState.None);
			}
		}
		
		/// <summary>
		/// Redirects call to DoGetRequest
		/// </summary>
		public void GetRequest()
		{
			DoGetRequest();
//			DataSourceController.GetRequest (this);
		}
		
		/// <summary>
		/// Activates GetRequest for this DataSource.
		///
		/// GetRequest transfers data from DataSource to connected Adaptors and their controls
		/// </summary>
		private void DoGetRequest()
		{
			AdapteeDataChanged (Target);
		}
		
		/// <summary>
		/// Provides additional functionality of live data
		/// if object hasn't called HasChanged = true, then this one will take care of this 
		/// during editing
		/// </summary>
		public void ResetChangeCallCheckup()
		{
			hasCalledForChange = false;
		}
		
		/// <summary>
		/// Receiving a notification about child being changed
		/// </summary>
		/// <param name="aObject">
		/// Child of this object has changed <see cref="IObserveable"/>
		/// </param>
		/// <param name="aIdx">
		/// Path of the changed child <see cref="System.Int32"/>
		/// </param>
		public virtual void ChildChanged (IObserveable aObject, int[] aIdx)
		{
			if (aObject == this)
				return;
		}
		
		/// <summary>
		/// Freezes calling update notifiers
		/// </summary>
		/// <returns>
		/// Current freeze count <see cref="System.Int32"/>
		/// </returns>
		public int Freeze()
		{
			freezeCount++;
			return (freezeCount);
		}
		
		/// <summary>
		/// Freezes calling update notifiers
		/// </summary>
		/// <returns>
		/// Current freeze count <see cref="System.Int32"/>
		/// </returns>
		public int Unfreeze()
		{
			freezeCount--;
			if (freezeCount <= 0) {
				freezeCount = 0;
				if (updateInProgress == true) {
					updateInProgress = false;
					GetRequest();
				}
			}
			return (freezeCount);
		}
		
		/// <summary>
		/// Locks state for this Observeable
		/// </summary>
		/// <param name="aState">
		/// New state <see cref="EObserveableState"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool SetState (EObserveableState aState)
		{
			if ((aState == EObserveableState.GetDataInProgress) && (State == EObserveableState.PostDataInProgress))
				return (false);
			if ((aState == EObserveableState.PostDataInProgress) && (State == EObserveableState.GetDataInProgress))
				return (false);
			if ((aState == EObserveableState.None) && (State == EObserveableState.None))
				return (false);
			switch (aState) {
				case EObserveableState.None :
					if (stateCounter > 0)
						stateCounter--;
					else
						stateCounter++;
					break;
				case EObserveableState.GetDataInProgress :
					stateCounter++;
					break;
				case EObserveableState.PostDataInProgress :
					stateCounter--;
					break;
			}
			if ((aState == EObserveableState.None) && (State == EObserveableState.None) && (HasChanged == true))
				GetRequest();
			return (true);
/*			if ((state == aState) || ((state != aState) && (aState != EObserveableState.None)))
				return (false);
			state = aState;
			return (true);*/
		}
		
		/// <summary>
		/// Activates PostRequest for this DataSource.
		///
		/// PostRequest transfers data from connected Adaptors and their controls to
		/// DataSource object
		/// </summary>
		public void PostRequest()
		{
			if (State == EObserveableState.GetDataInProgress)
				return;
				
			if (SetState(EObserveableState.PostDataInProgress) == true) {
				if (postRequested != null)
					postRequested();
				SetState (EObserveableState.None);
			}

			if (updateInProgress == true)
				GetRequest();
		}
		
		/// <summary>
		/// Calls every single connected event to update data in controls
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public void AdapteeDataChanged (object aSender)
		{
			if ((IsFrozen == true) && (State == EObserveableState.PostDataInProgress)) {
				updateInProgress = true;
				return;
			}
				
			if (SetState(EObserveableState.GetDataInProgress) == true) {
				updateInProgress = false;
				if (dataChanged != null)
					dataChanged (aSender);
				SetState (EObserveableState.None);

				// Was another update requested???
//				if (updateInProgress == true)
//					GetRequest();
			}
		}
		
		/// <summary>
		/// Closes all connections to help GC
		/// </summary>
		public void CloseConnections()
		{
			Target = null;
			postRequested = null;
			dataChanged = null;
		}

		/// <summary>
		/// Creates DataSourceInfo object with specified target
		/// </summary>
		/// <param name="aTarget">
		/// Target object <see cref="System.Object"/>
		/// </param>
		public DataSourceInfo (object aTarget)
		{
			Target = aTarget;
		}

		/// <summary>
		/// Creates DataSourceInfo object with specified target
		/// </summary>
		/// <param name="aTarget">
		/// Target object <see cref="System.Object"/>
		/// </param>
		/// <param name="aInstant">
		/// Apply method <see cref="EApplyMethod"/>
		/// </param>
		public DataSourceInfo (object aTarget, EApplyMethod aInstant)
		{
			Target = aTarget;
			ApplyMethod = aInstant;
		}
		
		/// <summary>
		/// Disconnects and destroys DataSourceInfo
		/// </summary>
		~DataSourceInfo()
		{
			CloseConnections();
		}
	}
}
