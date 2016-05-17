// Observeable.cs - Observeable implementation for Gtk#Databindings
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

namespace System.Data.Bindings
{
	/// <summary>
	/// Any class derived from this one is automatically avoiding the DataSourceConnector registry
	/// Meaning, it doesn't need to be searched, it provides complete functionality to provide
	/// live data and to provide complete functionality for accessing data from the connected controls
	///
	/// There is a catch. You have to do the notifications inside class with
	///   HasChanged = true;
	///
	/// Secondary function provided with Observeable class is handling of the parent objects
	/// It can have multiple parent objects and all will e notified about this change
	/// </summary>
	/// <remarks>
	/// Use this kind of object when you want to provide notifications, otherwise simply use
	/// standard objects and call
	///   DataSourceController.GetRequest (object);
	/// where object is the object you wan't to provide notification for
	/// </remarks>
	[ToDo ("Check if parents are still needed after ObservableList change")]
	public class Observeable : IObserveable
	{
		private int stateCounter = 0;
		private int freezeCount = 0;
		private bool updateInProgress = false;
		//TODO: Change this later with something lighter like WeakReference[], it is slower but a lot less memory hungry
		// Must be some kind of list because one child can have as many parents as one would like
		// this is how all parrents will be notified about this childs change
		internal ArrayList parents = null;

		private bool hasCalledForChange = false;
		/// <value>
		/// Returns if object has unresolved calls for change or not
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
		/// As soon as control connects to this interface , control also connects to this
		/// event notification. This way object can request all data without even being 
		/// referenced directly to it
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
		/// Redirects call to DoGetRequest
		/// </summary>
		public void GetRequest()
		{
			DoGetRequest();
//			DataSourceController.GetRequest (this);
		}
		
		/// <summary>
		/// Activates GetRequest for this Observeable.
		///
		/// GetRequest transfers data from DataSource to connected Adaptors and their controls
		/// </summary>
		private void DoGetRequest()
		{
			AdapteeDataChanged (Target);
		}
		
		/// <summary>
		/// Adds parent to the list of parents which will be notified on this objects change
		/// </summary>
		/// <param name="aObject">
		/// Adds parent to object <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// There can be more than one parent object, all will be notified when this one changes
		/// </remarks>
		public void AddParent (object aObject)
		{
			if (aObject == null)
				return;
				
			if (parents == null)
				parents = new ArrayList();
			parents.Add (new WeakReference(aObject));
		}
		
		/// <summary>
		/// Adds parent to the list of parents which will be notified on this objects change
		/// </summary>
		/// <param name="aObject">
		/// Parent object which needs removal <see cref="System.Object"/>
		/// </param>
		public void RemoveParent (object aObject)
		{
			if ((aObject == null) || (parents == null))
				return;
			
			for (int i=parents.Count-1; i>=0; i--) {
				// Remove if target is null
				if (((WeakReference) parents[i]).Target == null)
					parents.RemoveAt (i);
				else
					if (((WeakReference) parents[i]).Target == aObject) {
						// Remove object and went out as soon as possible
						parents.RemoveAt (i);
						break;
					}
			}
			
			if (parents.Count == 0)
				parents = null;
		}
		
		/// <summary>
		/// Receiving a notification about child being changed
		/// </summary>
		/// <param name="aObject">
		/// Called when observeable child changes <see cref="IObserveable"/>
		/// </param>
		/// <param name="aIdx">
		/// Path index to child <see cref="System.Int32"/>
		/// </param>
		public virtual void ChildChanged (IObserveable aObject, int[] aIdx)
		{
			// Notify all parents about this change
			if (parents != null)
				foreach (WeakReference wr in parents)
					if (wr != null) 
						if (wr.Target != null)
							if (wr.Target is IObserveable)
								if (wr.Target is IObserveableList)
									(wr.Target as IObserveableList).ListChildChanged (aObject, EListAction.Change, null);
								else
									if (wr.Target is IObserveable)
										(wr.Target as IObserveable).ChildChanged (aObject, aIdx);
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
		/// Returns state this datasource is in
		/// </summary>
		public EObserveableState State {
			get {
				if (stateCounter > 0)
					return (EObserveableState.GetDataInProgress);
				else
					if (stateCounter > 0)
						return (EObserveableState.PostDataInProgress);
					else
						return (EObserveableState.None);
			}
		}
		
		/// <summary>
		/// Simplification reason only
		/// </summary>
		public object Target {
			get { return (this); }
		}
		
		/// <summary>
		/// Freezes calling update notifiers
		/// </summary>
		/// <returns>
		/// Current freeze count <see cref="System.Int32"/>
		/// </returns>
		public virtual int Freeze()
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
		public virtual int Unfreeze()
		{
			freezeCount--;
			if (freezeCount <= 0) {
				freezeCount = 0;
				if (updateInProgress == true) {
					updateInProgress = false;
					Changed();
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
		public bool SetState(EObserveableState aState)
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
		}
		
		/// <summary>
		/// Resolves Data from all connected controls
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
		/// Notifies all connected parties
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public virtual void AdapteeDataChanged (object aSender)
		{
			if ((IsFrozen == true) || (State == EObserveableState.PostDataInProgress)) {
				updateInProgress = true;
				return;
			}

			if (SetState(EObserveableState.GetDataInProgress) == true) {
				updateInProgress = false;
				if (dataChanged != null)
					dataChanged (aSender);
				// Notify parent objects about their change
				ChildChanged (this, null);
				SetState (EObserveableState.None);

				// Was another requested inside???
//				if (updateInProgress == true)
//					AdapteeDataChanged (aSender);
			}
			else
				HasChanged = true;
		}

		/// <summary>
		/// Simplest way to notify controls about the change in object
		/// </summary>
		public void Changed()
		{
			AdapteeDataChanged (this);
		}

		/// <summary>
		/// Disconnects and destroys Observeable object
		/// </summary>
		~Observeable()
		{
			// Hate this, but this is the only way to be sure in GC
			parents.Clear();
			parents = null;
			dataChanged = null;
			postRequested = null;
		}
	}
}
